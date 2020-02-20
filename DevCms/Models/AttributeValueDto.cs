using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevCms.ContentTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevCms.Models
{
    public class AttributeValueDto
    {
        [HiddenInput]
        public int? Id { get; set; }

        [Required]
        [HiddenInput]
        public int AttributeId { get; set; }

        public string AttributeName { get; set; }

        public bool Required { get; set; }

        [HiddenInput]
        public AttrType AttributeType { get; set; }

        //[Required(ErrorMessage = @"Field is required.")]
        public string Value { get; set; }

        //[Required(ErrorMessage = @"Field is required.")]
        public IFormFile ValueAsImage { get; set; }

        public List<DictionaryItem> DictionaryItems { get; set; }
    }
}