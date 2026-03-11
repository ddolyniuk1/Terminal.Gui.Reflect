using Terminal.Gui;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Terminal.Gui.Input;
using Terminal.Gui.Reflect.Settings;
using Terminal.Gui.Reflect.TestApp;
using Terminal.Gui.Reflect.Views;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

ConfigurationManager.RuntimeConfig = """{ "Theme": "Dark" }""";
ConfigurationManager.Enable (ConfigLocations.All);
IApplication app = Application.Create().Init ();
ConfigurationManager.Apply();
app.Run<ExampleWindow>();

app.Dispose();

public class ExampleWindow : Window
{

    public ExampleWindow()
    { 
        Title = "Example App (Ctrl+Q to quit)";
        var model = new BasicViewModel();
        var settings = new PropertyGridSettings()
        {
            ShowBorder = true,
        };
        var reflected = new PropertyGrid(model, settings);
        reflected.Width  = Dim.Fill();
        reflected.Height = Dim.Fill();
        Add(reflected);
        
        model.PropertyChanged += (sender, args) =>
        {
            Title = model.SomeText + " " + model.SomeBool;
        };
    }
    
    public override ShadowStyle ShadowStyle => ShadowStyle.None;
}