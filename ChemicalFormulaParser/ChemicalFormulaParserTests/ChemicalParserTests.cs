using NUnit.Framework;
using ChemicalFormulaParser;
using System;

namespace ChemicalFormulaParserTests
{
    public class ChemicalParserTests
    {

        [Test]
        public void ByHtmlStringGetCorrectDescription()
        {
            ChemicalSystem system;
            ChemicalSubstance substance;
            string htmlFormula = "Ba<sub>x</sub>Sr<sub>1-x</sub>Nb<sub>2</sub>O<sub>6</sub>";
            ChemicalParser.GetChemicalSystemAndChemicalSubstanceByString(out system, out substance, htmlFormula);

            Assert.AreEqual(substance.SubstanceName, "Ba(x)Sr(1-x)Nb(2)O(6)");
            Assert.AreEqual(substance.SystemName, "Ba-Sr-Nb-O");
            Assert.AreEqual(substance.SystemNameNormalized, "O-Sr-Nb-Ba");
            
            // Проверяю элемент "O"
            Assert.AreEqual(substance.Elements[0].Name, "O");
            Assert.AreEqual(substance.Elements[0].AtomicNumber, 8);
            Assert.AreEqual(substance.Elements[0].Quantity.Min, 6);
            Assert.AreEqual(substance.Elements[0].Quantity.Max, 6);
            

            // Проверяю элемент "Sr"
            Assert.AreEqual(substance.Elements[1].Name, "Sr");
            Assert.AreEqual(substance.Elements[1].AtomicNumber, 38);
            Assert.AreEqual(substance.Elements[1].Quantity.Min, 1);
            Assert.AreEqual(substance.Elements[1].Quantity.Max, double.NaN);

            // Проверяю элемент "Nb"
            Assert.AreEqual(substance.Elements[2].Name, "Nb");
            Assert.AreEqual(substance.Elements[2].AtomicNumber, 41);
            Assert.AreEqual(substance.Elements[2].Quantity.Min, 2);
            Assert.AreEqual(substance.Elements[2].Quantity.Max, 2);

            // Проверяю элемент "Ba"
            Assert.AreEqual(substance.Elements[3].Name, "Ba");
            Assert.AreEqual(substance.Elements[3].AtomicNumber, 56);
            Assert.AreEqual(substance.Elements[3].Quantity.Min, double.NaN);
            Assert.AreEqual(substance.Elements[3].Quantity.Max, double.NaN);
        }

        [Test]
        public void ByPlainTextStringGetCorrectDescription()
        {
            ChemicalSystem system;
            ChemicalSubstance substance;
            string plaintTextFormula = "Ba(x)Sr(1-x)Nb(2)O(6)";
            ChemicalParser.GetChemicalSystemAndChemicalSubstanceByString(out system, out substance, plaintTextFormula);

            Assert.AreEqual(substance.SubstanceName, "Ba(x)Sr(1-x)Nb(2)O(6)");
            Assert.AreEqual(substance.SystemName, "Ba-Sr-Nb-O");
            Assert.AreEqual(substance.SystemNameNormalized, "O-Sr-Nb-Ba");

            // Проверяю элемент "O"
            Assert.AreEqual(substance.Elements[0].Name, "O");
            Assert.AreEqual(substance.Elements[0].AtomicNumber, 8);
            Assert.AreEqual(substance.Elements[0].Quantity.Min, 6);
            Assert.AreEqual(substance.Elements[0].Quantity.Max, 6);


            // Проверяю элемент "Sr"
            Assert.AreEqual(substance.Elements[1].Name, "Sr");
            Assert.AreEqual(substance.Elements[1].AtomicNumber, 38);
            Assert.AreEqual(substance.Elements[1].Quantity.Min, 1);
            Assert.AreEqual(substance.Elements[1].Quantity.Max, double.NaN);

            // Проверяю элемент "Nb"
            Assert.AreEqual(substance.Elements[2].Name, "Nb");
            Assert.AreEqual(substance.Elements[2].AtomicNumber, 41);
            Assert.AreEqual(substance.Elements[2].Quantity.Min, 2);
            Assert.AreEqual(substance.Elements[2].Quantity.Max, 2);

            // Проверяю элемент "Ba"
            Assert.AreEqual(substance.Elements[3].Name, "Ba");
            Assert.AreEqual(substance.Elements[3].AtomicNumber, 56);
            Assert.AreEqual(substance.Elements[3].Quantity.Min, double.NaN);
            Assert.AreEqual(substance.Elements[3].Quantity.Max, double.NaN);
        }

        [Test]
        public void IfFoundClosingTagInsteadOpeningTagShouldBeThrownException()
        {
            string formula = "LiNbO</sub>3<sub>";
            Assert.Throws<ArgumentException>(() => ChemicalParser.GetJsonDescriptionByString(formula));
        }
        
        [Test]
        public void IfFoundOpeningTagInsteadClosingTagShouldBeThrownException()
        {
            string formula = "LiNbO<sub>3<sub>";
            Assert.Throws<ArgumentException>(() => ChemicalParser.GetJsonDescriptionByString(formula));
        }
    }
}