using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Xml2Html.Service;

namespace Xml2HtmlTesting
{
    public class Tests
    {
        private Xml2HtmlTransformer transformer = null;

        [SetUp]
        public void Setup()
        {
            this.transformer = new Xml2HtmlTransformer();
        }
        
        //Testing failure when the file path passed is invalid
        [Test]
        public void InvalidXmlFileTest()
        {
            Assert.Throws(typeof(FileNotFoundException), new TestDelegate(
                delegate { transformer.ConvertXmlDoc2Html("invalid.xml", null); }
            ));
        }

        //Testing failure when the ids passed do not have the correct string formatting and whitelisted characters
        [Test]
        public void MalformedIdParsingTest()
        {
            Assert.Throws(typeof(FormatException), new TestDelegate(
                delegate { transformer.ParseCustomerIds("asfuhou,98765"); }
            ));
        }

        //Testing success of id parsing results
        [Test]
        public void IdParsingTest()
        {
            string[] expectedResult = new string[] { "900000", "060005", "060005" };
            string cmdString = String.Join(',', expectedResult);
            string[] result = transformer.ParseCustomerIds(cmdString);

            Assert.AreEqual(expectedResult, result);

            string idstring = "900000";
            string[] expectedResultSingle = new string[] { idstring };
            string[] resultSingle = transformer.ParseCustomerIds(idstring);

            Assert.AreEqual(expectedResultSingle, resultSingle);
        }

        //Testing success of namespace parsing from document to namespacemanager
        [Test]
        public void ParseDocNamespacesTest()
        {
            string baseNameSpace = "ns";
            XmlDocument xmlDoc = new XmlDocument();
            var rootElem = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(rootElem);

            rootElem.SetAttribute("xmlns", "test1");
            rootElem.SetAttribute("xmlns:first", "test2");
            rootElem.SetAttribute("xmlns:second", "test3");

            XmlNamespaceManager targetManager = new XmlNamespaceManager(xmlDoc.NameTable);
            
            transformer.ParseDocNameSpaces(xmlDoc, baseNameSpace, targetManager);

            Assert.AreEqual(targetManager.LookupNamespace(baseNameSpace), "test1");
            Assert.AreEqual(targetManager.LookupNamespace("first"), "test2");
            Assert.AreEqual(targetManager.LookupNamespace("second"), "test3");
        }

        //Testing  success of unique attribute key extraction from NodeList
        [Test]
        public void GetUniqueAttKeysOfNodeListTest()
        {
            XmlDocument xmlDoc = new XmlDocument();

            var rootElem = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(rootElem);

            var entry1 = xmlDoc.CreateElement("entry");
            var entry2 = xmlDoc.CreateElement("entry");
            var entry3 = xmlDoc.CreateElement("entry");
            rootElem.AppendChild(entry1);
            rootElem.AppendChild(entry2);
            rootElem.AppendChild(entry3);

            entry1.SetAttribute("first","value");
            entry1.SetAttribute("second", "value");
            entry1.SetAttribute("third", "value");

            entry2.SetAttribute("first", "value");
            entry2.SetAttribute("second", "value");
            entry2.SetAttribute("third", "value");
            entry2.SetAttribute("fourth", "value");
            entry2.SetAttribute("fifth", "value");

            entry3.SetAttribute("first", "value");
            entry3.SetAttribute("sixth", "value");

            string[] expectedAsArray = new string[] { "first", "second", "third", "fourth", "fifth", "sixth" };

            List<string> attKeys = transformer.GetUniqueAttKeysOfNodeList(rootElem.ChildNodes);

            Assert.AreEqual(expectedAsArray, attKeys.ToArray());
        }
    }
}