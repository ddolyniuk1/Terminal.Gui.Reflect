using System.Reflection;
using Terminal.Gui.Reflect.Drawers;

namespace Terminal.Gui.Reflect
{
    /// <summary>
    /// Maintains an ordered list of <see cref="PropertyEditorBase"/> instances and picks
    /// the first one that says it can handle a given property.
    /// 
    /// Register more specific editors before the fallback <see cref="DefaultTextEditor"/>:
    /// <code>
    ///   var registry = new PropertyEditorRegistry();
    ///   registry.Register(new BoolCheckboxEditor(), priority: 10);
    ///   registry.Register(new DateEditor(),         priority: 5);
    /// </code>
    /// </summary>
    public class PropertyEditorRegistry
    {
        private readonly List<(int Priority, PropertyEditorBase Editor)> _editors = new();

        public PropertyEditorRegistry()
        {
            Register(new BoolCheckboxEditor(), priority: 100);
            Register(new DefaultTextEditor(),  priority: 0);  
        }

        /// <summary>
        /// Registers an editor. Higher priority wins over lower priority.
        /// </summary>
        public void Register(PropertyEditorBase editor, int priority = 1)
        {
            _editors.Add((priority, editor));
            _editors.Sort((a, b) => b.Priority.CompareTo(a.Priority)); // descending
        }

        /// <summary>
        /// Returns the highest-priority editor that can handle the property,
        /// or null if no editor accepts it (e.g. [Browsable(false)]).
        /// </summary>
        public PropertyEditorBase? Resolve(PropertyInfo property) =>
            _editors.Select(e => e.Editor)
                    .FirstOrDefault(e => e.CanHandleProperty(property));
    }
}