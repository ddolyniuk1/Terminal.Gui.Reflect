using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Terminal.Gui.Drawing;
using Terminal.Gui.Reflect.Attributes;
using Terminal.Gui.Reflect.Base;
using Terminal.Gui.Reflect.Settings;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Terminal.Gui.Reflect.Views;

public class PropertyGrid : FrameView
{
    private readonly object _boundModel;
    protected virtual string DefaultCategory => "General";

    private readonly Dictionary<string, View> _categoryViewDictionary = new();
    private readonly PropertyGridSettings _settings;

    public PropertyGrid(object boundModel, PropertyGridSettings? settings = null)
    {
        _settings = settings ?? new PropertyGridSettings();
        _boundModel = boundModel ?? throw new ArgumentNullException(nameof(boundModel));
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

        var uniformGrid = new UniformGridA(getLayoutAttribute.MaxColumns);
        // uniformGrid.Height = 600;
        uniformGrid.Height = Dim.Absolute(200);
        uniformGrid.ForcedHeight = 200;
        uniformGrid.VerticalScrollBar.VisibilityMode = ScrollBarVisibilityMode.Always;
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

        categoryView.Margin!.Thickness = _settings.CategoryMargin;
        categoryView.Padding!.Thickness = _settings.CategoryPadding;
        
        UniformGridA flexGrid;

        var layoutDefinition = metadata.OfType<CategoryLayoutAttribute>().FirstOrDefault();

        if (layoutDefinition != null)
        {
            flexGrid = new UniformGridA(layoutDefinition.MaxColumns);

            categoryView.Add(flexGrid);
        }
        else
        {
            flexGrid = new UniformGridA(1);
            
            categoryView.Add(flexGrid);
        }

        parent.Add(categoryView);

        return flexGrid;
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
        var displayAttribute = type.GetCustomAttribute<DisplayAttribute>();
        if (displayNameAttribute != null || displayAttribute != null)
        {
            if (_settings.ShowTitle)
            {
                Title = displayNameAttribute?.DisplayName ?? displayAttribute?.Name ?? type.Name;  
            }
        }
        
        if (!_settings.ShowBorder)
        {
            BorderStyle = LineStyle.None;
        }

        Margin!.Thickness = _settings.Margin;
        Padding!.Thickness = _settings.Padding;

        foreach (var property in type.GetProperties())
        {
            var propertyEditor = new PropertyEditorRegistry().Resolve(property);

            if (propertyEditor == null)
            {
                continue;
            }

            var category = property.GetCustomAttribute<CategoryAttribute>()?.Category ?? DefaultCategory;

            var categoryView = GetCategoryView(category);

            var editorView = propertyEditor.Render(this, _boundModel, property, _settings);
            categoryView.Add(editorView);
        }
    }
}