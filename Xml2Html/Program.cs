using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Xml;
using System.Runtime.CompilerServices;
using Xml2Html.Service;
using Xml2Html.Interfaces;

namespace Xml2Html
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            /*args = new string[] { 
                "-i", "Sprint_11945.xml",
                "-k", "900000,060005,020000"
            };*/

            //Parsing Command Line Arguments
            RootCommand rootCommand = new RootCommand(
                description: "Convert xml content to html schema");
            Option inputOption = new Option(
                aliases: new string[] { "--input", "-i" }
                , description: "The path to the xml file to be converted.");
            inputOption.IsRequired = true;
            inputOption.Argument = new Argument<string>();
            rootCommand.AddOption(inputOption);
            Option idsOption = new Option(
                aliases: new string[] { "--ids", "-k" }
                , description: "A list of selected key attribute ids to be extracted as single html files");
            idsOption.Argument = new Argument<string>();
            idsOption.IsRequired = false;
            rootCommand.AddOption(idsOption);

            Option entryTagOption = new Option(
                aliases: new string[] { "--entrytag", "-t" }
                , description: "The tag of the Elements to be extracted from the document");
            entryTagOption.Argument = new Argument<string>();
            entryTagOption.IsRequired = false;
            rootCommand.AddOption(entryTagOption);
            Option subSetAttKey = new Option(
                aliases: new string[] { "--subattkey", "-a" }
                , description: "The attribute key to be considered for list subset extraction using the specified ids");
            subSetAttKey.Argument = new Argument<string>();
            subSetAttKey.IsRequired = false;
            rootCommand.AddOption(subSetAttKey);

            //IXmlHtmlConverter transformer = new Xml2HtmlTransformer();
            //IXmlHtmlConverter transformer = new Xml2HtmlTransformer("note", "ownerId");

            //Create Action for conversion and add console commands
            Action<string, string, string, string> performConversion = (string input, string ids, string entryTag, string subAttKey) =>
            {
                Console.WriteLine(input);
                Console.WriteLine(ids);

                IXmlHtmlConverter transformer = new Xml2HtmlTransformer(entryTag, subAttKey);

                transformer.ConvertXmlDoc2Html(input, ids);

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            };

            rootCommand.Handler = CommandHandler.Create<string, string, string, string>(performConversion);
            
            //Pass args to commandline parser
            return await rootCommand.InvokeAsync(args);
        }
    }
}
