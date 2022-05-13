using Microsoft.AspNetCore.Mvc;
using WebParser.Models;
using System.Net.Mime;
using System.Net.Http.Headers;
using ExcelDataReader;
using System.Text;
using System.Text.Json;

namespace WebParser.Controllers
{
    public class HomeController : Controller
    {
        IWebHostEnvironment _appEnvironment;

        public HomeController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Принимает на вход файл с хим. формулами и возвраащет .json файл - описание этих хим. формул.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostFileToServerAndGetJsonDescriptionOfChemicalFormula(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                try
                {
                    StringBuilder formulas = new StringBuilder();
                    string jsonString = "";
                    string file_name = uploadedFile.FileName.Split('.')[0];
                    string file_type = uploadedFile.ContentType;
                    string apiUrl = "http://localhost:5074/api/";
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(apiUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        if (uploadedFile.FileName.Split('.')[1] == "xlsx")
                        {
                            // Чтение таблицы.
                            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                            IExcelDataReader reader = ExcelReaderFactory.CreateReader(uploadedFile.OpenReadStream());
                            var dataSet = reader.AsDataSet();
                            var dataTable = dataSet.Tables[0];

                            // Cоздание строки.
                            for (int i = 0; i < dataTable.Rows.Count; i++)
                            {
                                formulas.Append(dataTable.Rows[i][0].ToString());
                                formulas.Append("\n");
                            }
                            formulas.Remove(formulas.Length - 1, 1);
                            string serializeStr = JsonSerializer.Serialize(formulas.ToString());
                            HttpContent content = new StringContent(serializeStr, Encoding.UTF8, "application/json");
                            var response = await client.PostAsync("parser", content);
                            if (response.IsSuccessStatusCode)
                            {
                                var result = response.Content.ReadAsStringAsync();
                                result.Wait();
                                jsonString = result.Result.ToString();
                            }
                        }
                        else if (uploadedFile.FileName.Split('.')[1] == "txt")
                        {
                            using (StreamReader reader = new StreamReader(uploadedFile.OpenReadStream()))
                            {
                                while (reader.Peek() >= 0)
                                {
                                    formulas.Append(reader.ReadLine());
                                    formulas.Append("\n");
                                }
                                formulas.Remove(formulas.Length - 1, 1);
                                HttpContent content = new StringContent(JsonSerializer.Serialize(formulas.ToString()), Encoding.UTF8, "application/json");
                                var response = await client.PostAsync("parser", content);
                                if (response.IsSuccessStatusCode)
                                {
                                    var result = response.Content.ReadAsStringAsync();
                                    result.Wait();
                                    jsonString = result.Result.ToString();
                                }
                            }
                        }
                        return File(Encoding.UTF8.GetBytes(FormatJson(jsonString)), MediaTypeNames.Application.Json, file_name + ".json");
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Переводит строку json в человеко-читабельный вид.
        /// </summary>
        public static string FormatJson(string str, string indentString = "\t")
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            foreach (var e in Enumerable.Range(0, ++indent))
                                sb.Append(indentString);
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            foreach (var e in Enumerable.Range(0, --indent))
                                sb.Append(indentString);
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            foreach (var e in Enumerable.Range(0, indent))
                                sb.Append(indentString);
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}