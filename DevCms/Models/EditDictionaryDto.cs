﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace DevCms.Models
{
    public class EditDictionaryDto
    {
        [HiddenInput]
        public int? Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        public IEnumerable<EditDictionaryItemDto> Items { get; set; }

        public EditDictionaryItemDto AddedOrEditedItem { get; set; }
    }
}