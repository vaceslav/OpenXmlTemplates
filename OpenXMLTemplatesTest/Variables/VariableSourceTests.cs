using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using OpenXMLTemplates.Variables;
using OpenXMLTemplates.Variables.Exceptions;

namespace OpenXMLTemplatesTest
{
    public class DataTests
    {
        [Test]
        public void GetVariableByIdentifierWorksCorrectly()
        {
            var addressDict = new Dictionary<string, string>
                { { "street", "MyStreet" }, { "number", "1" }, { "app", "2" } };


            var data = new Dictionary<string, object>
            {
                { "name", "MyName" }, { "address", addressDict }, { "phones", new List<string> { "123", "12345" } }
            };

            var source = new VariableSource(data);

            Assert.AreEqual("MyName", source.GetVariable<string>("name"));
            Assert.AreEqual("MyStreet", source.GetVariable<string>("address.street"));

            Assert.AreEqual("12345", source.GetVariable<string>("phones.[1]"));

            Assert.Throws<VariableNotFoundException>(() => source.GetVariable<string>("name.street"));
            Assert.Throws<VariableNotFoundException>(() => source.GetVariable<string>("address.streeets"));
            Assert.Throws<IncorrectVariableTypeException>(() => source.GetVariable<int>("name"));
        }

        [Test]
        public void Format_Numeric_Fields_Value_Null()
        {
            var data = new Dictionary<string, object>
            {
                { "prices", null }
            };

            var source = new VariableSource(data);

            Assert.AreEqual("", source.GetVariable<string>("prices(N2)"));
        }

        [Test]
        public void Format_Numeric_Fields()
        {
            // Set the current culture to invariant culture for consistent numeric formatting across different environments. 
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            var data = new Dictionary<string, object>
            {
                { "prices", new List<string> { "123", "12345.0001" } }
            };

            var source = new VariableSource(data);

            Assert.AreEqual("12,345.00", source.GetVariable<string>("prices.[1](N2)"));
        }
    }
}