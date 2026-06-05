using MegaCrit.Sts2.Core.Entities.Players;

namespace MapArtist.MapArtistCode;

using System.ComponentModel;
using BaseLib.Abstracts;
using BaseLib.Patches.Localization;
using BaseLib.Utils;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://MapArtistCode/NColorPicker.cs")]
public partial class NColorPicker : ColorPicker
{
    
    private static readonly StringName HoverTipImagePath = "res://images/ui/hover_tip.png";

    private static NColorPicker instance;

    public static NColorPicker Instance()
    {
        return instance;
    }
    
    private NColorPicker()
    {
        Name = "NColorPicker";
        UniqueNameInOwner = true;

        FocusMode = FocusModeEnum.All;
        GlobalPosition = new Vector2(200f, 200f);
        // Visible = false;
        instance = this;
        
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
