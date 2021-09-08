using GrpcServer.Source.Common.Converters;

namespace GrpcServer.Source.Models
{
    public class KV
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString() => $"{Key.Base64ToUTF8()}={Value.Base64ToUTF8()}";
    }
}
