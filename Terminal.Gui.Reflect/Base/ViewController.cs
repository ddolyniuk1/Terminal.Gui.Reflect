using System.Reactive.Disposables;
using Terminal.Gui.Reflect.Extensions;
using Terminal.Gui.Reflect.Interfaces; 
using System.ComponentModel;
using System.Linq.Expressions; 

namespace Terminal.Gui.Reflect.Base;

public abstract class ViewController<TView> : IViewController<TView> where TView : View, new()
{
    public           TView               Root { get; } = new();
    private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

    protected ViewController()
    {
    }

    public void Initialize()
    {
        InitializeComponents();
        SetupBindings();
    }

    public void AddCleanupOperation(IDisposable disposable)
    {
        _compositeDisposable.Add(disposable);
    }

    public void AddCleanupOperation(Action cleanupAction)
    {
        _compositeDisposable.Add(Disposable.Create(cleanupAction));
    }

    public abstract void InitializeComponents();
    public abstract void SetupBindings();

    View IViewController.GetRootBase()
    {
        return Root;
    }

    public IViewController<TView> Apply(Action<TView> setCallback)
    {
        setCallback?.Invoke(Root);
        return this;
    }

    public void Dispose()
    {
        if (Root is IDisposable rootDisposable)
        {
            rootDisposable.Dispose();
        }

        _compositeDisposable.Dispose();
    }
}

public abstract class ViewController<TView, TViewModel> : ViewController<TView>, IViewFor<TViewModel>,
    IViewModelMapper<TViewModel>, IPrivateSetViewModelMapper
    where TView : View, new() where TViewModel : INotifyPropertyChanged
{
    public TViewModel ViewModel { get; private set; } = default!;

    /// <summary>
    /// Binds the property to the view and auto disposes
    /// </summary>
    /// <typeparam name="TProp"></typeparam>
    /// <typeparam name="TTargetView"></typeparam>
    /// <typeparam name="TTargetViewProp"></typeparam>
    /// <param name="modelExpr"></param>
    /// <param name="view"></param>
    /// <param name="viewExpr"></param>
    /// <param name="configure"></param>
    public void Bind<TProp, TTargetView, TTargetViewProp>(Expression<Func<TViewModel, TProp?>> modelExpr,
        TTargetView                                                                            view,
        Expression<Func<TTargetView, TTargetViewProp?>>                                        viewExpr,
        Action<BindingOptionsBuilder<TViewModel, TProp, TTargetView, TTargetViewProp>>?        configure = null)
        where TTargetView : View
    {
        Binding.TwoWay(ViewModel, modelExpr, view, viewExpr, configure)
               .DisposeWith(this);
    }

    void IPrivateSetViewModelMapper.SetViewModel(object viewModel)
    {
        ViewModel = (TViewModel)viewModel;
    }
}