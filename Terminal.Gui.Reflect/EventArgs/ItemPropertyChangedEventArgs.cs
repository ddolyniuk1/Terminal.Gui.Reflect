using System.ComponentModel;

namespace Terminal.Gui.Reflect.EventArgs;

public sealed class ItemPropertyChangedEventArgs<T> : PropertyChangedEventArgs
{
    public ItemPropertyChangedEventArgs(T item, string? propertyName)
        : base(propertyName)
    {
        Item = item;
    }

    /// <summary>The item whose property changed.</summary>
    public T Item { get; }
}