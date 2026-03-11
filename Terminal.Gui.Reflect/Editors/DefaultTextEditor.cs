using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Terminal.Gui.Reflect.Attributes;
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
            var container = new View()
            {
                CanFocus = true,
                Width    = 10,
                Height   = 7,
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
            
            var infoLabel = new InfoLabel(description ?? "");
            infoLabel.Y  = 0;
            infoLabel.X  = Pos.Right(label) + 3;
            
            infoLabel.Width  = Dim.Auto(DimAutoStyle.Text);
            infoLabel.Height = 1;
                
            container.Add(infoLabel);
            if (description == null)
            {
                infoLabel.Visible = false;
            }

            var dataType   = ValidationMapper.GetDataType(property);
            var isPassword = dataType == DataType.Password;
            var isReadOnly = IsReadOnly(property);

            var textField = new TextField
            {
                Width    = Dim.Fill(),
                Height   = 3,
                X        = 0,
                Y        = Pos.Bottom(label),
                Secret   = isPassword && !isReadOnly,
                ReadOnly = isReadOnly,
                TabStop  = isReadOnly ? TabBehavior.NoStop : TabBehavior.TabStop,
                CanFocus = true,
                BorderStyle = LineStyle.Rounded,
            };
            textField.Border!.Thickness = new Thickness(1);

            var openDialogAttribute = property.GetCustomAttribute<OpenDialogAttribute>();
            if (openDialogAttribute != null)
            {
                var openDialogBtn = new Button();
                openDialogBtn.Text = "...";
                openDialogBtn.Width  = 5;
                openDialogBtn.Height = 1;
                openDialogBtn.X = Pos.AnchorEnd();
                openDialogBtn.Y = Pos.Bottom(label);
                container.Add(openDialogBtn);

                openDialogBtn.Activated += (_, _) =>
                {
                    var openDialogPath = property.GetValue(model) as string ?? "";
                    var split = openDialogPath.Split(';',  StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    var openDialog = new OpenDialog();
                    
                    openDialog.OpenMode = openDialogAttribute.Mode;
                    
                    openDialog.Path = split.FirstOrDefault() ?? "";

                    openDialog.AllowsMultipleSelection = openDialogAttribute.Options.HasFlag(OpenDialogAttributeOptions.AllowMultipleSelection);
                    
                    if (string.IsNullOrEmpty(openDialogAttribute.Filters))
                    {
                        openDialog.AllowedTypes = [new AllowedTypeAny()];
                    }
                    else
                    {
                        var filterString = openDialogAttribute.Filters.Split('|');
                        openDialog.AllowedTypes = filterString
                            .Select((t, i) => (t, i))
                            .Where(x => x.i % 2 == 0)
                            .Select(x => new AllowedType(
                                filterString.ElementAtOrDefault(x.i),
                                filterString.ElementAtOrDefault(x.i + 1)))
                            .OfType<IAllowedType>()
                            .ToList();
                    }
                    
                    openDialogBtn.SuperView!.App!.Run(openDialog);
                };
            }
            
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