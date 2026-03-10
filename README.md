
# Terminal.Gui.Reflect

This project adds additional views for Terminal.Gui v2 applications. Per project name, the focus is on using reflection to auto-generate the view as well as automatic two-way binding support.


## Acknowledgements

 - [Terminal.Gui](https://github.com/gui-cs/Terminal.Gui)

## Authors

- [@ddolyniuk1](https://www.github.com/ddolyniuk1)


## Features

- PropertyGrid
    - A view that auto generates categories and properties
    - Each field supports validation
- UniformGrid
    - A view similar to WPF's UniformGrid
- InfoLabel
    - A label that shows a tooltip when hovered


## Usage/Examples

### PropertyGrid Example: 
```csharp
// using Terminal.Gui v2
var model = new BasicViewModel();
var settings = new PropertyGridSettings()
{
    ShowBorder = false
};
var propertyGrid = new PropertyGrid(model, settings);
propertyGrid.Width  = Dim.Fill();
propertyGrid.Height = Dim.Fill();
Add(propertyGrid); // add the PropertyGrid to our View
```

### UniformGrid Example
```csharp
var uniformGrid = new UniformGrid(-1, 1); // vertical stacking control

uniformGrid = new UniformGrid(1, -1); // horizontal stacking control

uniformGrid = new UniformGrid(-1, 3); // N rows and up to 3 columns

Add(uniformGrid);

uniformGrid.Add(new Label { Text = "Test" });
uniformGrid.Add(new Label { Text = "Test2" });
uniformGrid.Add(new Label { Text = "Test3" });
```

### InfoLabel Example
```csharp
var infoLabel = new InfoLabel("Some helpful information.");

var label = new Label { Text = "Concise Label: ", X = 0, Y = 0, Width = Dim.Auto(DimAutoStyle.Text), Height = 1 }

infoLabel.X = Pos.Right(label) + 3;
infoLabel.Y = label.Y;

Add(label, infoLabel);
```
