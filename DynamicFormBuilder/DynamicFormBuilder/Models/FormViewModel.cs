using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DynamicFormBuilder.Models
{
    public class FormViewModel
    {
        [Required(ErrorMessage = "Form title is required")]
        [Display(Name = "Form Title")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Form field is required")]
        public List<FieldViewModel> Fields { get; set; } = new();
    }
}