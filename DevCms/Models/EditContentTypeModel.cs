using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DevCms.ContentTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevCms.Models
{
    public class EditContentTypeModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = @"Field ""Name"" is required.")]
        public string Name { get; set; }

        public List<ContentAttributeModel> Attrs { get; set; }

        public ContentAttributeModel AddedOrEditedAttr { get; set; }
        
        public List<SelectListItem> AttrTypes = new List<SelectListItem>
        {
            new SelectListItem
            {
                Value = AttrType.String.ToString(),
                Text = AttrType.String.ToString()
            },
            new SelectListItem
            {
                Value = AttrType.Image.ToString(),
                Text = AttrType.Image.ToString()
            }
        };
    }
}