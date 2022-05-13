using System;
using System.Text.Json.Serialization;

namespace ChemicalFormulaParser
{
    /// <summary>
    /// химическое вещество (раствор, гетерогенная смесь)
    /// </summary>

    public class ChemicalSubstance : ChemicalSystem, IComparable<ChemicalSubstance>, IComparable
    {

        [JsonIgnore]
        /// <summary>
        /// массив количественного содержания химических элементов из унаследованного массива ChemicalElement[] Elements (Внимание! Размеры массивов (и позиции) должны совпадать)
        /// </summary>
        public ChemicalQuantity[] Quantities { get; }

        /// <summary>
        /// формула вещества (это перечисление элементов через тире с указанием количественного состава, например, LiNbO<sub>3</sub>)
        /// </summary>
        public string SubstanceName
        {
            get
            {
                string st = "";
                for (int i = 0; i < Elements.Length; i++)
                    st += Elements[i] + "(" + Quantities[i].ToString() + ")";
                return st;
            }
        }

        [JsonIgnore]
        /// <summary>
        /// формула вещества в HTML
        /// </summary>
        public string SubstanceNameHtml
        {
            get
            {
                string st = "";
                for (int i = 0; i < Elements.Length; i++)
                    st += Elements[i] + Quantities[i].ToHtmlString;
                return st;
            }
        }

        /// <summary>
        /// конструктор вещества
        /// </summary>
        /// <param name="normalize">выполнять ли сортировку</param>
        /// <param name="system">химическая система</param>
        /// <param name="q">количественное описание</param>
        public ChemicalSubstance(bool normalize, ChemicalSystem system, params ChemicalQuantity[] q) : base(false, system.Elements)
        {
            if (system.Elements.Length != q.Length)
                throw new ApplicationException("Несовпадение размеров массивов качественного и количественного описания");
            Quantities = q;
            if (normalize)
            {
                Normalize();
            }
        }

        /// <summary>
        /// конструктор вещества
        /// </summary>
        /// <param name="normalize">выполнять ли сортировку</param>
        /// <param name="system">химическая система</param>
        /// <param name="q">количественное описание</param>
        public ChemicalSubstance(ChemicalSystem system, params ChemicalQuantity[] q) : this(false, system, q)
        { }

        /// <summary>
        /// конструктор копирования вещества
        /// </summary>
        /// <param name="substance">химическое вещество</param>
        public ChemicalSubstance(ChemicalSubstance substance) : base(false, substance.Elements)
        {
            Quantities = substance.Quantities;
            IsNormalized = substance.IsNormalized;
        }

        /// <summary>
        /// нормализует набор элементов и их количественных вхождений
        /// </summary>
        public override void Normalize()
        {
            if (IsNormalized) return;
            Array.Sort(Elements, Quantities);
            IsNormalized = true;
        }

        public override string ToString()
        {
            return SubstanceName;
        }

        #region реализация интерфейсов IComparable<ChemicalSubstance> и IComparable
        public int CompareTo(ChemicalSubstance other)
        {
            if (other == null) return 1;
            int retVal = Elements.Length.CompareTo(other.Elements.Length);
            if (retVal != 0)
                return retVal;
            var _thisEl = Elements.Clone() as ChemicalElement[];
            var _thisQ = Quantities.Clone() as ChemicalQuantity[];
            var _otherEl = other.Elements.Clone() as ChemicalElement[];
            var _otherQ = other.Quantities.Clone() as ChemicalQuantity[];
            Array.Sort(_thisEl, _thisQ);
            Array.Sort(_otherEl, _otherQ);
            for (int i = 0; i < _thisEl.Length; i++)
            {
                retVal = _thisEl[i].CompareTo(_otherEl[i]);
                if (retVal != 0)
                    return retVal;
                retVal = _thisQ[i].CompareTo(_otherQ[i]);
                if (retVal != 0)
                    return retVal;
            }
            return 0;
        }
        public new int CompareTo(object obj)
        {
            return ((IComparable<ChemicalSubstance>)this).CompareTo(obj as ChemicalSubstance);
        }
        #endregion
    }
}
