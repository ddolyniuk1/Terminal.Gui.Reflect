namespace Terminal.Gui.Reflect.Interfaces;

public interface IThemeService
{
    ColorScheme SuccessText  { get; set; }
    ColorScheme InfoText     { get; set; }
    ColorScheme Base         { get; set; }
    ColorScheme Subtle       { get; set; }
    ColorScheme Disabled     { get; set; }
    ColorScheme Button       { get; set; }
    ColorScheme ButtonDanger { get; set; }
    ColorScheme Input        { get; set; }
    ColorScheme Selected     { get; set; }
    ColorScheme Header       { get; set; }
    ColorScheme Border       { get; set; }
    ColorScheme Menu         { get; set; }
    ColorScheme WarningText  { get; set; }
    ColorScheme ErrorText    { get; set; }
}