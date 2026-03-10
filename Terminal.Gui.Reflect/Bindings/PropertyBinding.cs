using System.ComponentModel;
using System.Reflection;

namespace Terminal.Gui.Reflect.Bindings
{
    /// <summary>
    /// Two-way binding between a model property and ANY control.
    /// 
    /// Supply delegates for reading/writing the UI value. Wire up the
    /// UI-changed event yourself, then call <see cref="PushToModel"/> from it.
    ///
    /// Example — checkbox:
    /// <code>
    ///   var binding = new PropertyBinding&lt;bool&gt;(model, property,
    ///       uiSetter: v => checkbox.CheckedState = v ? CheckState.Checked : CheckState.UnChecked);
    ///
    ///   checkbox.Toggle += (_, _) =>
    ///       binding.PushToModel(() => checkbox.CheckedState == CheckState.Checked);
    ///
    ///   view.Removed += (_, _) => binding.Dispose();
    /// </code>
    ///
    /// Example — text field:
    /// <code>
    ///   var binding = new PropertyBinding&lt;string&gt;(model, property,
    ///       uiSetter: v => textField.Text = v ?? string.Empty);
    ///
    ///   textField.TextChanging += (_, e) =>
    ///       binding.PushToModel(() => e.NewValue);
    ///
    ///   view.Removed += (_, _) => binding.Dispose();
    /// </code>
    /// </summary>
    public sealed class PropertyBinding<T> : IDisposable
    {
        private readonly object       _model;
        private readonly PropertyInfo _property;
        private readonly Action<T?>   _uiSetter;
        private          bool         _updating;

        /// <summary>Raised after the model value is successfully updated.</summary>
        public event Action<T?>? ValueChanged;

        public PropertyBinding(object model, PropertyInfo property, Action<T?> uiSetter)
        {
            _model    = model;
            _property = property;
            _uiSetter = uiSetter;

            // Initialise UI from current model value
            _uiSetter(GetModelValue());

            if (model is INotifyPropertyChanged inpc)
                inpc.PropertyChanged += OnModelPropertyChanged;
        }

        /// <summary>
        /// Call from a UI-changed event to push the current UI value into the model.
        /// Skips if a model→UI update is already in progress (prevents loops).
        /// </summary>
        public void PushToModel(Func<T?> uiGetter)
        {
            if (_updating) return;
            _updating = true;
            try
            {
                var uiValue = uiGetter();
                _property.SetValue(_model, CoerceToPropertyType(uiValue));
                ValueChanged?.Invoke(uiValue);
                return;
            }
            finally { _updating = false; }
        }

        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _property.Name || _updating) return;
            _updating = true;
            try   { _uiSetter(GetModelValue()); }
            finally { _updating = false; }
        }

        private T? GetModelValue()
        {
            var raw = _property.GetValue(_model);
            if (raw is null)    return default;
            if (raw is T typed) return typed;
            try { return (T?)Convert.ChangeType(raw, typeof(T)); }
            catch { return default; }
        }

        /// <summary>
        /// Handles the common case where T is string but the property is int/decimal/etc.
        /// </summary>
        private object? CoerceToPropertyType(T? value)
        {
            if (value is null) return null;
            var target = Nullable.GetUnderlyingType(_property.PropertyType) ?? _property.PropertyType;
            if (value.GetType() == target) return value;
            return Convert.ChangeType(value, target);
        }

        public void Dispose()
        {
            if (_model is INotifyPropertyChanged inpc)
                inpc.PropertyChanged -= OnModelPropertyChanged;
        }
    }
}