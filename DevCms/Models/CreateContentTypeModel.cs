using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevCms.ContentTypes;

namespace DevCms.Models
{
    public class CreateContentTypeModel
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = @"Field ""Name"" is required.")]
        public string Name { get; set; }
        public List<Attribute> Attributes { get; set; }
    }
}