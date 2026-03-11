using System.Reflection;
using Terminal.Gui.Reflect.Base;
using Terminal.Gui.Reflect.Bindings;
using Terminal.Gui.Reflect.Settings;

namespace Terminal.Gui.Reflect.Editors
{
    /// <summary>
    /// Renders a <see cref="Button"/> that opens a selection dialog for enum properties.
    /// Supports nullable enums with a "(none)" option.
    /// </summary>
    public class DefaultEnumEditor : PropertyEditorBase
    {
        public override bool CanHandleProperty(PropertyInfo property)
        {
            if (!IsBrowsable(property)) return false;
            var t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            return t.IsEnum;
        }

        public override View Render(View owner, object model, PropertyInfo property, PropertyGridSettings propertyGridSettings)
        {
            var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
            var enumType       = underlyingType ?? property.PropertyType;
            var isNullable     = underlyingType != null;
            var isReadOnly     = IsReadOnly(property);
            var required       = ValidationMapper.IsRequired(property) ? " *" : string.Empty;

            var enumValues   = Enum.GetValues(enumType).Cast<object>().ToList();
            var displayNames = new List<string>();

            if (isNullable)
                displayNames.Add("(none)");

            displayNames.AddRange(enumValues.Select(v => v.ToString()!));

            var container = new View
            {
                CanFocus = true,
                Width    = 10,
                Height   = 3,
                ShadowStyle = ShadowStyle.None,
            };
            container.Padding!.Thickness = new Thickness(1);

            var label = new Label()
            {
                Text = GetLabel(property),
                X = 0,
                Y = 0,
            };
            var button = new Button
            {
                Text        = FormatButtonText(property, model, isNullable),
                Width       = Dim.Fill(),
                Height      = 1,
                X           = Pos.Right(label),
                Y           = 0,
                ShadowStyle = ShadowStyle.None,
                NoDecorations = true,
                TabStop     = isReadOnly ? TabBehavior.NoStop : TabBehavior.TabStop,
            };

            container.Add(label, button);

            var validationLabel = AddValidationLabel(container, button);

            var binding = new PropertyBinding<object?>(
                model,
                property,
                uiSetter: _ => button.Text = FormatButtonText(property, model, isNullable));

            button.Accepting += (_, _) =>
            {
                if (isReadOnly) return;

                var currentValue = property.GetValue(model);
                var currentIndex = currentValue == null
                    ? 0
                    : isNullable
                        ? enumValues.IndexOf(currentValue) + 1
                        : enumValues.IndexOf(currentValue);

                var list = new ListView
                {
                    Width  = Dim.Fill(),
                    Height = Dim.Fill(),
                    Source = new ListWrapper<string>([..displayNames]),
                    SelectedItem = Math.Max(currentIndex, 0),
                };

                var dialog = new Dialog
                {
                    Title  = GetLabel(property) + required,
                    Width  = 40,
                    Height = Math.Min(displayNames.Count + 4, 20),
                };

                dialog.Add(list);

                list.OpenSelectedItem += (_, _) =>
                {
                    var selected = list.SelectedItem;

                    binding.PushToModel(() =>
                    {
                        if (isNullable)
                            return selected == 0 ? null : enumValues[selected - 1];

                        return selected >= 0 ? enumValues[selected] : null;
                    });
                    button.Text = FormatButtonText(property, model, isNullable);
                    Validate(model, property, property.GetValue(model), validationLabel);
                    Application.RequestStop(dialog);
                };

                Application.Run(dialog);
            };

            container.Removed += (_, _) => binding.Dispose();

            return container;
        }

        private static string FormatButtonText(PropertyInfo property, object model, bool isNullable)
        {
            var value = property.GetValue(model);
            var display = value?.ToString() ?? (isNullable ? "(none)" : "—");
            return $"▼ {display}";
        }
    }
}