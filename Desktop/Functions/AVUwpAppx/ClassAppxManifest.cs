using System.Collections.Generic;
using System.Xml.Serialization;

namespace ArnoldVinkCode
{
    public partial class AVUwpAppx
    {
        private const string nameSpace_Windows = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";
        private const string nameSpace_Desktop = "http://schemas.microsoft.com/appx/manifest/desktop/windows10";
        private const string nameSpace_UAP = "http://schemas.microsoft.com/appx/manifest/uap/windows10";
        private const string nameSpace_UAP3 = "http://schemas.microsoft.com/appx/manifest/uap/windows10/3";

        [XmlRoot(ElementName = "Package", Namespace = nameSpace_Windows)]
        public class AppxManifest
        {
            public Identity Identity { get; set; } = new Identity();
            public Properties Properties { get; set; } = new Properties();
            public Dependencies Dependencies { get; set; } = new Dependencies();
            public Applications Applications { get; set; } = new Applications();
        }

        public class Identity
        {
            [XmlAttribute(AttributeName = "Name")]
            public string FamilyName { get; set; }
            [XmlAttribute(AttributeName = "Version")]
            public string Version { get; set; }
            [XmlAttribute(AttributeName = "ProcessorArchitecture")]
            public string ProcessorArchitecture { get; set; }
            [XmlAttribute(AttributeName = "Publisher")]
            public string Publisher { get; set; }
        }

        public class Properties
        {
            public string DisplayName { get; set; }
            public string PublisherDisplayName { get; set; }
            public string Logo { get; set; }
        }

        public class Dependencies
        {
            [XmlElement(ElementName = "TargetDeviceFamily")]
            public TargetDeviceFamily TargetDeviceFamily { get; set; }
        }

        public class TargetDeviceFamily
        {
            [XmlAttribute(AttributeName = "Name")]
            public string Name { get; set; }
        }

        public class Applications
        {
            [XmlElement(ElementName = "Application")]
            public List<Application> Application { get; set; }
        }

        public class Application
        {
            [XmlAttribute(AttributeName = "Id")]
            public string Id { get; set; }
            [XmlAttribute(AttributeName = "Executable")]
            public string Executable { get; set; }
            [XmlAttribute(AttributeName = "EntryPoint")]
            public string EntryPoint { get; set; }
            public Extensions Extensions { get; set; } = new Extensions();
            [XmlElement(Namespace = nameSpace_UAP)]
            public VisualElements VisualElements { get; set; } = new VisualElements();
        }

        public class Extensions
        {
            [XmlElement(ElementName = "Extension", Namespace = nameSpace_UAP3)]
            public List<ExtensionUAP3> ExtensionUAP3 { get; set; }
        }

        public class ExtensionUAP3
        {
            [XmlAttribute(AttributeName = "Category")]
            public string Category { get; set; }
            [XmlAttribute(AttributeName = "EntryPoint")]
            public string EntryPoint { get; set; }
            public AppExecutionAlias AppExecutionAlias { get; set; } = new AppExecutionAlias();
        }

        public class ExecutionAlias
        {
            [XmlAttribute(AttributeName = "Alias")]
            public string Alias { get; set; }
        }

        public class AppExecutionAlias
        {
            [XmlElement(Namespace = nameSpace_Desktop)]
            public ExecutionAlias ExecutionAlias { get; set; } = new ExecutionAlias();
        }

        public class DefaultTile
        {
            [XmlAttribute(AttributeName = "Square44x44Logo")]
            public string Square44x44Logo { get; set; }
            [XmlAttribute(AttributeName = "Square71x71Logo")]
            public string Square71x71Logo { get; set; }
            [XmlAttribute(AttributeName = "Square150x150Logo")]
            public string Square150x150Logo { get; set; }
            [XmlAttribute(AttributeName = "Square310x310Logo")]
            public string Square310x310Logo { get; set; }
            [XmlAttribute(AttributeName = "Wide310x150Logo")]
            public string Wide310x150Logo { get; set; }
        }

        public class VisualElements
        {
            public DefaultTile DefaultTile { get; set; } = new DefaultTile();
            [XmlAttribute(AttributeName = "DisplayName")]
            public string DisplayName { get; set; }
            [XmlAttribute(AttributeName = "BackgroundColor")]
            public string BackgroundColor { get; set; }
            [XmlAttribute(AttributeName = "Square44x44Logo")]
            public string Square44x44Logo { get; set; }
            [XmlAttribute(AttributeName = "Square71x71Logo")]
            public string Square71x71Logo { get; set; }
            [XmlAttribute(AttributeName = "Square150x150Logo")]
            public string Square150x150Logo { get; set; }
            [XmlAttribute(AttributeName = "Square310x310Logo")]
            public string Square310x310Logo { get; set; }
            [XmlAttribute(AttributeName = "Wide310x150Logo")]
            public string Wide310x150Logo { get; set; }
        }
    }
}