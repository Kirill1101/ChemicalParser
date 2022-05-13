using NUnit.Framework;
using ChemicalFormulaParser;
using System;

namespace ChemicalFormulaParserTests
{
    public class ChemicalSystemTests
    {
        [Test]
        public void MethodReturnCorrectSystemName()
        {
            ChemicalQuantity quantity = new ChemicalQuantity(2);
            ChemicalElement[] elements = new ChemicalElement[] { new ChemicalElement("O", quantity), new ChemicalElement("H", quantity) };
            ChemicalSystem system = new ChemicalSystem(elements);
            Assert.AreEqual(system.SystemName, "O-H");
            Assert.AreEqual(system.SystemNameNormalized, "H-O");
        }

        [Test]
        public void ComparisonOfSubstanceIsCorrect()
        {
            ChemicalQuantity quantity1 = new ChemicalQuantity(1);
            ChemicalElement[] elements1 = new ChemicalElement[] { new ChemicalElement("O", quantity1), new ChemicalElement("H", quantity1) };
            ChemicalElement[] elements2 = new ChemicalElement[] { new ChemicalElement("Ba", quantity1), new ChemicalElement("Sr", quantity1) };
            ChemicalSystem system1 = new ChemicalSystem(elements1);
            ChemicalSystem system2 = new ChemicalSystem(elements2);

            Assert.AreEqual(system1.CompareTo(system2), -37);
            Assert.AreEqual(system1.CompareTo(system1), 0);
            Assert.AreEqual(system2.CompareTo(system1), 37);
        }
    }
}