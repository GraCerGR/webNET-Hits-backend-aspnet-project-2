﻿using System.ComponentModel.DataAnnotations;

namespace Test.Models.DTO
{
    public class TagDto
    {
        [Required]

        public Guid id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]

        public string createTime { get; set; }

        [Required]
        [MinLength(1)]

        public string name { get; set; }

        //public string PostDtoid { get; set; }

    }
}
