using System;
using System.IO;
using System.Reflection;

namespace ChemicalFormulaParser
{
    /// <summary>
    /// химический элемент
    /// </summary>
    public sealed class ChemicalElement : IComparable<ChemicalElement>, IComparable
    {
        /// <summary>
        /// список всех химических элементов Периодической системы (из ресурса), упорядоченный по возрастанию атомного номера
        /// _elements = {"H", "He", ...};
        /// </summary>
        private readonly static string[] _elements;

        /// <summary>
        /// Статический конструктор (инициализация списка допустимых химичеких элементов)
        /// </summary>
        static ChemicalElement()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(@"ChemicalFormulaParser.elements.txt")))
            {
                string result = sr.ReadToEnd();
                _elements = result.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// обозначение элемента (H, He, ...)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Атомный номер элемента
        /// </summary>
        public byte AtomicNumber { get; }

        /// <summary>
        /// Количественное содержание элемента
        /// </summary>
        public ChemicalQuantity Quantity { get; set; }

        /// <summary>
        /// создание элемента по имени (обозначению)
        /// </summary>
        /// <param name="name">имя элемента, например: "H", "He" и т.п.</param>
        public ChemicalElement(string name, ChemicalQuantity quantity)
        {
            int num = Array.IndexOf(_elements, name); // StringComparison.InvariantCulture
            if (num < 0 || num > 125)
                throw new ApplicationException("Не найден атомный номер по названию элемента: " + name);
            AtomicNumber = (byte)(num + 1);
            Name = name;
            Quantity = quantity;
        }

        /// <summary>
        /// создание элемента по атомному номеру
        /// </summary>
        /// <param name="atomicNumber">Атомный номер элемента</param>
        public ChemicalElement(int atomicNumber, ChemicalQuantity quantity)
        {
            Name = _elements[AtomicNumber = (byte)(atomicNumber - 1)]; // если атомный номер неправильный => IndexOutOfRangeException
            Quantity = quantity;
        }

        public override string ToString()
        {
            return Name;
        }

        #region реализация интерфейсов IComparable<ChemicalElement> и IComparable
        public int CompareTo(ChemicalElement other)
        {
            if (other == null) return 1;
            return AtomicNumber.CompareTo(other.AtomicNumber);
        }

        public int CompareTo(object obj)
        {
            return (this as IComparable<ChemicalElement>).CompareTo(obj as ChemicalElement);
        }
        #endregion
    }

}
