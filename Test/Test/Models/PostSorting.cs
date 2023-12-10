using System.Text.Json.Serialization;

namespace Test.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PostSorting
    {
        CreateDesc,
        CreateAsc,
        LikeAsc,
        LikeDesc
    }
}
