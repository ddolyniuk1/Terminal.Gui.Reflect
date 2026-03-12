namespace Terminal.Gui.Reflect.Base;

public class BindingOptionsBuilder<TModel, TProp, TView, TViewProp>
{
    private readonly BindingOptions<TModel, TProp, TView, TViewProp> _options =
        new BindingOptions<TModel, TProp, TView, TViewProp>();

    public BindingOptionsBuilder<TModel, TProp, TView, TViewProp> WithConvertIn(Func<TProp?, TViewProp?>? convertIn)
    {
        _options.ConvertIn = convertIn;
        return this;
    }
    
    public BindingOptionsBuilder<TModel, TProp, TView, TViewProp> WithConvertOut(Func<TViewProp?, TProp?>? convertOut)
    {
        _options.ConvertOut = convertOut;
        return this;
    }

    public BindingOptionsBuilder<TModel, TProp, TView, TViewProp> WithOnModelChangedCallback(
        PropertyChangeCallback<TProp>? callback)
    {
        _options.OnModelChanged = callback;
        return this;
    }

    public BindingOptionsBuilder<TModel, TProp, TView, TViewProp> WithOnViewChangedCallback(
        PropertyChangeCallback<TViewProp>? callback)
    {
        _options.OnViewChanged = callback;
        return this;
    }
    
    public BindingOptionsBuilder<TModel, TProp, TView, TViewProp> WithOnChangedCallback(
        Action callback)
    {
        _options.OnChanged = callback;
        return this;
    }

    public BindingOptions<TModel, TProp, TView, TViewProp> Build()
    {
        return _options;
    }
}