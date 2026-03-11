using Terminal.Gui.Views;

namespace Terminal.Gui.Reflect.Attributes;

[Flags]
public enum OpenDialogAttributeOptions
{
    None = 0,
    AllowMultipleSelection = 1 << 0,
}

[AttributeUsage(AttributeTargets.Property)]
public class OpenDialogAttribute : Attribute
{
    public OpenMode Mode { get; }
    public OpenDialogAttributeOptions Options { get; }
    public string? Filters { get; }

    public OpenDialogAttribute(
        OpenMode mode = OpenMode.File,
        OpenDialogAttributeOptions options = OpenDialogAttributeOptions.None,
        string? filters = null)
    {
        Mode = mode;
        Options = options;
        Filters = filters;
    }
}