using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Terminal.Gui.Reflect.Base;
using Terminal.Gui.Reflect.Bindings;
using Terminal.Gui.Reflect.Settings;
using Terminal.Gui.Reflect.Views;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Attribute = System.Attribute;

namespace Terminal.Gui.Reflect.Editors
{
    /// <summary>
    /// Fallback editor for string and primitive value-type properties.
    /// Renders a label, text field, and validation message.
    /// Handles [DataType(Password)], [ReadOnly], [Display(Prompt)], and [Required].
    /// </summary>
    public class DefaultTextEditor : PropertyEditorBase
    {
        private const string DefaultTextEditorConversionFailedSchemeName = "DefaultTextEditorConversionFailedScheme";

        // CanHandleProperty is inherited from PropertyEditorBase (browsable = true)
        // so this is the catch-all fallback — register it at lowest priority.
        public override View Render(View owner, object model, PropertyInfo property, PropertyGridSettings propertyGridSettings)
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
                Width  = Dim.Auto(DimAutoStyle.Text),
                X      = 0,
                Y      = 0,
            };

            label.X = propertyGridSettings.HorizontalContentAlignment switch
            {
                EHorizontalContentAlignment.Left => 0,
                EHorizontalContentAlignment.Center => Pos.Center(),
                EHorizontalContentAlignment.Right => Pos.AnchorEnd(1),
                _ => throw new ArgumentOutOfRangeException()
            };

            label.Y = propertyGridSettings.VerticalContentAlignment switch
            {
                EVerticalContentAlignment.Top => 0,
                EVerticalContentAlignment.Center => Pos.Center(),
                EVerticalContentAlignment.Bottom => Pos.AnchorEnd(1),
                _ => throw new ArgumentOutOfRangeException()
            };

            container.Add(label);

            var description = GetDescription(property);
            if (description != null)
            {
                var infoLabel = new InfoLabel(description);
                infoLabel.Y  = label.Y;
                infoLabel.X  = Pos.Right(label) + 3;
                
                infoLabel.Width  = Dim.Auto(DimAutoStyle.Text);
                infoLabel.Height = 1;
                
                container.Add(infoLabel);
            }

            var dataType   = ValidationMapper.GetDataType(property);
            var isPassword = dataType == DataType.Password;
            var isReadOnly = IsReadOnly(property);

            var textField = new TextField
            {
                Width    = Dim.Fill(),
                Height   = 1,
                X        = label.X,
                Y        = Pos.Bottom(label),
                Secret   = isPassword && !isReadOnly,
                ReadOnly = isReadOnly,
                TabStop  = isReadOnly ? TabBehavior.NoStop : TabBehavior.TabStop,
                CanFocus = true,
            };
 
            container.Add(textField);

            var validationLabel = AddValidationLabel(container, textField);

            var binding = new PropertyBinding<string>(
                model,
                property,
                uiSetter: v => textField.Text = v ?? string.Empty);

            textField.TextChanging += (_, e) =>
            {
                try
                {
                    binding.PushToModel(() => e.Result);
                    textField.SchemeName = container.SchemeName;
                }
                catch
                {
                    if (string.IsNullOrEmpty(propertyGridSettings.ConversionErrorColorSchemeName))
                    {
                        var parentScheme = SchemeManager.GetScheme(owner.App!.TopRunnableView!.SchemeName!);
                        SchemeManager.AddScheme(DefaultTextEditorConversionFailedSchemeName, new Scheme
                        {
                            Focus = new Drawing.Attribute(Color.BrightRed, parentScheme!.Focus.Background),
                            Normal = new Drawing.Attribute(Color.BrightRed, parentScheme!.Normal.Background)
                        });
                        textField.SchemeName = DefaultTextEditorConversionFailedSchemeName;
                    }
                    else
                    {
                        textField.SchemeName =  propertyGridSettings.ConversionErrorColorSchemeName;
                    }
                    textField.SetNeedsDraw();
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