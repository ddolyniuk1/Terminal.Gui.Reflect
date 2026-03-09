using System.ComponentModel;
using System.Reflection;
using Terminal.Gui.Reflect.Attributes;
using Terminal.Gui.Reflect.TerminalGuiComponents;

namespace Terminal.Gui.Reflect;

public class ReflectedView : FrameView
{
    private readonly object _boundModel;
    protected virtual string DefaultCategory => "General";

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
           .OrderBy(c => (categoryLayoutDefinitions.GetValueOrDefault(c) ?? new CategoryLayoutAttribute(c)).Order)
           .ToList();

        var uniformGrid = new UniformGrid(getLayoutAttribute.MaxRows, getLayoutAttribute.MaxColumns);
        uniformGrid.Width  = Dim.Fill();
        uniformGrid.Height = Dim.Fill();
        
        var hasCategories = categoryPropertiesOrdered.Any(t => t != DefaultCategory);
        
        foreach (var category in categoryPropertiesOrdered)
        {
            if (!categoryLayoutDefinitions.TryGetValue(category, out var metadata))
            {
                metadata = new CategoryLayoutAttribute(category);
            }

            _categoryViewDictionary[category] = BuildAndAddCategoryView(uniformGrid, hasCategories, category, metadata);
        }

        Add(uniformGrid);
    }

    protected virtual View GetCategoryView(string category)
    {
        return _categoryViewDictionary.GetValueOrDefault(category) ??
               _categoryViewDictionary.GetValueOrDefault(DefaultCategory)!;
    }

    protected virtual View BuildAndAddCategoryView(View parent, bool hasCategories, string category, params object[] metadata)
    {
        View categoryView;
       

        if (hasCategories)
        {
            categoryView = new FrameView();
            categoryView.Title  = category;
        }
        else
        {
            categoryView = new View();
        }
        
        categoryView.CanFocus = true;
        categoryView.Width  = Dim.Fill();
        categoryView.Height = Dim.Fill();
        
        UniformGrid uniformGrid;

        var layoutDefinition = metadata.OfType<CategoryLayoutAttribute>().FirstOrDefault();

        if (layoutDefinition != null)
        {
            uniformGrid        = new UniformGrid(layoutDefinition.MaxRows, layoutDefinition.MaxColumns);
            uniformGrid.Width  = Dim.Fill();
            uniformGrid.Height = Dim.Fill();

            categoryView.Add(uniformGrid);
        }
        else
        {
            uniformGrid = new UniformGrid();
            uniformGrid.Width  = Dim.Fill();
            uniformGrid.Height = Dim.Fill();
            
            categoryView.Add(uniformGrid);
        }

        parent.Add(categoryView);

        return uniformGrid;
    }

    protected virtual IEnumerable<string> GetCategories()
    {
        var registry = new PropertyEditorRegistry();
        return _boundModel.GetType().GetProperties()
            .Where(p => registry.Resolve(p) != null)
            .Select(p => p.GetCustomAttribute<CategoryAttribute>()?.Category ?? DefaultCategory)
            .Distinct()
            .ToList();
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
            var propertyEditor = new PropertyEditorRegistry().Resolve(property);

            if (propertyEditor == null)
            {
                continue;
            }

            var category = property.GetCustomAttribute<CategoryAttribute>()?.Category ?? DefaultCategory;

            var categoryView = GetCategoryView(category);

            var editorView = propertyEditor.Render(this, _boundModel, property);
            categoryView.Add(editorView);
        }
    }
}