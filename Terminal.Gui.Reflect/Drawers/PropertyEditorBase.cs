using System.ComponentModel;
using System.Reflection;

namespace Terminal.Gui.Reflect.Drawers;

public abstract class PropertyEditorBase
{
    public abstract View Render(View                    owner, object model, PropertyInfo propertyInfo);

    /// <summary>
    /// Override this to support filtering
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public virtual bool CanHandleProperty(PropertyInfo property)
    {
        return IsBrowsable(property);
    }
    
    protected virtual string GetLabel(PropertyInfo property)
    {
        return property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? property.Name;
    }

    protected virtual bool IsReadonly(PropertyInfo property)
    { 
        var browsable = property.GetCustomAttribute<ReadOnlyAttribute>();
        return browsable?.IsReadOnly == true;
    }

    protected virtual bool IsBrowsable(PropertyInfo property)
    {
        var browsable = property.GetCustomAttribute<BrowsableAttribute>();
        return browsable?.Browsable != false;
    }
}