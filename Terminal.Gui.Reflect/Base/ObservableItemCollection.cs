using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Terminal.Gui.Reflect.EventArgs;

namespace Terminal.Gui.Reflect.Base;

/// <inheritdoc />
/// <summary>
///     An <see cref="T:System.Collections.ObjectModel.ObservableCollection`1">ObservableCollection{T}</see> that forwards <see cref="E:System.ComponentModel.INotifyPropertyChanged.PropertyChanged">INotifyPropertyChanged.PropertyChanged</see>
///     events from its items, so the collection itself raises <see cref="E:Terminal.Gui.Reflect.Base.ObservableItemCollection`1.ItemPropertyChanged">ItemPropertyChanged</see>
///     whenever a contained item changes a property.
/// </summary>
public class ObservableItemCollection<T> : ObservableCollection<T>
    where T : INotifyPropertyChanged
{
    public ObservableItemCollection()
    {
    }

    public ObservableItemCollection(IEnumerable<T> items) : base(items)
    {
        foreach (var item in this)
        {
            item.PropertyChanged += OnItemPropertyChanged;
        }
    }

    public event EventHandler<ItemPropertyChangedEventArgs<T>>? ItemPropertyChanged;

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (T item in e.OldItems)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (T item in e.NewItems)
            {
                item.PropertyChanged += OnItemPropertyChanged;
            }
        }

        base.OnCollectionChanged(e);
    }

    protected override void ClearItems()
    {
        foreach (var item in this)
        {
            item.PropertyChanged -= OnItemPropertyChanged;
        }

        base.ClearItems();
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is T item)
        {
            ItemPropertyChanged?.Invoke(this, new ItemPropertyChangedEventArgs<T>(item, e.PropertyName));
        }
    }
}