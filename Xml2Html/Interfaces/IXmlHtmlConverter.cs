using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xml2Html.Interfaces
{
    public interface IXmlHtmlConverter
    {
        void ConvertXmlDoc2Html(string input, string ids);
    }
}
