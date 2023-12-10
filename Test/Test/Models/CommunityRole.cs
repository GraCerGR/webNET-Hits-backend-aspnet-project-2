using System.Data;
using System.Drawing;
using System.Text.Json.Serialization;

namespace Test.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CommunityRole
    {
        Administrator = 0,
        Subscriber = 1
    }
}
