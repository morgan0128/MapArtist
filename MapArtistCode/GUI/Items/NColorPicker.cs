using Godot;
using MapArtist.MapArtistCode.Config;

namespace MapArtist.MapArtistCode.GUI.Items;

[ScriptPath("res://MapArtistCode/GUI/Items/NColorPicker.cs")]
public partial class NColorPicker : ColorPicker
{
    private Tween? _tween;
    
    public NColorPicker()
    {
        Name = "NColorPicker";
        UniqueNameInOwner = true;

        FocusMode = FocusModeEnum.All;
        GlobalPosition = new Vector2(200f, 200f);
        EditAlpha = false;
        DeferredMode = true;
        
        InitRestrictiveDefaultSettings();
        SamplerVisible = MapArtistConfig.ColorSamplerTool;
    }

    // For a cleaner gui with fewer levers. Allow this to be toggleable in mod config, but set this as the default.
    private void InitRestrictiveDefaultSettings()
    {
        CanAddSwatches = false;
        ColorModesVisible = false;
        // EditAlpha = false; disabled by default (eraser issues)
        EditIntensity = false; // unsafe? test later
        PresetsVisible = false;
        SlidersVisible = false;
        PresetsVisible = false;
        // SamplerVisible = false;
        Alignment = AlignmentMode.Begin;
    }
    
    private void UnsafeEnableEditAlpha()
    {
        EditAlpha = true;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        ColorChanged += OnColorChanged;
    }

    private void OnColorChanged(Color color)
    {
        if (MapArtistConfig.SynchronizedColorPicker)
        {
            MapArtistController.MapArtistController.Instance.ApplySettingColor();
        }
    }
    
}
