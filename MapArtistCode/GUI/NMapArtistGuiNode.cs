using Godot;
using MapArtist.MapArtistCode.GUI.Items;

namespace MapArtist.MapArtistCode.GUI;

[ScriptPath("res://MapArtistCode/GUI/NMapArtistGuiNode.cs")]
public partial class NMapArtistGuiNode : VBoxContainer
{
    // Both a row and an item; no container exclusively for this item; first row of the MapArtist GUI container
    private NColorPicker? _rowitemColorPicker;
    
    // Container for buttons row of the MapArtist GUI container
    private HBoxContainer? _rowButtonsContainer;
    
    private NMapArtistApplyButton? _itemApplyButton;
    private NMapArtistResetButton? _itemResetButton;
    private NMapArtistBrushWidth? _itemBrushWidthInterface;

    
    public NMapArtistGuiNode()
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
        return _rowitemColorPicker?.Color ?? Colors.White;
    }
    
    public void SetColorInColorPicker(Color color)
    {
        if (_rowitemColorPicker == null) return;
        _rowitemColorPicker.Color = color;
    }

    public int GetValueBrushWidth()
    {
        return _itemBrushWidthInterface?.BrushWidth ?? Util.DefaultBrushWidth;
    }

    public void ResetBrushWidth()
    {
        _itemBrushWidthInterface?.ResetValueBrushWidth(); // changing slider value without Brush width; ValueChanged signal to update BrushWidth
    }
    
    public void AssignRowitemColorPicker(NColorPicker colorPicker)
    {
        _rowitemColorPicker = colorPicker;
        AddChild(_rowitemColorPicker);
    }
    
    public void AssignRowButtonsContainer(HBoxContainer container)
    {
        _rowButtonsContainer = container;
        AddChild(_rowButtonsContainer);
    }

    public void AssignItemApplyButton(NMapArtistApplyButton button)
    {
        _itemApplyButton = button;
        
        if (_rowButtonsContainer == null) return;
        _rowButtonsContainer.AddChild(_itemApplyButton);
        _itemApplyButton.MapArtistButtonContainer = _rowButtonsContainer;

    }

    public void AssignItemResetButton(NMapArtistResetButton button)
    {
        _itemResetButton = button;
        
        if (_rowButtonsContainer == null) return;
        _rowButtonsContainer.AddChild(_itemResetButton);
        _itemResetButton.MapArtistButtonContainer = _rowButtonsContainer;
    }
    
    public void AssignItemBrushWidthInterface(NMapArtistBrushWidth brushWidth)
    {
        _itemBrushWidthInterface = brushWidth;
        _rowButtonsContainer?.AddChild(_itemBrushWidthInterface);
    }
    
}