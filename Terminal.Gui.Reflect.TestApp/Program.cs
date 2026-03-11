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
        VerticalScrollBar.VisibilityMode = ScrollBarVisibilityMode.Always;
        Title = "Example App (Ctrl+Q to quit)";
        var model = new BasicViewModel();
        var settings = new PropertyGridSettings()
        {
            ShowBorder = true,
        };
        var button = new Button()
        {
            Text = "Copy Layout Data"
        };
        button.Activated += (sender, args) =>
        {
            
        };
        var reflected = new PropertyGrid(model, settings);
        reflected.Width  = Dim.Fill();
        reflected.Height = Dim.Fill();
        reflected.VerticalScrollBar.VisibilityMode = ScrollBarVisibilityMode.Always;
        
        Add(reflected);
        
        reflected.DrawingSubViews += ReflectedOnDrawingSubViews;
        
        model.PropertyChanged += (sender, args) =>
        {
            Title = model.SomeText + " " + model.SomeBool;
        };
    }

    private void ReflectedOnDrawingSubViews(object? sender, DrawEventArgs e)
    {
        if (sender is PropertyGrid pg)
        {
            if (!pg.Viewport.IsEmpty)
            {
                Console.WriteLine("");
            }
        }
    }

    public override ShadowStyle ShadowStyle => ShadowStyle.None;
}