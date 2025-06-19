using System.IO;
using System.Xml;

namespace ArnoldVinkCode
{
    public partial class AVXml
    {
        /// <summary>
        /// Xml text reader that skips namespaces
        /// Note: make sure to remove all namespaces from classes
        /// </summary>
        public class XmlTextReaderSkipNamespace : XmlTextReader
        {
            public XmlTextReaderSkipNamespace(Stream stream) : base(stream)
            {
                Namespaces = true;
            }

            public override string NamespaceURI => string.Empty;
        }
    }
}