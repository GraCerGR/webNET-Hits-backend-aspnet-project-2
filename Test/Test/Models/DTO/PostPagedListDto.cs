namespace Test.Models.DTO
{
    public class PostPagedListDto
    {
        public PostDto posts { get; set; }

        public PageInfoModel pagination { get; set; }
    }
}
