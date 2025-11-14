using System;
using System.Collections.Generic;

namespace DynamicFormBuilder.Models
{
    /// <summary>
    /// Represents a form created by the user. A form has a title and one or more dropdown fields.
    /// </summary>
    public class Form
    {
        /// <summary>
        /// Gets or sets the unique identifier for this form.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the form.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date the form was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the collection of dropdown fields belonging to this form.
        /// </summary>
        public List<FormField> Fields { get; set; } = new();
    }
}