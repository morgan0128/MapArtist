using Godot;
using MapArtist.MapArtistCode.GUI.Items;
using MapArtist.MapArtistCode.GUI.Items.Abstract;

namespace MapArtist.MapArtistCode.GUI;

[ScriptPath("res://MapArtistCode/GUI/NMapArtistGui.cs")]
public partial class NMapArtistGui : NMapArtistBoxContainerItem
{
    // private NColorPickerItem? _itemColorPicker;
    
    public NMapArtistGui()
    {
        Vertical = true;
        Name = "MapArtistGUI";
        UniqueNameInOwner = true;
        Visible = false; 
        LayoutMode = 2;
        MouseFilter = MouseFilterEnum.Pass;
        SetAnchorsPreset(LayoutPreset.TopLeft);
        AddThemeConstantOverride("separation", 0);
    }
    
    public override void _Ready() {}
    
    public void AddItemColorPicker(NColorPickerItem colorPicker)
    {
        if (AssignedColorPicker()) return; // may have only one color picker
        AddItem(colorPicker);
    }
    public void AddItemColorPicker(int index, NColorPickerItem colorPicker)
    {
        if (AssignedColorPicker()) return; // may have only one color picker
        AddItem(index, colorPicker);
    }

    private NColorPickerItem? FetchColorPickerItem()
    {
        if (ChildItems == null) return null;
        foreach (var ctrl in ChildItems)
        {
            if (ctrl.GetType() == typeof(NColorPickerItem))
            {
                return (NColorPickerItem)ctrl;
            }
        }

        return null;
    }
    
    public bool AssignedColorPicker()
    {
        return (FetchColorPickerItem() != null);
    }

    public void SetColorSamplerVisible(bool visible)
    {
        var colorPicker = FetchColorPickerItem();
        if (colorPicker == null) return;
        colorPicker.SamplerVisible = visible;
    }

    public void SetColorInColorPicker(Color color)
    {
        if (!AssignedColorPicker()) return;
        FetchColorPickerItem()!.Color = color;
        MapArtistController.MapArtistController.Instance.SelectedColor = color;
    }
    
    // public void ResetWidthInWidthItem()
    // {
    //     var widthItem = FetchFirstBrushWidthItem();
    //     if (widthItem == null) return;
    //     widthItem.ResetWidth();
    //     MapArtistController.MapArtistController.Instance.SelectedWidth = widthItem.BrushWidth;
    // }
    
    // private NMapArtistBrushWidthItem? FetchFirstBrushWidthItem()
    // {
    //     var c = FetchFirstItemByType(typeof(NMapArtistBrushWidthItem));
    //     if (c == null) return null;
    //     return (NMapArtistBrushWidthItem)c;
    // }
    
}