using System;
using System.Collections.Generic;
using System.Text;
using Terminal.Gui.Reflect.Drawers;

namespace Terminal.Gui.Reflect
{
    public class ReflectedEditorRegistry
    {
        private static readonly Dictionary<Type, PropertyEditorBase> _defaultPropertyEditors = new();

        static ReflectedEditorRegistry()
        {
            RegisterPropertyEditor(typeof(string), new DefaultTextEditor());
            RegisterPropertyEditor(typeof(bool),   new DefaultBooleanEditor());
        }

        public static void RegisterPropertyEditor(Type type, PropertyEditorBase propertyEditor)
        {
            _defaultPropertyEditors[type] = propertyEditor;
        }

        public static PropertyEditorBase? GetPropertyEditor(Type type)
        {
            return _defaultPropertyEditors.GetValueOrDefault(type);
        }
    }
}
