using Terminal.Gui.Reflect.Interfaces;

namespace Terminal.Gui.Reflect.Services;

public class ThemeService : IThemeService
{
    public ColorScheme SuccessText { get; set; } = new()
    {
        Normal = new Attribute(ColorName16.BrightGreen, ColorName16.DarkGray),
        Focus  = new Attribute(ColorName16.BrightGreen, ColorName16.Gray)
    };

    public ColorScheme InfoText { get; set; } = new()
    {
        Normal = new Attribute(ColorName16.BrightCyan, ColorName16.DarkGray),
        Focus  = new Attribute(ColorName16.BrightCyan, ColorName16.Gray)
    };

    public ColorScheme Base { get; set; } = new()
    {
        Normal    = new Attribute(ColorName16.White,      ColorName16.DarkGray),
        Focus     = new Attribute(ColorName16.White,      ColorName16.Gray),
        HotNormal = new Attribute(ColorName16.BrightCyan, ColorName16.DarkGray),
        HotFocus  = new Attribute(ColorName16.BrightCyan, ColorName16.Gray)
    };

    public ColorScheme Subtle { get; set; } = new()
    {
        Normal = new Attribute(ColorName16.Gray,  ColorName16.DarkGray),
        Focus  = new Attribute(ColorName16.White, ColorName16.Gray)
    };

    public ColorScheme Disabled { get; set; } = new()
    {
        Normal = new Attribute(ColorName16.Gray, ColorName16.DarkGray),
        Focus  = new Attribute(ColorName16.Gray, ColorName16.DarkGray)
    };

    public ColorScheme Button { get; set; } = new()
    {
        Normal    = new Attribute(ColorName16.White,      ColorName16.Gray),
        Focus     = new Attribute(ColorName16.White,      ColorName16.BrightCyan),
        HotNormal = new Attribute(ColorName16.BrightCyan, ColorName16.Gray),
        HotFocus  = new Attribute(ColorName16.DarkGray,   ColorName16.BrightCyan)
    };

    public ColorScheme ButtonDanger { get; set; } = new()
    {
        Normal    = new Attribute(ColorName16.BrightRed, ColorName16.Gray),
        Focus     = new Attribute(ColorName16.White,     ColorName16.Red),
        HotNormal = new Attribute(ColorName16.BrightRed, ColorName16.Gray),
        HotFocus  = new Attribute(ColorName16.White,     ColorName16.Red)
    };

    public ColorScheme Input { get; set; } = new()
    {
        Normal = new Attribute(ColorName16.White, ColorName16.Gray),
        Focus  = new Attribute(ColorName16.White, ColorName16.DarkGray)
    };

    public ColorScheme Selected { get; set; } = new()
    {
        Normal = new Attribute(ColorName16.White, ColorName16.Cyan),
        Focus  = new Attribute(ColorName16.White, ColorName16.BrightCyan)
    };

    public ColorScheme Header { get; set; } = new()
    {
        Normal    = new Attribute(ColorName16.White,      ColorName16.Gray),
        Focus     = new Attribute(ColorName16.BrightCyan, ColorName16.Gray),
        HotNormal = new Attribute(ColorName16.BrightCyan, ColorName16.Gray),
        HotFocus  = new Attribute(ColorName16.White,      ColorName16.BrightCyan)
    };

    public ColorScheme Border { get; set; } = new()
    {
        Normal = new Attribute(ColorName16.Gray,       ColorName16.DarkGray),
        Focus  = new Attribute(ColorName16.BrightCyan, ColorName16.DarkGray)
    };

    public ColorScheme Menu { get; set; } = new()
    {
        Normal    = new Attribute(ColorName16.White,      ColorName16.Gray),
        Focus     = new Attribute(ColorName16.White,      ColorName16.BrightCyan),
        HotNormal = new Attribute(ColorName16.BrightCyan, ColorName16.Gray),
        HotFocus  = new Attribute(ColorName16.White,      ColorName16.BrightCyan)
    };

    public ColorScheme WarningText { get; set; } = new()
    {
        Normal = new Attribute(ColorName16.BrightYellow, ColorName16.DarkGray),
        Focus  = new Attribute(ColorName16.BrightYellow, ColorName16.Gray)
    };

    public ColorScheme ErrorText { get; set; } = new()
    {
        Normal = new Attribute(ColorName16.BrightRed, ColorName16.DarkGray),
        Focus  = new Attribute(ColorName16.BrightRed, ColorName16.Gray)
    };
}