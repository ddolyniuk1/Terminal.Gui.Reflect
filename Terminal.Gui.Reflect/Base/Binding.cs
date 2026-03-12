using System.ComponentModel;
using System.Linq.Expressions;
using Terminal.Gui.Reflect.Bindings;

namespace Terminal.Gui.Reflect.Base;

public static class Binding
{
    /// <summary>
    ///     Binds a model property to a view's <c>Text</c> property (the most common TGui case).
    ///     The view property type is inferred as <c>string</c>.
    /// </summary>
    /// <example>
    ///     <code>
    /// var b = Binding.TwoWay(person, p => p.Name, nameField);
    /// </code>
    /// </example>
    public static PropertyBinding<TModel, TProp, TView, string> TwoWay<TModel, TProp, TView>(
        TModel                                                       model,
        Expression<Func<TModel, TProp?>>                             modelExpr,
        TView                                                        view,
        Action<BindingOptionsBuilder<TModel, TProp, TView, string>>? configure = null)
        where TModel : INotifyPropertyChanged
        where TView : View
    {
        return new PropertyBinding<TModel, TProp, TView, string>(model, modelExpr, view, v => v.Text, configure);
    }

    /// <summary>
    ///     Full overload: explicit expressions for both model and view properties.
    /// </summary>
    /// <example>
    ///     <code>
    /// var b = Binding.TwoWay(vm, m => m.Amount, numField, v => v.Text);
    /// </code>
    /// </example>
    public static PropertyBinding<TModel, TProp, TView, TViewProp> TwoWay<TModel, TProp, TView, TViewProp>(TModel model,
        Expression<Func<TModel, TProp?>> modelExpr,
        TView view,
        Expression<Func<TView, TViewProp?>> viewExpr,
        Action<BindingOptionsBuilder<TModel, TProp, TView, TViewProp>>? configure = null)
        where TModel : INotifyPropertyChanged
        where TView : View
    {
        return new PropertyBinding<TModel, TProp, TView, TViewProp>(model, modelExpr, view, viewExpr, configure);
    }
}