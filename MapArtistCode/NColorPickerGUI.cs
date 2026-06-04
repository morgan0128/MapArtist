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

[ScriptPath("res://MapArtistCode/NColorPickerGUI.cs")]
public partial class NColorPickerGUI : ColorPicker
{
    
    private static readonly StringName HoverTipImagePath = "res://images/ui/hover_tip.png";

    private static NColorPickerGUI instance;
    
    private NColorPickerGUI()
    {
        Name = "ColorPickerGUI";
        UniqueNameInOwner = true;
        // CustomMinimumSize = new Vector2(68, 60);
        // LayoutMode = 2;
        FocusMode = FocusModeEnum.All;
        GlobalPosition = new Vector2(200f, 200f);
        Visible = false;
        instance = this;


        // var r = new Resource();
        // r.ResourcePath = HoverTipImagePath;
        // r.ResourceName = "bg";
        // r.ResourceLocalToScene = true;



        // ------ test ------
        
        // this.TextureRepeat = TextureRepeatEnum.Disabled;
        // this.ClipChildren = ClipChildrenMode.AndDraw;
        // this.ClipContents = true;

        var sbt = new StyleBoxTexture();
        sbt.AxisStretchHorizontal = StyleBoxTexture.AxisStretchMode.Stretch;
        sbt.AxisStretchVertical = StyleBoxTexture.AxisStretchMode.Stretch;
        sbt.DrawCenter = true;
        sbt.RegionRect = new Rect2(0, 0, 0, 0);
        sbt.Texture = PreloadManager.Cache.GetTexture2D((string) HoverTipImagePath);
        sbt.SetName("color_picker_sbt");
        
        var t = new Theme();

        t.AddType("color_picker_theme");

        t.SetStylebox("color_picker_bg", "color_picker_theme", sbt);
        // this.AddThemeStyleboxOverride("color_picker_background", sbt);
        // this.AddThemeIconOverride("color_picker_icon", sbt.Texture);
        this.Theme = t;
        
        // ------------
        
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

    public static void displayGUI()
    {
        if (!instance.IsVisible())
        {
            instance.Visible = true;
        }
    }
    
    public static void hideGUI()
    {
        if (instance.IsVisible())
        {
            instance.Visible = false;
        }
    }
    
    public static void toggleGUI(Player? localPlayer)
    {
        if (instance.IsVisible())
        {
            // instance._selectedColor = instance.Color;
            MapArtistDrawingColors.Set(localPlayer, instance.Color);
        }
        instance.Visible = !instance.Visible;
        
    }
    
    public static readonly AddedNode<NMapScreen, NColorPickerGUI> Map = new((mapScreen) =>
    {
        // grab drawing tools display container node
        // var parent = mapScreen.GetNode<NinePatchRect>("DrawingTools");
        
        // initialize color picker gui node
        var gui = new NColorPickerGUI();
        // gui.SetGlobalPosition(new Vector2(200f, 200f));
        
        // add this node to the drawing tools container
        // parent.AddChild(gui);
        // mapScreen.AddChild(gui);
        
        // return the newly created color picker gui
        return gui;
    });
    
    
}
