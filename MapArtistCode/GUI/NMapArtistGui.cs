using Godot;
using MapArtist.MapArtistCode.GUI.Items;
using MapArtist.MapArtistCode.GUI.Items.Abstract;

namespace MapArtist.MapArtistCode.GUI;

[ScriptPath("res://MapArtistCode/GUI/NMapArtistGui.cs")]
public partial class NMapArtistGui : NMapArtistBoxContainerItem
{
    // Both a row and an item; no container exclusively for this item
    private NColorPickerItem? _itemColorPicker;
    
    // Container for buttons row of the MapArtist GUI container
    // private HBoxContainer? _rowButtonsContainer;
    
    // private NMapArtistApplyButtonItem? _itemApplyButton;
    // private NMapArtistResetButtonItem? _itemResetButton;
    // private NMapArtistBrushWidthItem? _itemBrushWidth;
    
    public NMapArtistGui()
    {
        Vertical = true;
        Name = "MapArtistGUI";
        UniqueNameInOwner = true;
        Visible = false; 
        LayoutMode = 2;
        MouseFilter = MouseFilterEnum.Pass;
        SetAnchorsPreset(LayoutPreset.TopLeft);
        GlobalPosition = new Vector2(12f, 158f);
    }
    
    public override void _Ready() {}
    
    // public Color GetColorInColorPicker()
    // {
    //     return _itemColorPicker?.Color ?? Colors.White;
    // }
    //
    // public void SetColorInColorPicker(Color color)
    // {
    //     if (_itemColorPicker == null) return;
    //     _itemColorPicker.Color = color;
    // }
    //
    // public int GetValueBrushWidth()
    // {
    //     return _itemBrushWidth?.BrushWidth ?? Util.DefaultBrushWidth;
    // }
    //
    // public void ResetBrushWidth()
    // {
    //     _itemBrushWidth?.ResetValueBrushWidth(); // changing slider value without Brush width; ValueChanged signal to update BrushWidth
    // }
    //
    public void AddItemColorPicker(NColorPickerItem colorPicker)
    {
        _itemColorPicker = colorPicker;
        AddChild(_itemColorPicker);
    }
    //
    // public void AssignRowButtonsContainer(HBoxContainer container)
    // {
    //     _rowButtonsContainer = container;
    //     AddChild(_rowButtonsContainer);
    // }
    //
    // public void AssignItemApplyButton(NMapArtistApplyButtonItem button)
    // {
    //     _itemApplyButton = button;
    //     
    //     if (_rowButtonsContainer == null) return;
    //     _rowButtonsContainer.AddChild(_itemApplyButton);
    //     _itemApplyButton.MapArtistButtonContainer = _rowButtonsContainer;
    //
    // }
    //
    // public void AssignItemResetButton(NMapArtistResetButtonItem button)
    // {
    //     _itemResetButton = button;
    //     
    //     if (_rowButtonsContainer == null) return;
    //     _rowButtonsContainer.AddChild(_itemResetButton);
    //     _itemResetButton.MapArtistButtonContainer = _rowButtonsContainer;
    // }
    //
    // public void AssignItemBrushWidthInterface(NMapArtistBrushWidthItem brushWidthItem)
    // {
    //     _itemBrushWidth = brushWidthItem;
    //     _rowButtonsContainer?.AddChild(_itemBrushWidth);
    // }
    
}