using System.IO;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using NUnit.Framework;
using OpenXMLTemplates.Utils;

namespace OpenXMLTemplatesTest.XMLReplacementTest
{
    public class XmlReplacementTests
    {
        private WordprocessingDocument GetDoc()
        {
            return WordFileUtils.OpenFile(this.CurrentFolder() + "XMLReplacementTestDoc.docx");
        }

        [Test]
        public void ReplacesXmlUsingJson()
        {
            using var doc = GetDoc();

            var json = File.ReadAllText(this.CurrentFolder() + "XMLReplacement.json");

            doc.AddOrReplaceCustomXmlPart(json, "XMLReplacementTest");
            doc.AssertValid();

//            doc.SaveAs(TestContext.CurrentContext.TestDirectory + "/XMLReplacementTest/result.docx");
        }

        [Test]
        public void ReplacesXml()
        {
            using var doc = GetDoc();

            var xData =
                XDocument.Load(this.CurrentFolder() + "XMLReplacement.xml");

            doc.AddOrReplaceCustomXmlPart(xData);

//            Can't be tested directly, because word needs to reevaluate the content controls first         
//            Assert.AreEqual("NewItem1Value", doc.FindContentControl("item1").GetTextElement().Text);
//            Assert.AreEqual("NewItem2Value", doc.FindContentControl("item2").GetTextElement().Text);
        }
    }
}