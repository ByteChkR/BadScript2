using Newtonsoft.Json.Linq;

namespace BadScript2.Container
{
    public class BadLayeredFileSystemInfo
    {
        public bool Writable { get;set; }
        public string Name { get; set; }
        public JObject MetaData { get; set; }
    }
}