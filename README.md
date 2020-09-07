# Xml2HtmlConverter

Showcase of a simple Program using C# and .NET including Unit Testing.  

Takes an xml document as input and selects a subset of elements by the specified tag name, then converts the attribute contents of those elements into
an HTML table and writes the document.  
The entry list can be filtered by specifying a list of comma seperated ids and setting the attribute key of the filter variable.  

*Example Command:*  
Xml2Html.exe -i NoteCollection.xml -k 20000,30000,50000 -t note -a ownerId

## Building Process
For this project Visual Studio 2019 was used and the application can be easily be compiled with that IDE.
Target Framework: .NET Framework 4.7.2

## Options:

- > -i pathToSourceFile  

Required - "The path to the xml file to be converted."
- > -k id1,id2,id3  

Optional - "A list of selected key attribute ids to be extracted as single html files"
- > -t elementTag  

Optional - "The tag of the Elements to be extracted from the document"
- > -t attributeKey  

Optional - "The attribute key to be considered for list subset extraction using the specified ids"

## Defaults:
Default elementTag = "ReleaseNote"  
Default attributeKey = "CustomerId"  

## Effects:
When multiple ids are specified -> a .html document (table) is created for every id associated xml entry list  
No ids are specified -> All entries are written into a single .html document (as table)

## Possible Improvements
- Abstraction of the xml entry extraction process and transformation of the data to an internal format for transmission to html converter.
Through that multiple extraction classes can be made for different document types and structures.
The extractor then could be replaced because they share the same interface for communicating with the rest of the application.
- Abstraction of the html conversion for different document structures (table, list, etc.) or even completely different file formats (.json)
- More Test coverage  
  
Of course such improvements are only necessary if the software has such requirements, otherwise the application may be already sufficient in its current state.  


