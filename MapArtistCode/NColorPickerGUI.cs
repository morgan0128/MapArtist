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

[ScriptPath("res://MapArtistCode/NColorPickerGUI.cs")]
public partial class NColorPickerGUI : ColorPicker
{
    
    // [Signal]
    // public delegate void DisplayGUIEventHandler();
    
    // [Signal]
    // public delegate void HideGUIEventHandler();

    private static NColorPickerGUI instance;
    
    private NColorPickerGUI()
    {
        Name = "ColorPickerGUI";
        UniqueNameInOwner = true;
        // CustomMinimumSize = new Vector2(68, 60);
        // LayoutMode = 2;
        FocusMode = FocusModeEnum.All;
        Visible = true;
        instance = this;
    }

    public static void displayGUI()
    {
        var colorPickerGUI = ResourceLoader.Load<PackedScene>("res://scenes/screens/map/map_screen.tscn").Instantiate().
            GetNode<NColorPickerGUI>("ColorPickerGUI");
        
        
        if (!colorPickerGUI.IsVisible())
        {
            colorPickerGUI.Visible = true;
        }
    }
    
    public static void hideGUI()
    {
        var colorPickerGUI = ResourceLoader.Load<PackedScene>("res://scenes/screens/map/map_screen.tscn").Instantiate().
            GetNode<NColorPickerGUI>("ColorPickerGUI");
        
        if (colorPickerGUI.IsVisible())
        {
            colorPickerGUI.Visible = false;
        }
    }

    // public override void _Ready()
    // {
    //     // Initialize Signal events
    //     // DisplayGUI += () => Visible = true;
    //     // HideGUI += () => Visible = false;
    //     // NMapColorPickerButton colorPickerButton = GetNode<NMapColorPickerButton>("ColorPickerButton");
    // }
    
    public static readonly AddedNode<NMapScreen, NColorPickerGUI> Map = new((mapScreen) =>
    {
        // grab drawing tools display container node
        // var parent = mapScreen.GetNode<NinePatchRect>("DrawingTools");
        
        // initialize color picker gui node
        var gui = new NColorPickerGUI();
        // gui.SetGlobalPosition(new Vector2(50f, 50f));
        
        // add this node to the drawing tools container
        // parent.AddChild(gui);
        // mapScreen.AddChild(gui);
        
        // return the newly created color picker gui
        return gui;
    });
    
    
}