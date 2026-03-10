using System.Reflection;
using Terminal.Gui.Drawing;
using Terminal.Gui.Reflect.Base;
using Terminal.Gui.Reflect.Bindings;
using Terminal.Gui.Reflect.Settings;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Terminal.Gui.Reflect.Editors
{
    /// <summary>
    /// Renders a <see cref="CheckBox"/> for <c>bool</c> and <c>bool?</c> properties.
    /// Supports <c>bool?</c> as a tri-state checkbox (null = indeterminate).
    /// </summary>
    public class BoolCheckboxEditor : PropertyEditorBase
    {
        public override bool CanHandleProperty(PropertyInfo property)
        {
            if (!IsBrowsable(property)) return false;
            var t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            return t == typeof(bool);
        }

        public override View Render(View owner, object model, PropertyInfo property, PropertyGridSettings propertyGridSettings)
        {
            var isNullable = Nullable.GetUnderlyingType(property.PropertyType) != null;
            var isReadOnly = IsReadOnly(property);
            var required   = ValidationMapper.IsRequired(property) ? " *" : string.Empty;

            var container = new View
            {
                CanFocus = true,
                Width    = Dim.Fill(),
                Height   = Dim.Auto(),
            };
            container.Padding!.Thickness = new Thickness(1);

            var checkbox = new CheckBox
            {
                Text      = GetLabel(property) + required,
                Width     = Dim.Fill(),
                Height    = 1,
                X         = 0,
                Y         = 0,
                
                // Allow indeterminate state for nullable bools
                AllowCheckStateNone = isNullable,
                TabStop   = isReadOnly ? TabBehavior.NoStop : TabBehavior.TabStop,
            };

            checkbox.X = propertyGridSettings.HorizontalContentAlignment switch
            {
                EHorizontalContentAlignment.Left => 0,
                EHorizontalContentAlignment.Center => Pos.Center(),
                EHorizontalContentAlignment.Right => Pos.AnchorEnd()
            };

            checkbox.Y = propertyGridSettings.VerticalContentAlignment switch
            {
                EVerticalContentAlignment.Top => 0,
                EVerticalContentAlignment.Center => Pos.Center(),
                EVerticalContentAlignment.Bottom => Pos.AnchorEnd()
            };
            
            container.Add(checkbox);

            var validationLabel = AddValidationLabel(container, checkbox);

            // ── Binding ────────────────────────────────────────────────────────
            // Use bool? internally so we handle both bool and bool? properties uniformly.
            var binding = new PropertyBinding<bool?>(
                model,
                property,
                uiSetter: value => checkbox.Value = value switch
                {
                    true  => CheckState.Checked,
                    false => CheckState.UnChecked,
                    null  => CheckState.None,
                });

            checkbox.ValueChanged += (_, _) =>
            {
                binding.PushToModel(() => checkbox.Value switch
                {
                    CheckState.Checked   => true,
                    CheckState.UnChecked => false,
                    _                    => (bool?)null,
                });

                Validate(model, property, property.GetValue(model), validationLabel);
            };

            container.Removed += (_, _) => binding.Dispose();

            return container;
        }
    }
}