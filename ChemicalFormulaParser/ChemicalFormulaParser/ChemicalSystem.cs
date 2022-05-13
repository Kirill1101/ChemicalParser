using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace ChemicalFormulaParser
{
    /// <summary>
    /// химическая система
    /// </summary>

    public class ChemicalSystem : IComparable<ChemicalSystem>, IComparable
    {

        /// <summary>
        /// название системы (это перечисление хим. элементов через тире, например, As-Ga)
        /// </summary>
        public string SystemName
        {
            get { return string.Join("-", Array.ConvertAll(Elements, x => x.Name)); }
        }

        /// <summary>
        /// название системы (это перечисление хим. элементов через тире, например, As-Ga)
        /// </summary>
        public string SystemNameNormalized
        {
            get
            {
                Normalize();
                return SystemName;
            }
        }

        /// <summary>
        /// массив химических элементов
        /// </summary>
        public ChemicalElement[] Elements { get; }


        [JsonIgnore]
        /// <summary>
        /// признак того, что список элементов нормализован (отсортирован)
        /// </summary>
        public bool IsNormalized { get; protected set; } = false;

        /// <summary>
        /// конструктор (проверяется уникальность элементов в массиве)
        /// </summary>
        /// <param name="normalize">выполнять ли сортировку</param>
        /// <param name="el">массив химических элементов</param>
        public ChemicalSystem(bool normalize, params ChemicalElement[] el)
        {
            //if (el.Length != el.Distinct().Count())
            //throw new ApplicationException("ChemicalSystem: найдены повторяющиеся элементы");
            Elements = el;
            if (normalize)
            {
                Normalize();
            }
        }

        /// <summary>
        /// конструктор (проверяется уникальность элементов в массиве)
        /// </summary>
        /// <param name="el">массив химических элементов</param>
        public ChemicalSystem(params ChemicalElement[] el)
            : this(false, el) { }

        /// <summary>
        /// нормализует набор элементов (упорядочивает по атомному номеру)
        /// </summary>
        public virtual void Normalize()
        {
            if (IsNormalized) return;
            Array.Sort(Elements); // generic call: Array.Sort<ChemicalElement>(Elements);
            IsNormalized = true;
        }

        public override string ToString() { return SystemName; }

        #region реализация интерфейсов IComparable<ChemicalSystem> и IComparable

        public int CompareTo(ChemicalSystem other)
        {
            if (other == null) return 1;
            int retVal = Elements.Length.CompareTo(other.Elements.Length);
            if (retVal != 0)
                return retVal;
            var _this = Elements.Clone() as ChemicalElement[];
            var _other = other.Elements.Clone() as ChemicalElement[];
            Array.Sort<ChemicalElement>(_this);
            Array.Sort<ChemicalElement>(_other);
            for (int i = 0; i < _this.Length; i++)
            {
                retVal = _this[i].CompareTo(_other[i]);
                if (retVal != 0)
                    return retVal;
            }
            return 0;
        }
        public int CompareTo(object obj)
        {
            return ((IComparable<ChemicalSystem>)this).CompareTo(obj as ChemicalSystem);
        }
        #endregion
    }
}
