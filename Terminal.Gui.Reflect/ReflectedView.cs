using System.ComponentModel;
using System.Reflection;
using Terminal.Gui.Reflect.Attributes;
using Terminal.Gui.Reflect.TerminalGuiComponents;

namespace Terminal.Gui.Reflect;

public class ReflectedView : FrameView
{
    private readonly object _boundModel;

    private readonly Dictionary<string, View> _categoryViewDictionary = new();

    public ReflectedView(object boundModel)
    {
        _boundModel = boundModel;
        Initialize();
    }

    private void Initialize()
    { 
        InitializeCategories();
        InitializeEditors();
    }

    protected virtual void InitializeCategories()
    {
        var getLayoutAttribute = _boundModel.GetType().GetCustomAttribute<LayoutAttribute>() ?? new LayoutAttribute();

        var categoryLayoutDefinitions = GetCategoryLayoutDefinitionLookup();
        var categoryPropertiesOrdered = GetCategories()
           .OrderBy(c => (categoryLayoutDefinitions.GetValueOrDefault(c) ?? new CategoryLayoutAttribute(c)).Order);

        var uniformGrid = new UniformGrid(getLayoutAttribute.MaxRows, getLayoutAttribute.MaxColumns);
        uniformGrid.Width  = Dim.Fill();
        uniformGrid.Height = Dim.Fill();

        foreach (var category in categoryPropertiesOrdered)
        {
            if (!categoryLayoutDefinitions.TryGetValue(category, out var metadata))
            {
                metadata = new CategoryLayoutAttribute(category);
            }

            _categoryViewDictionary[category] = BuildAndAddCategoryView(uniformGrid, category, metadata);
        }

        Add(uniformGrid);
    }

    protected virtual View GetCategoryView(string category)
    {
        return _categoryViewDictionary.GetValueOrDefault(category) ??
               _categoryViewDictionary.GetValueOrDefault("General")!;
    }

    protected virtual View BuildAndAddCategoryView(View parent, string category, params object[] metadata)
    {
        var frameView = new FrameView();
        frameView.Width  = Dim.Fill();
        frameView.Height = Dim.Fill();
        frameView.Title  = category;

        UniformGrid uniformGrid;

        var layoutDefinition = metadata.OfType<CategoryLayoutAttribute>().FirstOrDefault();

        if (layoutDefinition != null)
        {
            uniformGrid        = new UniformGrid(layoutDefinition.MaxRows, layoutDefinition.MaxColumns);
            uniformGrid.Width  = Dim.Fill();
            uniformGrid.Height = Dim.Fill();

            frameView.Add(uniformGrid);
        }
        else
        {
            uniformGrid = new UniformGrid();
            frameView.Add(uniformGrid);
        }

        parent.Add(frameView);

        return uniformGrid;
    }

    protected virtual IEnumerable<string> GetCategories()
    {
        return _boundModel.GetType().GetProperties()
                          .Select(t => t.GetCustomAttribute<CategoryAttribute>()?.Category ??
                                       "General").Distinct().ToList();
    }

    protected virtual Dictionary<string, CategoryLayoutAttribute> GetCategoryLayoutDefinitionLookup()
    {
        return _boundModel.GetType().GetCustomAttributes<CategoryLayoutAttribute>()
                          .ToLookup(t => t.Category)
                          .ToDictionary(t => t.Key, t => t.FirstOrDefault() ?? new CategoryLayoutAttribute("General"));
    }
    
    protected virtual void InitializeEditors()
    {
        var type = _boundModel.GetType();

        var displayNameAttribute = type.GetCustomAttribute<DisplayNameAttribute>();
        if (displayNameAttribute != null)
        {
            Title = displayNameAttribute?.DisplayName ?? type.Name;
        }

        foreach (var property in type.GetProperties())
        {
            var propertyEditor = ReflectedEditorRegistry.GetPropertyEditor(property.PropertyType);

            if (propertyEditor == null)
            {
                continue;
            }

            var category = property.GetCustomAttribute<CategoryAttribute>()?.Category ?? "General";

            var categoryView = GetCategoryView(category);

            var editorView = propertyEditor.Render(this, _boundModel, property);
            categoryView.Add(editorView);
        }
    }
}