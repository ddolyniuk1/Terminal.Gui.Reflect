using Terminal.Gui; 
using Terminal.Gui.Reflect.TestApp;

public static class Program
{
    public static void Main()
    {
        
    }
}
// //  
// // ConfigurationManager.Apply();
// // ThemeManager.Instance.Theme = "Dark";
// //
// // Application.Run<ExampleWindow>();
// //
// // Application.Shutdown();
//
// public class ExampleWindow : Window
// {
//     public ExampleWindow()
//     { 
//         VerticalScrollBar.AutoShow = true;
//         Title = "Example App (Ctrl+Q to quit)";
//         var model = new BasicViewModel();
//         // var settings = new PropertyGridSettings()
//         // {
//         //     ShowBorder = false,
//         // };
//         Task.Run(() =>
//         { 
//             model.SomeBool2 = true;
//         });
//         
//         // var reflected = new PropertyGrid(model, settings);
//         // reflected.Width  = Dim.Fill();
//         // reflected.Height = Dim.Auto();
//         //
//         // VerticalScrollBar.ScrollableContentSize = 55;
//         //
//         // Add(reflected);
//         model.PropertyChanged += (sender, args) =>
//         {
//             Title = model.SomeText + " " + model.SomeBool;
//         };
//     }
// }