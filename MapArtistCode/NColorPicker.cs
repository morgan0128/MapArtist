namespace MapArtist.MapArtistCode;

using Godot;

[ScriptPath("res://MapArtistCode/NColorPicker.cs")]
public partial class NColorPicker : ColorPicker
{
    public NColorPicker()
    {
        Name = "NColorPicker";
        UniqueNameInOwner = true;

        FocusMode = FocusModeEnum.All;
        GlobalPosition = new Vector2(200f, 200f);
        
        InitRestrictiveDefaultSettings();
    }

    // For a cleaner gui with fewer levers. Allow this to be toggleable in mod config, but set this as the default.
    private void InitRestrictiveDefaultSettings()
    {
        CanAddSwatches = false;
        ColorModesVisible = false;
        EditAlpha = false;
        EditIntensity = false;
        PresetsVisible = false;
        SlidersVisible = false;
        PresetsVisible = false;
        SamplerVisible = false;
        Alignment = AlignmentMode.Begin;
    }
    
}
