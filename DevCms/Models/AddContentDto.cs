using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace DevCms.Models
{
    public class AddEntityDto
    {
        [HiddenInput]
        public int? Id { get; set; }

        [HiddenInput]
        public int ContentTypeId { get; set; }

        public List<AttributeValueDto> Attrs{ get; set; }
    }
}