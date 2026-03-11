namespace Terminal.Gui.Reflect.Settings;

public class PropertyGridSettings
{
    public bool ShowCategories { get; set; } = true;

    public bool ShowTitle { get; set; } = true;

    public bool ShowBorder { get; set; } = true;
    
    public EVerticalContentAlignment VerticalContentAlignment { get; set; } = EVerticalContentAlignment.Top;
    public EHorizontalContentAlignment HorizontalContentAlignment { get; set; } = EHorizontalContentAlignment.Left;
    
    public Thickness Margin { get; set; } = new  Thickness(1);
    public Thickness Padding { get; set; } = new  Thickness(0);
    
    public Thickness CategoryMargin { get; set; } = new  Thickness(1);
    public Thickness CategoryPadding { get; set; } = new  Thickness(0);
    
    public string? ConversionErrorColorSchemeName { get; set; }
}

public enum EVerticalContentAlignment
{
    Top,
    Center,
    Bottom,
}

public enum EHorizontalContentAlignment
{
    Left, 
    Center,
    Right
}