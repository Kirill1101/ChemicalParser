using NUnit.Framework;
using ChemicalFormulaParser;
using System;

namespace ChemicalFormulaParserTests
{
    public class ChemicalElementTests
    {
        [Test]
        public void ElementCreatedWithCorrectAtomicNumber()
        {
            ChemicalElement element = new ChemicalElement("O", new ChemicalQuantity(2));
            Assert.AreEqual(element.AtomicNumber, 8);
        }

        [Test]
        public void ComparisonOfElementsIsCorrect()
        {
            ChemicalElement elementO = new ChemicalElement("O", new ChemicalQuantity(2));
            ChemicalElement elementH = new ChemicalElement("H", new ChemicalQuantity(2));
            Assert.AreEqual(elementH.CompareTo(elementO), -7);
            Assert.AreEqual(elementH.CompareTo(elementH), 0);
            Assert.AreEqual(elementO.CompareTo(elementH), 7);
        }
    }
}