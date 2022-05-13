using NUnit.Framework;
using ChemicalFormulaParser;
using System;

namespace ChemicalFormulaParserTests
{
    public class ChemicalQuantityTests
    {
        [Test]
        public void QuantityMethodToStringWorkCorrect()
        {
            ChemicalQuantity quantity = new ChemicalQuantity(1, double.NaN);
            Assert.AreEqual(quantity.ToString(), "1-x");
        }

        [Test]
        public void ComparisonOfQuantityIsCorrect()
        {
            ChemicalQuantity quantity3 = new ChemicalQuantity(3);
            ChemicalQuantity quantity4 = new ChemicalQuantity(4);
            Assert.AreEqual(quantity3.CompareTo(quantity4), -1);
            Assert.AreEqual(quantity3.CompareTo(quantity3), 0);
            Assert.AreEqual(quantity4.CompareTo(quantity3), 1);
        }
    }
}