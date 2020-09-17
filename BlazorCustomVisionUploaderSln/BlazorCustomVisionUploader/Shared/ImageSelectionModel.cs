using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace BlazorCustomVisionUploader.Shared
{
    public class GuidValueAttribute:ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (Guid.TryParse(value.ToString(), out Guid result))
                return true;
            else
                return false;
        }
    }
    public class UploadImagesModel
    {
        [Required]
        [GuidValue]
        public string ProjectId { get; set; }
        [Required]
        public string Tag { get; set; }
        public List<ImageSelectionItem> Items { get; set; }
    }
    public class ImageSelectionItem
    {
        public bool IsSelected { get; set; }
        [Required]
        [Url]
        public string ImageUrl { get; set; }
    }
}
