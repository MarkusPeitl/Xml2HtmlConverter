using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Xml2Html.Interfaces;

namespace Xml2Html.Service
{
    public class Xml2HtmlTransformer: IXmlHtmlConverter
    {
        private const string validatePattern = @"^[0-9,]+$";
        private Regex validateRegex = null;

        private string entryElemTag = "ReleaseNote";
        private string subSetAttributeKey = "CustomerId";

        private const string baseNameSpace = "ns";
        private const string htmlExtension = ".html";
        public Xml2HtmlTransformer()
        {
            this.validateRegex = new Regex(validatePattern);
        }
        public Xml2HtmlTransformer(string entryElemTag, string subSetAttributeKey)
        {
            this.validateRegex = new Regex(validatePattern);
            if (entryElemTag != null)
            {
                this.entryElemTag = entryElemTag;
            }
            if (subSetAttributeKey != null)
            {
                this.subSetAttributeKey = subSetAttributeKey;
            }
        }
        
        //Read the specified xml document, convert to .html (filtering for passed ids) files with tables and write to disk
        public void ConvertXmlDoc2Html(string input, string ids)
        {
            //Absolute path
            string fullAbsInputPath = input;

            //Relative Path
            if (!Path.IsPathRooted(input))
            {
                fullAbsInputPath = Path.Combine(Directory.GetCurrentDirectory(), input);
            }

            string[] customerIdList = ParseCustomerIds(ids);

            if (File.Exists(fullAbsInputPath))
            {
                string inFileName = Path.GetFileNameWithoutExtension(input);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fullAbsInputPath);
                XmlNamespaceManager nameSpaceManager = new XmlNamespaceManager(xmlDoc.NameTable);

                ParseDocNameSpaces(xmlDoc, baseNameSpace, nameSpaceManager);

                XmlNodeList allReleaseNotes = xmlDoc.SelectNodes("//" + baseNameSpace + ":" + entryElemTag, nameSpaceManager);
                List<string> attKeys = GetUniqueAttKeysOfNodeList(allReleaseNotes);

                if (customerIdList == null)
                {
                    WriteNodeListAsHtmlDoc(allReleaseNotes, attKeys, inFileName + htmlExtension);
                }
                else
                {
                    foreach (string custormerId in customerIdList)
                    {
                        XmlNodeList customerNodeList = xmlDoc.SelectNodes("//" + baseNameSpace + ":" + entryElemTag + 
                            "[@" + subSetAttributeKey + "=" + custormerId + "]", nameSpaceManager);

                        WriteNodeListAsHtmlDoc(customerNodeList, attKeys, inFileName + "_" + custormerId + htmlExtension);
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("The file " + fullAbsInputPath + " could not be found");
            }
        }

        //Parse comma seperated string containing number ids to string array
        public string[] ParseCustomerIds(string idsString)
        {
            string[] customerIdList = null;
            if (idsString != null && idsString != "")
            {
                if (validateRegex.IsMatch(idsString))
                {
                    customerIdList = idsString.Split(',');
                    if (customerIdList == null)
                    {
                        return new string[] { idsString };
                    }
                    return customerIdList;
                }
                else
                {
                    throw new FormatException("Invalid string format for the id List");
                }
            }
            return null;
        }

        //Parse namespaces from document and add them to the passed NamespaceManager
        public void ParseDocNameSpaces(XmlDocument xmlDoc, string baseNameSpace, XmlNamespaceManager targetNsMgr)
        {
            foreach (XmlAttribute xmlAtt in xmlDoc.DocumentElement.Attributes)
            {
                if (xmlAtt.Name != null && xmlAtt.Name.Contains("xmlns"))
                {
                    if (xmlAtt.Name.Contains(":"))
                    {
                        var attParts = xmlAtt.Name.Split(':');
                        targetNsMgr.AddNamespace(attParts[1], xmlAtt.Value);
                    }
                    else if (xmlAtt.Name.Equals("xmlns"))
                    {
                        targetNsMgr.AddNamespace(baseNameSpace, xmlAtt.Value);
                    }
                }
            }
        }

        //Go through all entry nodes in the passed list and extract unique attribute keys
        public List<string> GetUniqueAttKeysOfNodeList(XmlNodeList entriesList)
        {
            List<string> attKeys = new List<string>();
            Dictionary<string, bool> attExists = new Dictionary<string, bool>();
            foreach (XmlNode node in entriesList)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (!attExists.ContainsKey(attribute.Name))
                    {
                        attExists[attribute.Name] = true;
                        attKeys.Add(attribute.Name);
                    }
                }
            }

            return attKeys;
        }

        //Transform NodeList to an äquivalent representation in .html and write to outFilePath
        public void WriteNodeListAsHtmlDoc(XmlNodeList nodeList, List<string> attributeNames, string outFilePath)
        {
            var htmlDoc = new XmlDocument();
            var htmlRoot = htmlDoc.CreateElement("html");
            htmlDoc.AppendChild(htmlRoot);
            var body = htmlDoc.CreateElement("body");
            htmlRoot.AppendChild(body);

            var table = NodeListToHtmlTable(nodeList, attributeNames, htmlDoc);
            if (table != null)
            {
                body.AppendChild(table);

                Console.WriteLine("Writing HTML file to: " + outFilePath);
                File.WriteAllText(outFilePath, htmlDoc.OuterXml);
            }
        }

        //Iterate over xml entries and insert attributes as column values into a html table
        public XmlNode NodeListToHtmlTable(XmlNodeList entriesList, List<string> columnNames, XmlDocument creatorDoc)
        {
            var table = creatorDoc.CreateElement("table");
            if (entriesList.Count > 0)
            {
                //Create Name Column
                var colNamesRow = creatorDoc.CreateElement("tr");
                table.AppendChild(colNamesRow);
                foreach (string columnName in columnNames)
                {
                    var column = creatorDoc.CreateElement("th");
                    column.InnerText = columnName;
                    colNamesRow.AppendChild(column);
                }

                //Populate table data
                foreach (XmlNode entry in entriesList)
                {
                    var row = creatorDoc.CreateElement("tr");
                    table.AppendChild(row);

                    foreach (string columnName in columnNames)
                    {
                        var columnEntry = creatorDoc.CreateElement("td");

                        var selectedAttribute = entry.Attributes.GetNamedItem(columnName);
                        if (selectedAttribute != null)
                        {
                            columnEntry.InnerText = selectedAttribute.InnerText;
                        }

                        row.AppendChild(columnEntry);
                    }
                }

                return table;
            }

            return null;
        }
    }
}
