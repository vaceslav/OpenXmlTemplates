using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpenXMLTemplates;
using OpenXMLTemplates.Documents;
using OpenXMLTemplates.Engine;
using OpenXMLTemplates.Variables;

namespace OpenXMLTemplatesTest.EngineTest
{
    public class EngineTest
    {
        [Test]
        public void TestEngine()
        {
            using var doc = new TemplateDocument(this.CurrentFolder() + "Doc.docx");

            var json = File.ReadAllText(this.CurrentFolder() + "data.json");

            var src = new VariableSource(json);

            var engine = new DefaultOpenXmlTemplateEngine();

            engine.ReplaceAll(doc, src);


            doc.SaveAs(this.CurrentFolder() + "result.docx");

            var wd = doc.WordprocessingDocument;

            string GetText(string tagName, int elementIndex)
            {
                return wd.FindContentControls(tagName).ElementAtOrDefault(elementIndex).GetTextElement().Text;
            }


            Assert.AreEqual("Antoniantaz", GetText("variable_name", 0));
            Assert.AreEqual("1", GetText("variable_index", 0));
            Assert.AreEqual("2", GetText("variable_index", 1));
            Assert.AreEqual("1", GetText("variable_index", 2));
            Assert.AreEqual("2", GetText("variable_index", 3));

            Assert.AreEqual(4, wd.FindContentControls("repeating_streets").Count());
            Assert.AreEqual(2, wd.FindContentControls("variable_city").Count());
            Assert.AreEqual(9, wd.FindContentControls("variable_name").Count());

            Assert.AreEqual("Antoniantaz", GetText("variable_name", 1));
            Assert.AreEqual("Antoniantaz", GetText("variable_name", 2));

            Assert.AreEqual("Novigrad", GetText("variable_name", 3));
            Assert.AreEqual("First street", GetText("variable_name", 4));
            Assert.AreEqual("Second Street", GetText("variable_name", 5));
            Assert.AreEqual("Khan", GetText("variable_name", 6));

            Assert.AreEqual("Novigrad", GetText("variable_city.name", 0));


            doc.WordprocessingDocument.AssertValid();
        }
        
        [Test]
        public void TestRepeatingControlImagesCollectionReplace()
        {
            const string imageReplacerTag = "image";
            using var doc = new TemplateDocument(this.CurrentFolder() + "Doc2.docx");

            var json = File.ReadAllText(this.CurrentFolder() + "data.json");
            var src = new VariableSource(json);

            // Substitue path for testing 
            var itemsList =((List<object>)src.Data["items"]);
            var image1 = (Dictionary<string, object>)itemsList![0];
            var image2 = (Dictionary<string, object>)itemsList[1];
            var image3 = (Dictionary<string, object>)itemsList[2];
            image1["pic"] = this.CurrentFolder() + "/" + image1["pic"];
            image2["pic"] = this.CurrentFolder() + "/" + image2["pic"];
            image3["pic"] = this.CurrentFolder() + "/" + image3["pic"];

            var engine = new DefaultOpenXmlTemplateEngine();
            engine.ReplaceAll(doc, src);

            doc.SaveAs(this.CurrentFolder() + "result.docx");

            Assert.AreEqual("DocumentFormat.OpenXml.Wordprocessing.SdtRun",
                doc.WordprocessingDocument.FindContentControl(imageReplacerTag + "_" + "pic").GetType()
                    .ToString());
        }
    }
}