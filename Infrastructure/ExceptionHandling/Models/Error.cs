using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AuthorizationServer.ExceptionHandling.Models
{
    public class Error
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        public string Message { get; }

        public Error(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }
}
