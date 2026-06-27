using Godot;
using MapArtist.MapArtistCode.GUI.Items;

namespace MapArtist.MapArtistCode.GUI;

[ScriptPath("res://MapArtistCode/GUI/NMapArtistGUINode.cs")]
public partial class NMapArtistGUINode : VBoxContainer
{
    // Both a row and an item; no container exclusively for this item; first row of the MapArtist GUI container
    private NColorPicker? _rowitemColorPicker;
    
    // Container for buttons row of the MapArtist GUI container
    private HBoxContainer? _rowButtonsContainer;
    
    private NMapArtistApplyButton? _itemApplyButton;
    private NMapArtistResetButton? _itemResetButton;
    
    private NMapArtistBrushWidth? _itemBrushWidthInterface;
    // private NMapArtistBrushWidthButton? _itemBrushWidthButton;
    // private HBoxContainer? _bWidthSliderContainer;
    // private HSlider? _bWidthSlider;
    // private Label? _bWidthLabel;
    
    public NMapArtistGUINode()
    {
      Name = "MapArtistGUI";
      UniqueNameInOwner = true;
      Visible = false; 
      LayoutMode = 2;
      MouseFilter = MouseFilterEnum.Pass;
      SetAnchorsPreset(LayoutPreset.TopLeft);
      GlobalPosition = new Vector2(12f, 158f);
    }
    
    public override void _Ready() {}
    
    public Color GetColorInColorPicker()
    {
        return _rowitemColorPicker.Color;
    }
    
    public void SetColorInColorPicker(Color color)
    {
        _rowitemColorPicker.Color = color;
    }

    public int GetValueBrushWidth()
    {
        return _itemBrushWidthInterface.BrushWidth;
    }

    public void ResetBrushWidth()
    {
        _itemBrushWidthInterface.ResetValueBrushWidth(); // changing slider value without Brush width; ValueChanged signal to update BrushWidth
    }



    // public void AddRow(Container row)
    // {
    //     _rows.Add(row);
    // }

    public void AssignRowitemColorPicker(NColorPicker colorPicker)
    {
        _rowitemColorPicker = colorPicker;
        this.AddChild(_rowitemColorPicker);
    }
    
    public void AssignRowButtonsContainer(HBoxContainer container)
    {
        _rowButtonsContainer = container;
        this.AddChild(_rowButtonsContainer);
    }
    
    public HBoxContainer GetRowButtonsContainer()
    {
        return _rowButtonsContainer;
    }

    public void AssignItemApplyButton(NMapArtistApplyButton button)
    {
        _itemApplyButton = button;
        _rowButtonsContainer.AddChild(_itemApplyButton);
        _itemApplyButton.MapArtistButtonContainer = _rowButtonsContainer;

    }

    public void AssignItemResetButton(NMapArtistResetButton button)
    {
        _itemResetButton = button;
        _rowButtonsContainer.AddChild(_itemResetButton);
        _itemResetButton.MapArtistButtonContainer = _rowButtonsContainer;
    }
    
    public void AssignItemBrushWidthInterface(NMapArtistBrushWidth brushWidth)
    {
        _itemBrushWidthInterface = brushWidth;
        _rowButtonsContainer.AddChild(_itemBrushWidthInterface);
        
        // _itemBrushWidthInterface.SetButtonAdjustSeparationOffset()
        
        
        // do not use        
        // _bWidthSliderContainer.Position = new Vector2((_itemBrushWidthButton.Size.X + 7f), 0f);

    }
    
}