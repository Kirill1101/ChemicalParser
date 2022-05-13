using Microsoft.AspNetCore.Mvc;
using ChemicalFormulaParser;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace WebParserApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParserController : ControllerBase
    {
        private readonly ILogger<ParserController> _logger;

        public ParserController(ILogger<ParserController> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Метод для начального экрана. Демонстрирует JSON описание хим. элемента.
        /// </summary>
        [HttpGet]
        public ChemicalSubstance Get()
        {
            ChemicalQuantity quantity = new ChemicalQuantity(2);
            ChemicalQuantity[] quantity1 = new ChemicalQuantity[] { quantity, quantity };
            ChemicalElement[] elements = new ChemicalElement[] { new ChemicalElement("H", quantity), new ChemicalElement("O", quantity) };
            ChemicalSubstance substance = new ChemicalSubstance(new ChemicalSystem(elements), quantity1);
            return substance;
        }


        /// <summary>
        /// Метод POST, возвращает JSON описание химической формулы, которая содержалась в аргументах.
        /// </summary>
        [HttpPost]
        public IActionResult PostStringAndGetJsonDescriptionChemicalFormula(Object str)
        {
            try
            {
                string htmlString = str.ToString();
                var options = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
                };

                List<object> systems = new List<object>();
                StringBuilder jsonString = new StringBuilder();
                ChemicalSystem system;
                ChemicalSubstance substance;

                htmlString = htmlString.Replace("\r", "");
                htmlString = htmlString.Replace(" ", "");
                string[] formulas = htmlString.Split('\n');

                foreach (string formula in formulas)
                {
                    ChemicalParser.GetChemicalSystemAndChemicalSubstanceByString(out system, out substance, formula);

                    // Химическая система, в отличии от химического вещества, не содержит индексов.
                    if (formula.Contains('x') || formula.Contains('1') || formula.Contains('2') || formula.Contains('3') ||
                        formula.Contains('4') || formula.Contains('5') || formula.Contains('6') || formula.Contains('7') ||
                        formula.Contains('8') || formula.Contains('9'))
                    {
                        systems.Add(substance);
                    }
                    else if (formula.Contains('-'))
                    {
                        systems.Add(system);
                    }
                    else
                    {
                        systems.Add(substance);
                    }
                }
                return Ok(systems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}