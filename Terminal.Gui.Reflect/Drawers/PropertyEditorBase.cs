using System.ComponentModel;
using System.Reflection;

namespace Terminal.Gui.Reflect.Drawers
{
    /// <summary>
    /// Base class for all property editors.
    /// Subclasses implement <see cref="Render"/> and optionally override <see cref="CanHandleProperty"/>.
    /// </summary>
    public abstract class PropertyEditorBase
    {
        public abstract View Render(View owner, object model, PropertyInfo property);

        /// <summary>
        /// Return true if this editor can handle the given property.
        /// The editor registry calls this in priority order and uses the first match.
        /// </summary>
        public virtual bool CanHandleProperty(PropertyInfo property) =>
            IsBrowsable(property);

        protected string  GetLabel(PropertyInfo p)       => ValidationMapper.GetLabel(p);
        protected string? GetPrompt(PropertyInfo p)      => ValidationMapper.GetPrompt(p);
        protected string? GetDescription(PropertyInfo p) => ValidationMapper.GetDescription(p);

        protected bool IsBrowsable(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<BrowsableAttribute>();
            return attr?.Browsable != false;
        }

        protected bool IsReadOnly(PropertyInfo property)
        {
            if (!property.CanWrite) return true;
            var attr = property.GetCustomAttribute<ReadOnlyAttribute>();
            return attr?.IsReadOnly == true;
        }

        /// <summary>
        /// Creates and attaches the standard validation label below <paramref name="above"/>.
        /// Returns the label so the caller can position the next control below it.
        /// </summary>
        protected Label AddValidationLabel(View container, View above)
        {
            var lbl = new Label
            {
                Text            = string.Empty,
                Height          = 1,
                Width           = Dim.Fill(),
                X               = 0,
                Y               = Pos.Bottom(above),
                ColorScheme     = CreateErrorColorScheme(),
            };
            container.Add(lbl);
            return lbl;
        }

        /// <summary>
        /// Validates the property, updates the validation label, and returns whether the value is valid.
        /// </summary>
        protected bool Validate(object model, PropertyInfo property, object? value, Label validationLabel)
        {
            var result = ValidationMapper.ValidateAndNotify(model, property, value);
            validationLabel.Text = result.IsValid ? string.Empty : result.ErrorMessage ?? string.Empty;
            return result.IsValid;
        }

        private static ColorScheme CreateErrorColorScheme() => new()
        {
            Normal   = new Attribute(Color.BrightRed,  Color.Black),
            Focus    = new Attribute(Color.BrightRed,  Color.Black),
            HotNormal = new Attribute(Color.BrightRed, Color.Black),
            HotFocus  = new Attribute(Color.BrightRed, Color.Black),
        };
    }
}