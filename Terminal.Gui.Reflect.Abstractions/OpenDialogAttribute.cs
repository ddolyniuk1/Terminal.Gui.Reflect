namespace Terminal.Gui.Reflect.Attributes;

[Flags]
public enum OpenDialogAttributeOptions
{
    None = 0,
    AllowMultipleSelection = 1 << 0,
}

public enum PathOpenMode
{ 
    File,
 
    Directory,
 
    Mixed
}

[AttributeUsage(AttributeTargets.Property)]
public class OpenDialogAttribute : System.Attribute
{
    public PathOpenMode Mode { get; }
    public OpenDialogAttributeOptions Options { get; }
    public string? Filters { get; }

    public OpenDialogAttribute(
        PathOpenMode mode = PathOpenMode.File,
        OpenDialogAttributeOptions options = OpenDialogAttributeOptions.None,
        string? filters = null)
    {
        Mode = mode;
        Options = options;
        Filters = filters;
    }
}