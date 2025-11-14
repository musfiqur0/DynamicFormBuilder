namespace DynamicFormBuilder.Models
{
    /// <summary>
    /// Represents a dropdown field within a form. Each field has a label, indicates whether
    /// it is required and stores the option selected by the user. The set of possible
    /// options is fixed in the UI (Option 1, Option 2, Option 3), but the selected value
    /// is persisted in the database.
    /// </summary>
    public class FormField
    {
        /// <summary>
        /// Gets or sets the unique identifier for this field.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the parent form.
        /// </summary>
        public int FormId { get; set; }

        /// <summary>
        /// Gets or sets the label displayed above the dropdown.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this field is required.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the option selected by the user.
        /// </summary>
        public string SelectedOption { get; set; } = string.Empty;
    }
}