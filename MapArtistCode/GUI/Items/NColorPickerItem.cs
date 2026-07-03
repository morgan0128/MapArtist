using Godot;
using MapArtist.MapArtistCode.Config;
using MapArtist.MapArtistCode.GUI.Items.Abstract;

namespace MapArtist.MapArtistCode.GUI.Items;

[ScriptPath("res://MapArtistCode/GUI/Items/NColorPickerItem.cs")]
public partial class NColorPickerItem : NMapArtistItem
{
    private Tween? _tween;
    private ColorPicker _colorPicker;
    public Color Color
    {
        get => _colorPicker.Color;
        set => _colorPicker.Color = value;
    }

    public NColorPickerItem()
    {
        Name = "NColorPickerItem";
        UniqueNameInOwner = true;
        // GlobalPosition = new Vector2(200f, 200f);
        
        NestedControlNodes = new List<Control>();
        _colorPicker = new ColorPicker();
        NestedControlNodes.Add(_colorPicker);
        AddChild(_colorPicker);

        _colorPicker.Name = "NColorPicker";
        _colorPicker.UniqueNameInOwner = true;

        _colorPicker.FocusMode = FocusModeEnum.All;
        _colorPicker.EditAlpha = false;
        _colorPicker.DeferredMode = true;
        
        InitRestrictiveDefaultSettings();
        _colorPicker.SamplerVisible = MapArtistConfig.ColorSamplerTool;
    }

    // For a cleaner gui with fewer levers. Allow this to be toggleable in mod config, but set this as the default.
    private void InitRestrictiveDefaultSettings()
    {
        _colorPicker.CanAddSwatches = false;
        _colorPicker.ColorModesVisible = false;
        // EditAlpha = false; disabled by default (eraser issues)
        _colorPicker.EditIntensity = false; // unsafe? test later
        _colorPicker.PresetsVisible = false;
        _colorPicker.SlidersVisible = false;
        _colorPicker.PresetsVisible = false;
        // SamplerVisible = false;
        _colorPicker.Alignment = BoxContainer.AlignmentMode.Begin;
    }
    
    private void UnsafeEnableEditAlpha()
    {
        _colorPicker.EditAlpha = true;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        _colorPicker.ColorChanged += OnColorChanged;
    }

    private void OnColorChanged(Color color)
    {
        if (MapArtistConfig.SynchronizedColorPicker)
        {
            MapArtistController.MapArtistController.Instance.ApplySettingColor();
        }
    }
    
}
