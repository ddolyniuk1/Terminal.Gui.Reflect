namespace Terminal.Gui.Reflect.Base;

public delegate void PropertyChangeCallback<in TProp>(TProp? newValue);
public class BindingOptions<TModel, TProp, TView, TViewProp>
{
    public Func<TProp?, TViewProp?>? ConvertIn  { get; set; }
    public Func<TViewProp?, TProp?>? ConvertOut { get; set; }

    public PropertyChangeCallback<TProp>?     OnModelChanged { get; set; }
    public PropertyChangeCallback<TViewProp>? OnViewChanged  { get; set; }

    public Action? OnChanged { get; set; }
}