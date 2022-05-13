using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalFormulaParser
{
    public class ChemicalParser
    {
        /// <summary>
        /// Метод получения JSON описания химической формулы по строке.
        /// </summary>
        public static string GetJsonDescriptionByString(string str)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };

            List<object> systems = new List<object>();
            StringBuilder jsonString = new StringBuilder();
            ChemicalSystem system;
            ChemicalSubstance substance;

            str = str.Replace("\r", "");
            str = str.Replace(" ", "");
            string[] formulas = str.Split('\n');

            foreach (string formula in formulas)
            {
                GetChemicalSystemAndChemicalSubstanceByString(out system, out substance, formula);

                if (formula.Contains('x') || formula.Contains('1') || formula.Contains('2') || formula.Contains('3') ||
                    formula.Contains('4') || formula.Contains('5') || formula.Contains('6') || formula.Contains('7') ||
                    formula.Contains('8') || formula.Contains('9'))
                {
                    systems.Add(substance);
                }
                else
                {
                    systems.Add(system);
                }
            }
            return JsonSerializer.Serialize(systems, options).Replace("NaN", "x");
        }

        /// <summary>
        /// Метод получения объектов ChemicalSystem и ChemicalSubstance по строке.
        /// </summary>
        public static void GetChemicalSystemAndChemicalSubstanceByString(out ChemicalSystem system, out ChemicalSubstance substance, string str)
        {
            int index = 0;
            Dictionary<string, ChemicalQuantity> elements;
            if (str.IndexOf("sub") != -1)
            {
                elements = GetInfoAboutElementsByHtml(str);
            }
            else
            {
                elements = GetInfoAboutElementsByPlainText(str);
            }
            ChemicalQuantity[] quantities = new ChemicalQuantity[elements.Count];
            ChemicalElement[] chemicalElements = new ChemicalElement[elements.Count];
            foreach (var element in elements)
            {
                chemicalElements[index] = new ChemicalElement(element.Key, element.Value);
                quantities[index] = element.Value;
                index++;
            }
            system = new ChemicalSystem(chemicalElements);
            substance = new ChemicalSubstance(system, quantities);
        }

        /// <summary>
        /// Метод получения информации о химической формуле по строке Plain text в виде словаря <string, ChemicalQuantity>,
        /// в котором содердится информация о элементе и его количественном вхождении в формулу.
        /// </summary>
        private static Dictionary<string, ChemicalQuantity> GetInfoAboutElementsByPlainText(string str)
        {
            Dictionary<string, ChemicalQuantity> elements = new Dictionary<string, ChemicalQuantity>();
            double minIndex, maxIndex, num;
            string Element = "";
            string NestedFormula = "";
            minIndex = 1;
            maxIndex = 1;
            int i = 0;
            int i_start = 0, i_end = 0, flag = 0;
            while (i < str.Length)
            {
                // ищем индекс
                if (double.TryParse(str[i].ToString(), out num) || (str[i] == 'x'))
                {
                    i_start = i;
                    flag = 0;
                    while (i < str.Length)
                    {
                        if (str[i] >= 'A' && str[i] <= 'Z')
                        {
                            flag = 1;
                            break;
                        }
                        else
                        {
                            i += 1;
                        }
                    }

                    string index = str.Substring(i_start, i - i_start);
                    ParseIndexes(ref minIndex, ref maxIndex, ref index);
                    continue;
                }
                else if (char.IsUpper(str, i))
                {
                    i_start = i;
                    if (i < str.Length)
                        i += 1;
                    while (i < str.Length)
                    {
                        if ((char.IsLower(str, i)) && (str[i] != 'x'))
                            i += 1;
                        else
                            break;
                    }
                    if (Element != "" || NestedFormula != "")
                    {
                        AddElementToDictionary(elements, ref Element, ref NestedFormula, ref minIndex, ref maxIndex, false);
                    }
                    // встали на символ следующий после окончания элемента
                    Element = str.Substring(i_start, i - i_start);
                    continue;
                }
                else if (str[i] == '(')
                {
                    i_start = i; // встали на (
                    flag = 0;
                    if (i < str.Length)
                        i += 1; // уйдем с символа (
                    while (i < str.Length)   // ищем )
                    {
                        if (str[i] == ')')
                        {
                            if (flag == 0)
                            {
                                i_end = i;
                                flag = -1;
                                break;
                            }
                            else
                                flag -= 1;
                        }
                        else if (str[i] == '(')
                            flag += 1;
                        if (i < str.Length)
                            i += 1;
                    }
                    if (flag == -1)
                    {
                        if (double.TryParse(str[i_start + 1].ToString(), out num) || (str[i_start + 1] == 'x'))
                        {
                            string index = str.Substring(i_start + 1, i_end - i_start - 1);
                            ParseIndexes(ref minIndex, ref maxIndex, ref index);
                        }
                        else
                        {
                            if (Element != "" || NestedFormula != "")
                            {
                                AddElementToDictionary(elements, ref Element, ref NestedFormula, ref minIndex, ref maxIndex, false);
                            }
                            if (str.Substring(i_start + 1, i_end - i_start - 1).Length > 2)
                            {
                                NestedFormula = str.Substring(i_start + 1, i_end - i_start - 1);
                            }
                            else
                            {
                                Element = str.Substring(i_start + 1, i_end - i_start - 1);
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Не найден символ ')' парный к '('");
                    }
                    continue;
                }
                if (i < str.Length)
                    i += 1;
            }

            if (Element != "" || NestedFormula != "")
            {
                AddElementToDictionary(elements, ref Element, ref NestedFormula, ref minIndex, ref maxIndex, false);
            }
            return elements;
        }

        /// <summary>
        /// Метод получения информации о химической формуле по строке HTML в виде словаря <string, ChemicalQuantity>,
        /// в котором содердится информация о элементе и его количественном вхождении в формулу.
        /// </summary>
        private static Dictionary<string, ChemicalQuantity> GetInfoAboutElementsByHtml(string str)
        {
            Dictionary<string, ChemicalQuantity> elements = new Dictionary<string, ChemicalQuantity>();
            double minIndex, maxIndex;
            string Element = "";
            string NestedFormula = "";
            minIndex = 1;
            maxIndex = 1;
            int i = 0;
            int i_start = 0, i_end = 0, flag = 0;
            while (i < str.Length)
            {
                // ищем индекс (он должен быть в тегах <sub>...</sub>)
                if (str[i] == '<')
                {
                    i_start = i;
                    flag = 0;
                    while (i < str.Length)
                    {
                        if (str[i] != '>')
                            i += 1;
                        else
                        {
                            flag = 1;
                            break;
                        }
                    }
                    if (flag == 1)
                    {
                        if (str.IndexOf('/', i_start, i - i_start) > -1)
                        {
                            throw new ArgumentException("Встречен закрывающий тег " + str.Substring(i_start, i - i_start + 1) + ", тогда как ожидался открывающий");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Встречен признак открытия тега (<), которому не соответствует знак < (" + str.Substring(i_start, i - i_start + 1) + ")");
                    }
                    if (i < str.Length)
                        i += 1;
                    i_start = i;     // указывает на начало индекса
                    flag = 0;
                    while (i < str.Length)
                    {
                        if (str[i] != '<')
                            i += 1;
                        else
                        {
                            flag = 1;
                            break;
                        }
                    }
                    if (flag == 1)
                    {
                        i_end = i;   // указывает на "<" 
                        flag = 0;
                        while (i < str.Length)
                        {
                            if (str[i] != '>')
                                i += 1;
                            else
                            {
                                flag = 1;
                                break;
                            }
                        }
                        if (flag == 1)
                        {
                            if (str.IndexOf('/', i_start, i - i_start) == -1)
                            {
                                throw new ArgumentException("Встречен открывающий тег " + str.Substring(i_start, i - i_start) + ", тогда как ожидался закрывающий");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Встречен признак открытия тега '<', которому не соответствует признак закрытия тега '>' (" + str.Substring(i_start, i - i_start) + ")");
                        }
                        string index = str.Substring(i_start, i_end - i_start);
                        flag = ParseIndexes(ref minIndex, ref maxIndex, ref index);
                    }
                    else
                    {
                        throw new ArgumentException("Не обнаружен закрывающий тег");
                    }
                    if (i < str.Length)
                        i += 1; // уйдем с символа >
                    continue;
                }
                else if (char.IsUpper(str, i))
                {
                    i_start = i;
                    if (i < str.Length)
                        i += 1;
                    while (i < str.Length)
                    {
                        if (char.IsLower(str, i))
                            i += 1;
                        else
                            break;
                    }
                    if (Element != "" || NestedFormula != "")
                    {
                        AddElementToDictionary(elements, ref Element, ref NestedFormula, ref minIndex, ref maxIndex, true);
                    }
                    // встали на символ следующий после окончания элемента
                    Element = str.Substring(i_start, i - i_start);
                    continue;
                }
                else if (str[i] == '(')
                {
                    i_start = i; // встали на (
                    flag = 0;
                    if (i < str.Length)
                        i += 1; // уйдем с символа (
                    while (i < str.Length)   // ищем )
                    {
                        if (str[i] == ')')
                        {
                            if (flag == 0)
                            {
                                i_end = i;
                                flag = -1;
                                break;
                            }
                            else
                                flag -= 1;
                        }
                        else if (str[i] == '(')
                            flag += 1;
                        if (i < str.Length)
                            i += 1;
                    }
                    if (flag == -1)
                    {
                        if (Element != "" || NestedFormula != "")
                        {
                            AddElementToDictionary(elements, ref Element, ref NestedFormula, ref minIndex, ref maxIndex, true);
                        }
                        NestedFormula = str.Substring(i_start + 1, i_end - i_start - 1);
                    }
                    else
                    {
                        throw new ArgumentException("Не найден символ ')' парный к '('");
                    }
                    continue;
                }
                if (i < str.Length)
                    i += 1;
            }

            if (Element != "" || NestedFormula != "")
            {
                AddElementToDictionary(elements, ref Element, ref NestedFormula, ref minIndex, ref maxIndex, true);
            }
            return elements;
        }

        /// <summary>
        /// Метод добавления элемента(вложенной формулы) в словарь.
        /// </summary>
        private static void AddElementToDictionary(Dictionary<string, ChemicalQuantity> elements, ref string Element, ref string NestedFormula,
            ref double minIndex, ref double maxIndex, bool strIsHtml)
        {
            if (Element != "")
            {
                if (elements.ContainsKey(Element))
                {
                    elements[Element].Min += minIndex;
                    elements[Element].Max += maxIndex;
                }
                else
                {
                    elements.Add(Element, new ChemicalQuantity(minIndex, maxIndex));
                }
                Element = "";
            }
            else if (NestedFormula != "")
            {
                Dictionary<string, ChemicalQuantity> nestedElements;
                if (strIsHtml)
                {
                    nestedElements = GetInfoAboutElementsByHtml(NestedFormula);
                }
                else
                {
                    nestedElements = GetInfoAboutElementsByPlainText(NestedFormula);
                }
                foreach (var element in nestedElements)
                {
                    if (elements.ContainsKey(element.Key))
                    {
                        elements[element.Key].Min += element.Value.Min * minIndex;
                        elements[element.Key].Max += element.Value.Max * minIndex;
                    }
                    else
                    {
                        elements.Add(element.Key, new ChemicalQuantity(minIndex * element.Value.Min, maxIndex * element.Value.Max));
                    }
                }
                NestedFormula = "";
            }
            minIndex = 1;
            maxIndex = 1;
        }

        /// <summary>
        /// Метод парсинга индексов. Проеряет строку index на корректность и записывает
        /// минимальное и максимальное количественное содержания элемента в веществе.
        /// </summary>
        private static int ParseIndexes(ref double minIndex, ref double maxIndex, ref string index)
        {
            int retVal = 0;
            int i;
            bool result;

            minIndex = 0;
            maxIndex = 0;
            i = index.IndexOf('-');
            if (i > -1)
            {
                // Разберемся с минимальным индексом.
                result = double.TryParse(index.Substring(0, i), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out minIndex);
                if (!result)
                    minIndex = double.NaN;
                // Разберемся с максимальным индексом.
                result = double.TryParse(index.Substring(i + 1, index.Length - i - 1), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out maxIndex);
                if (!result)
                    maxIndex = double.NaN;
            }
            else
            {
                result = double.TryParse(index, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out minIndex);
                if (!result)
                    minIndex = double.NaN;
                maxIndex = minIndex;
            }

            return retVal;
        }
    }
}