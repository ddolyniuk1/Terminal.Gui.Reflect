using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Terminal.Gui.Reflect.Base;

namespace Terminal.Gui.Reflect.Bindings;

/// <inheritdoc />
/// <summary>
///     Expression-based two-way property binding for Terminal.Gui v2 views.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
/// <typeparam name="TProp">The property type on the model.</typeparam>
/// <typeparam name="TView">The Terminal.Gui view type.</typeparam>
/// <typeparam name="TViewProp">The view property type (often string).</typeparam>
public sealed class PropertyBinding<TModel, TProp, TView, TViewProp> : IDisposable
    where TModel : INotifyPropertyChanged
    where TView : View
{
    private readonly TModel                                          _model;
    private readonly Func<TModel, TProp?>                            _modelGetter;
    private readonly PropertyInfo                                    _modelProp;
    private readonly Action<TModel, TProp?>                          _modelSetter;
    private readonly TView                                           _view;
    private readonly Func<TView, TViewProp?>                         _viewGetter;
    private readonly PropertyInfo                                    _viewProp;
    private readonly Action<TView, TViewProp?>                       _viewSetter;
    private          bool                                            _updating;
    private readonly BindingOptions<TModel, TProp, TView, TViewProp> _options;

    /// <param name="model">The data model (must implement INotifyPropertyChanged).</param>
    /// <param name="modelExpr">Member expression selecting the model property, e.g. <c>m => m.Name</c>.</param>
    /// <param name="view">The Terminal.Gui view to bind to.</param>
    /// <param name="viewExpr">Member expression selecting the view property, e.g. <c>v => v.Text</c>.</param>
    /// <param name="converterIn"></param>
    /// <param name="converterOut"></param>
    /// <param name="changedCallback"></param>
    /// <param name="configure"></param>
    public PropertyBinding(TModel                                    model,
        Expression<Func<TModel, TProp?>>                             modelExpr,
        TView                                                        view,
        Expression<Func<TView, TViewProp?>>                          viewExpr,
        Action<BindingOptionsBuilder<TModel, TProp, TView, TViewProp>>? configure = null)
    {
        _model           = model;
        _view            = view;
        var bindingOptionsBuilder = new BindingOptionsBuilder<TModel, TProp, TView, TViewProp>();
        configure?.Invoke(bindingOptionsBuilder);
        _options = bindingOptionsBuilder.Build();

        (_modelProp, _modelGetter, _modelSetter) = ResolveProperty<TModel, TProp>(modelExpr);
        (_viewProp, _viewGetter, _viewSetter)    = ResolveProperty<TView, TViewProp>(viewExpr);

        // Seed UI from current model value
        PushToView();

        _model.PropertyChanged += OnModelPropertyChanged;

        // Wire Terminal.Gui v2 unified text-changed event
        _view.TextChanged += OnViewTextChanged;
    }

    public void Dispose()
    {
        _model.PropertyChanged -= OnModelPropertyChanged;
        _view.TextChanged      -= OnViewTextChanged;
    }

    /// <summary>Raised after the model value is successfully updated from the UI.</summary>
    public event Action<TProp?>? ValueChanged;

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != _modelProp.Name || _updating)
        {
            return;
        }

        _updating = true;
        try
        {
            PushToView();

            _options.OnChanged?.Invoke();
        }
        finally
        {
            _updating = false;
        }
    }

    private void PushToView()
    {
        var modelValue = _modelGetter(_model);

        var coerceValue = _options.ConvertIn != null ? _options.ConvertIn.Invoke(modelValue) : CoerceValue<TProp, TViewProp>(modelValue);
        _viewSetter(_view, coerceValue);
        
        _options.OnModelChanged?.Invoke(modelValue);
        _options.OnChanged?.Invoke();
    }

    private void OnViewTextChanged(object? sender, System.EventArgs e)
    {
        if (_updating)
        {
            return;
        }

        _updating = true;
        try
        {
            var viewValue  = _viewGetter(_view);

            var modelValue = _options.ConvertOut != null ? _options.ConvertOut.Invoke(viewValue) : CoerceValue<TViewProp, TProp>(viewValue);

            _modelSetter(_model, modelValue);
            
            _options.OnViewChanged?.Invoke(viewValue);
            _options.OnChanged?.Invoke();
            ValueChanged?.Invoke(modelValue);
        }
        finally
        {
            _updating = false;
        }
    }

    /// <summary>
    ///     Extracts PropertyInfo + compiled getter/setter from a member expression.
    /// </summary>
    private static (PropertyInfo prop,
        Func<TOwner, TVal?> getter,
        Action<TOwner, TVal?> setter)
        ResolveProperty<TOwner, TVal>(Expression<Func<TOwner, TVal?>> expr)
    {
        if (expr.Body is not MemberExpression memberExpr ||
            memberExpr.Member is not PropertyInfo pi)
        {
            throw new ArgumentException($"Expression must be a simple property access, got: {expr.Body}");
        }

        if (!pi.CanRead)
        {
            throw new ArgumentException($"Property {pi.Name} is not readable.");
        }

        if (!pi.CanWrite)
        {
            throw new ArgumentException($"Property {pi.Name} is not writable.");
        }

        // Compile getter directly from the supplied lambda
        var getter = expr.Compile();

        // Build setter: (owner, value) => owner.Prop = value
        var ownerParam = Expression.Parameter(typeof(TOwner), "owner");
        var valueParam = Expression.Parameter(typeof(TVal),   "value");
        var setterLambda = Expression.Lambda<Action<TOwner, TVal?>>(
            Expression.Assign(
                Expression.Property(ownerParam, pi),
                valueParam),
            ownerParam, valueParam);
        var setter = setterLambda.Compile();

        return (pi, getter, setter);
    }

    /// <summary>
    ///     Coerces a value from <typeparamref name="TFrom" /> to <typeparamref name="TTo" />,
    ///     handling null, identity, and Convert.ChangeType for cross-type scenarios
    ///     (e.g. int model ↔ string view).
    /// </summary>
    private static TTo? CoerceValue<TFrom, TTo>(TFrom? value)
    {
        if (value is null)
        {
            return default;
        }

        if (value is TTo to)
        {
            return to;
        }

        var target = Nullable.GetUnderlyingType(typeof(TTo)) ?? typeof(TTo);

        try
        {
            return (TTo?)Convert.ChangeType(value, target);
        }
        catch
        {
            return default;
        }
    }
}

/// <summary>
///     Two-way binding between a model property and ANY control.
///     Supply delegates for reading/writing the UI value. Wire up the
///     UI-changed event yourself, then call <see cref="PushToModel" /> from it.
///     Example — checkbox:
///     <code>
///   var binding = new PropertyBinding&lt;bool&gt;(model, property,
///       uiSetter: v => checkbox.CheckedState = v ? CheckState.Checked : CheckState.UnChecked);
/// 
///   checkbox.Toggle += (_, _) =>
///       binding.PushToModel(() => checkbox.CheckedState == CheckState.Checked);
/// 
///   view.Removed += (_, _) => binding.Dispose();
/// </code>
///     Example — text field:
///     <code>
///   var binding = new PropertyBinding&lt;string&gt;(model, property,
///       uiSetter: v => textField.Text = v ?? string.Empty);
/// 
///   textField.TextChanging += (_, e) =>
///       binding.PushToModel(() => e.NewValue);
/// 
///   view.Removed += (_, _) => binding.Dispose();
/// </code>
/// </summary>
public sealed class PropertyBinding<T> : IDisposable
{
    private readonly object       _model;
    private readonly PropertyInfo _property;
    private readonly Action<T?>   _uiSetter;
    private          bool         _updating;

    public PropertyBinding(object model, PropertyInfo property, Action<T?> uiSetter)
    {
        _model    = model;
        _property = property;
        _uiSetter = uiSetter;

        // Initialise UI from current model value
        _uiSetter(GetModelValue());

        if (model is INotifyPropertyChanged inpc)
        {
            inpc.PropertyChanged += OnModelPropertyChanged;
        }
    }

    public void Dispose()
    {
        if (_model is INotifyPropertyChanged inpc)
        {
            inpc.PropertyChanged -= OnModelPropertyChanged;
        }
    }

    /// <summary>Raised after the model value is successfully updated.</summary>
    public event Action<T?>? ValueChanged;

    /// <summary>
    ///     Call from a UI-changed event to push the current UI value into the model.
    ///     Skips if a model→UI update is already in progress (prevents loops).
    /// </summary>
    public void PushToModel(Func<T?> uiGetter)
    {
        if (_updating)
        {
            return;
        }

        _updating = true;
        try
        {
            var uiValue = uiGetter();
            _property.SetValue(_model, CoerceToPropertyType(uiValue));
            ValueChanged?.Invoke(uiValue);
        }
        finally
        {
            _updating = false;
        }
    }

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != _property.Name || _updating)
        {
            return;
        }

        _updating = true;
        try
        {
            _uiSetter(GetModelValue());
        }
        finally
        {
            _updating = false;
        }
    }

    private T? GetModelValue()
    {
        var raw = _property.GetValue(_model);
        switch (raw)
        {
            case null:
                return default;
            case T typed:
                return typed;
            default:
                try
                {
                    return (T?)Convert.ChangeType(raw, typeof(T));
                }
                catch
                {
                    return default;
                }

                break;
        }
    }

    /// <summary>
    ///     Handles the common case where T is string but the property is int/decimal/etc.
    /// </summary>
    private object? CoerceToPropertyType(T? value)
    {
        if (value is null)
        {
            return null;
        }

        var target = Nullable.GetUnderlyingType(_property.PropertyType) ?? _property.PropertyType;
        return value.GetType() == target ? value : Convert.ChangeType(value, target);
    }
}