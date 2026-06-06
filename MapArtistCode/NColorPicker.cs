namespace MapArtist.MapArtistCode;

using Godot;

[ScriptPath("res://MapArtistCode/NColorPicker.cs")]
public partial class NColorPicker : ColorPicker
{
    
    // private static readonly StringName HoverTipImagePath = "res://images/ui/hover_tip.png";

    // public static NColorPicker Instance { get; } = new NColorPicker();

    // public static NColorPicker? Instance()
    // {
    //     return instance;
    // }
    
    public NColorPicker()
    {
        Name = "NColorPicker";
        UniqueNameInOwner = true;

        FocusMode = FocusModeEnum.All;
        GlobalPosition = new Vector2(200f, 200f);
        // Visible = false;
        // instance = this;
        
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

    // public static void displayGUI()
    // {
    //     if (!instance.IsVisible())
    //     {
    //         instance.Visible = true;
    //     }
    // }
    //
    // public static void hideGUI()
    // {
    //     if (instance.IsVisible())
    //     {
    //         instance.Visible = false;
    //     }
    // }
    //
    // public static void toggleGUI(Player? localPlayer)
    // {
    //     if (instance.IsVisible())
    //     {
    //         MapArtistDrawingColors.Set(localPlayer, instance.Color);
    //     }
    //     instance.Visible = !instance.Visible;
    //     
    // }
    
    // public static readonly AddedNode<NMapScreen, NColorPicker> Map = new((mapScreen) =>
    // {
    //     // grab drawing tools display container node
    //     // var parent = mapScreen.GetNode<NinePatchRect>("DrawingTools");
    //     
    //     // initialize color picker gui node
    //     var gui = new NColorPicker();
    //     // gui.SetGlobalPosition(new Vector2(200f, 200f));
    //     
    //     // add this node to the drawing tools container
    //     // parent.AddChild(gui);
    //     // mapScreen.AddChild(gui);
    //     
    //     // return the newly created color picker gui
    //     return gui;
    // });
    
    
}
