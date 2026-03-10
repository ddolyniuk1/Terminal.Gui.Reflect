using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Terminal.Gui.Reflect.Drawers
{
    /// <summary>
    /// Fallback editor for string and primitive value-type properties.
    /// Renders a label, text field, and validation message.
    /// Handles [DataType(Password)], [ReadOnly], [Display(Prompt)], and [Required].
    /// </summary>
    public class DefaultTextEditor : PropertyEditorBase
    {
        // CanHandleProperty is inherited from PropertyEditorBase (browsable = true)
        // so this is the catch-all fallback — register it at lowest priority.
        public override View Render(View owner, object model, PropertyInfo property)
        {
            var container = new View
            {
                CanFocus = true,
                Width    = Dim.Fill(),
                Height   = Dim.Auto(),
            };
            container.Padding!.Thickness = new Thickness(1);

            var required  = ValidationMapper.IsRequired(property) ? " *" : string.Empty;
            var label     = new Label
            {
                Text   = GetLabel(property) + required + ":",
                Height = 1,
                Width  = Dim.Fill(),
                X      = 0,
                Y      = 0,
            };
            container.Add(label);

            var dataType   = ValidationMapper.GetDataType(property);
            var isPassword = dataType == DataType.Password;
            var isReadOnly = IsReadOnly(property);

            var textField = new TextField
            {
                Width    = Dim.Fill(),
                Height   = 1,
                X        = 0,
                Y        = Pos.Bottom(label),
                Secret   = isPassword && !isReadOnly,
                ReadOnly = isReadOnly,
                TabStop  = isReadOnly ? TabBehavior.NoStop : TabBehavior.TabStop,
                CanFocus = true,
            };

            var prompt = GetPrompt(property);
            if (!string.IsNullOrEmpty(prompt))
            {
                textField.Caption = prompt;
            }

            container.Add(textField);

            var validationLabel = AddValidationLabel(container, textField);

            var binding = new PropertyBinding<string>(
                model,
                property,
                uiSetter: v => textField.Text = v ?? string.Empty);

            textField.TextChanging += (_, e) =>
            {
                if (!binding.PushToModel(() => e.NewValue))
                {
                    e.Cancel = true;
                }
            };

            binding.ValueChanged += value =>
            {
                Validate(model, property, value, validationLabel);
            };
 
            textField.HasFocusChanged += (_, args) =>
            {
                if (args.NewValue)
                {
                    return;
                }
                Validate(model, property, property.GetValue(model), validationLabel);
            };

            container.Removed += (_, _) => binding.Dispose();

            return container;
        }
    }
}