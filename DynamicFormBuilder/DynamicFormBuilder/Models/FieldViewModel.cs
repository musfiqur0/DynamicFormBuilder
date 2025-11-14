using System.ComponentModel.DataAnnotations;

namespace DynamicFormBuilder.Models
{
    public class FieldViewModel
    {
        [Required(ErrorMessage = "Label is required")]
        public string Label { get; set; } = string.Empty;

        public bool IsRequired { get; set; } = false;

        [Required(ErrorMessage = "Please select an option")]
        public string SelectedOption { get; set; } = string.Empty;
    }
}