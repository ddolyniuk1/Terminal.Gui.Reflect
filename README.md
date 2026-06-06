
# Terminal.Gui.Reflect

This project is an opinionated view framework for scalable Terminal.GUI apps

## Acknowledgements

 - [Terminal.Gui](https://github.com/gui-cs/Terminal.Gui)

## Authors

- [@ddolyniuk1](https://www.github.com/ddolyniuk1)


## Features

- PropertyGrid
    - A view that auto generates categories and properties
    - Each field supports validation
- UniformGrid
    - A view similar to WPF's UniformGrid
- InfoLabel
    - A label that shows a tooltip when hovered


## Usage/Examples

### PropertyGrid Example: 
```csharp

var hostBuilder = Host.CreateApplicationBuilder();
hostBuilder.Services.AddReflectServices();
hostBuilder.Services.AddOptions<AppSettings>();
hostBuilder.Services.AddSingleton(Application.Create().Init());

using var host = hostBuilder.Build();

await host.StartAsync();

var app = host.Services.GetRequiredService<IApplication>();
var viewControllerFactory = host.Services.GetRequiredService<IViewControllerFactory>();
var window = new Window
{
    Title = "Sample App"
};

window.Add(viewControllerFactory.Create<DefaultView>()); // use our IViewControllerFactory to initialize and autowire views

app.Run(window);

host.WaitForShutdown();
```

### View / ViewModel Example
```csharp
public class DefaultView : ViewController<FrameView, DefaultViewModel>
{
    private readonly Button _btn = new Button()
    {
        Text = "Click me!"
    };

    public override void InitializeComponents()
    {
        Root.Width  = Dim.Fill();
        Root.Height = Dim.Fill();
        Root.Text = "Default View";
        _btn.X      = Pos.Center();
        _btn.Y      = Pos.Center();
        _btn.MouseEvent += BtnOnMouseEvent; // hook to mouse events for this button

        Root.Add(_btn); // add our button to our root component

        AddCleanupOperation(() => _btn.MouseEvent -= BtnOnMouseEvent); // unhook event when cleaning up
    }

    private void BtnOnMouseEvent(object? sender, Mouse e)
    {
        if (e.IsPressed)    
        {
            ViewModel.Counter++; // when our button is clicked, increment the counter
        }
    }

    public override void SetupBindings()
    {
        // custom binding utility function
        Binding.TwoWay(ViewModel,                                          // view model
                       model => model.Counter,                             // property expression
                       _btn,                                               // ui element
                       builder => builder
                           .WithConvertIn(i => _btn.Text = $"Clicked {i}") // map the value
                           .Build())
                       .DisposeWith(this);                                 // cleanup
        
        // for basic bindings we use the Bind method instead
        // Bind(m => m.Counter, _label, l => l.Text)
    }
}

public class DefaultViewModel : INotifyPropertyChanged
{
    public int Counter
    {
        get;
        set
        {
            if (value == field)
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```
