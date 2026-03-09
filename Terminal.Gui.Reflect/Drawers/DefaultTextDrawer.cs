using System.Reflection;
using Terminal.Gui.Reflect.TerminalGuiComponents;

namespace Terminal.Gui.Reflect.Drawers
{
    public class DefaultTextEditor : PropertyEditorBase
    {
        public override View Render(View owner, object model, PropertyInfo propertyInfo)
        {
            var view = new UniformGrid(1, 2);
            view.Width  = Dim.Fill();
            view.Height = Dim.Fill();
            view.Add(new Label { Text = GetLabel(propertyInfo) + ": " });

            var textField = new TextField();
            textField.TabStop = TabBehavior.NoStop;
            view.Add(textField);
            textField.Height = 1;
            return view;
        }
    }
}
