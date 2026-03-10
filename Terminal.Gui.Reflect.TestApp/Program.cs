using System.Diagnostics;
using Terminal.Gui;
using Terminal.Gui.Reflect;
using Terminal.Gui.Reflect.TerminalGuiComponents;
using Terminal.Gui.Reflect.TestApp;

Application.Run<ExampleWindow>();

Application.Shutdown();

System.Console.WriteLine($@"Username: {ExampleWindow.Username}");

public class ExampleWindow : Window
{
    public static string Username { get; internal set; }
    public TextField usernameText;

    public ExampleWindow()
    {
        Title = "Example App (Ctrl+Q to quit)";

        // Create input components and labels
        // var usernameLabel = new Label () {
        //     Text = "Username:"
        // };
        //
        // usernameText = new TextField () {
        //     // Position text field adjacent to the label
        //     X = Pos.Right (usernameLabel) + 1,
        //
        //     // Fill remaining horizontal space
        //     Width = Dim.Fill (),
        // };
        //
        // var passwordLabel = new Label () {
        //     Text = "Password:",
        //     X    = Pos.Left (usernameLabel),
        //     Y    = Pos.Bottom (usernameLabel) + 1
        // };
        //
        // var passwordText = new TextField () {
        //     Secret = true,
        //     // align with the text box above
        //     X     = Pos.Left (usernameText),
        //     Y     = Pos.Top (passwordLabel),
        //     Width = Dim.Fill (),
        // };
        //
        // // Create login button
        // var btnLogin = new Button () {
        //     Text = "Login",
        //     Y    = Pos.Bottom (passwordLabel) + 1,
        //     // center the login button horizontally
        //     X         = Pos.Center (),
        //     IsDefault = true,
        // };

        // When login button is clicked display a message popup
        // btnLogin.Clicked += () => {
        //     if (usernameText.Text == "admin" && passwordText.Text == "password") {
        //         MessageBox.Query ("Logging In", "Login Successful", "Ok");
        //         Username = usernameText.Text.ToString ();
        //         Application.RequestStop ();
        //     } else {
        //         MessageBox.ErrorQuery ("Logging In", "Incorrect username or password", "Ok");
        //     }
        // };

        //// Add the views to the Window
        //Add (usernameLabel, usernameText, passwordLabel, passwordText, btnLogin);

      
        Application.Init();
        ConfigurationManager.Themes.Theme = "Dark";
        ConfigurationManager.Apply();
        var sw        = Stopwatch.StartNew();
        var model = new BasicViewModel();
        var settings = new PropertyGridSettings()
        {
            ShowBorder = false,
            
        };
        var reflected = new PropertyGrid(model, settings);
        reflected.Width  = Dim.Fill();
        reflected.Height = Dim.Fill();
        Add(reflected);
        
        model.PropertyChanged += (sender, args) =>
        {
            Title = model.SomeText + " " + model.SomeBool;
        };
        
        //
        // var uniformGrid = new UniformGrid(1);
        // uniformGrid.Width  = Dim.Fill();
        // uniformGrid.Height = Dim.Fill();
        // uniformGrid.Add (usernameLabel, usernameText, passwordLabel, passwordText, btnLogin);
        // Add(uniformGrid);
        // Debug.WriteLine("Total time taken: " + sw.ElapsedMilliseconds + "ms");


    }
}