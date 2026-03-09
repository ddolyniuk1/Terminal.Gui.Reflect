using System.Reflection;

namespace Terminal.Gui.Reflect.Drawers
{
    public class DefaultBooleanEditor : PropertyEditorBase
    {
        public override View Render(View owner, object model, PropertyInfo propertyInfo)
        {
            return new CheckBox
            {
                Text = GetLabel(propertyInfo)
            };
        }
    }
}
