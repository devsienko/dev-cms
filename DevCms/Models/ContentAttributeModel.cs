using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevCms.ContentTypes;
using Microsoft.AspNetCore.Mvc;

namespace DevCms.Models
{
    public class ContentAttributeModel
    {
        [HiddenInput]
        public int? Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = @"Field ""Name"" is required.")]
        public string Name { get; set; }

        [DisplayName("Type")]
        [Required(ErrorMessage = @"Field ""Type"" is required.")]
        public AttrType AttributeType { get; set; }

        [DisplayName("Dictionary")]
        [Required]
        public int? DictionaryId { get; set; }

        [DisplayName("Required")]
        public bool Required { get; set; }
    }
}