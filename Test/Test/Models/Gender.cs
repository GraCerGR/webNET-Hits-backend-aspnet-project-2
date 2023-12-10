using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Test.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        Male,
        Female
    }
}
