﻿namespace Test.Models.DTO
{
    public class PostPagedListDto
    {
        public List<PostDto> posts { get; set; }

        public PageInfoModel pagination { get; set; }
    }
}
