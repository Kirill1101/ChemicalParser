using NUnit.Framework;
using ChemicalFormulaParser;
using System;

namespace ChemicalFormulaParserTests
{
    public class ChemicalSubstanceTests
    {
        [Test]
        public void MethodReturnCorrectSubstanceName()
        {
            ChemicalQuantity quantity = new ChemicalQuantity(2);
            ChemicalQuantity[] quantity1 = new ChemicalQuantity[] { quantity, quantity };
            ChemicalElement[] elements = new ChemicalElement[] { new ChemicalElement("H", quantity), new ChemicalElement("O", quantity) };
            ChemicalSubstance substance = new ChemicalSubstance(new ChemicalSystem(elements), quantity1);
            Assert.AreEqual(substance.SubstanceName, "H(2)O(2)");
        }

        [Test]
        public void ComparisonOfSubstanceIsCorrect()
        {
            ChemicalQuantity quantity1 = new ChemicalQuantity(1);
            ChemicalQuantity[] quantity = new ChemicalQuantity[] { quantity1, quantity1 };
            ChemicalElement[] elements1 = new ChemicalElement[] { new ChemicalElement("O", quantity1), new ChemicalElement("H", quantity1) };
            ChemicalElement[] elements2 = new ChemicalElement[] { new ChemicalElement("Ba", quantity1), new ChemicalElement("Sr", quantity1) };
            ChemicalSubstance substance1 = new ChemicalSubstance(new ChemicalSystem(elements1), quantity);
            ChemicalSubstance substance2 = new ChemicalSubstance(new ChemicalSystem(elements2), quantity);

            Assert.AreEqual(substance1.CompareTo(substance2), -37);
            Assert.AreEqual(substance1.CompareTo(substance1), 0);
            Assert.AreEqual(substance2.CompareTo(substance1), 37);
        }
    }
}