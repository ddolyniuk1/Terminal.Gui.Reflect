using System.ComponentModel;
using System.Reflection;
using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Terminal.Gui.Reflect.Base;
using Terminal.Gui.Reflect.Settings;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Attribute = System.Attribute;

namespace Terminal.Gui.Reflect.Editors
{
    /// <summary>
    /// Base class for all property editors.
    /// Subclasses implement <see cref="Render"/> and optionally override <see cref="CanHandleProperty"/>.
    /// </summary>
    public abstract class PropertyEditorBase
    {
        private const string PropertyEditorValidationErrorScheme = "ConversionFailedTextEditorScheme";
        
        public abstract View Render(View owner, object model, PropertyInfo property, PropertyGridSettings propertyGridSettings);

        public PropertyEditorBase()
        {
            Scheme scheme;
            try
            {
                scheme = SchemeManager.GetScheme(PropertyEditorValidationErrorScheme);
            }
            catch (Exception e)
            {
                scheme = CreateErrorColorScheme();
                SchemeManager.AddScheme(PropertyEditorValidationErrorScheme, scheme);
            }
        }
        
        /// <summary>
        /// Return true if this editor can handle the given property.
        /// The editor registry calls this in priority order and uses the first match.
        /// </summary>
        public virtual bool CanHandleProperty(PropertyInfo property) =>
            IsBrowsable(property);

        protected string  GetLabel(PropertyInfo p)       => ValidationMapper.GetLabel(p);
        protected string? GetPrompt(PropertyInfo p)      => ValidationMapper.GetPrompt(p);
        protected string? GetDescription(PropertyInfo p) => ValidationMapper.GetDescription(p);
        private static readonly HashSet<Type> IntegerTypes = new()
        {
            typeof(int),    typeof(int?),
            typeof(long),   typeof(long?),
            typeof(short),  typeof(short?),
            typeof(byte),   typeof(byte?),
        };

        private static bool IsNumberType(Type type) =>
            IntegerTypes.Contains(type) ||
            IntegerTypes.Contains(Nullable.GetUnderlyingType(type) ?? type);
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
                SchemeName     = PropertyEditorValidationErrorScheme,
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

        private static Scheme CreateErrorColorScheme()
        {
            var baseScheme = SchemeManager.GetScheme("Base");
            return new Scheme
            {
                Normal = new Drawing.Attribute(Color.BrightRed, baseScheme.Normal.Background),
                Focus = new Drawing.Attribute(Color.BrightRed, baseScheme.Focus.Background),
                HotNormal = new Drawing.Attribute(Color.BrightRed, baseScheme.HotNormal.Background),
                HotFocus = new Drawing.Attribute(Color.BrightRed, baseScheme.HotFocus.Background),
            };
        }
    }
}