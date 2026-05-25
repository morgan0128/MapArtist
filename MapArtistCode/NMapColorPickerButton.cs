using BaseLib.Patches.Localization;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MapArtist.MapArtistCode;

[ScriptPath("res://MapArtistCode/NMapColorPickerButton.cs")]
public partial class NMapColorPickerButton : Control
{
    private const string MapScreenScenePath = "res://scenes/screens/map/map_screen.tscn";

    public static readonly AddedNode<NButton, NMapColorPickerButton> Node =
        new(_mapColorPickerButton =>
        {
            var mapScreen = ResourceLoader.Load<PackedScene>(MapScreenScenePath).GetLocalScene();
            var drawingTools = mapScreen.GetNode<NinePatchRect>("DrawingTools");
            var hbox = mapScreen.GetNode<HBoxContainer>("DrawingTools/HBoxContainer");
                   
            
            var nButton = new NMapColorPickerButton {
                Name = "_mapColorPickerButton",
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            nButton.SetUniqueNameInOwner(true);

            // Match the existing drawing buttons.
            nButton.CustomMinimumSize = new Vector2(60, 60);
            nButton.LayoutMode = 2;
            nButton.FocusMode = Control.FocusModeEnum.All;


            // Put it after ClearButton.
            hbox.AddChild(nButton);

            // Focus navigation.

            var clearButton = drawingTools.GetNode<NButton>("ClearButton");

            clearButton.FocusNeighborRight = new NodePath("../ColorButton");
            nButton.FocusNeighborLeft = new NodePath("../ClearButton");

            // Optional: expand the background/container because the original
            // was sized exactly for 3 buttons: 60 + 60 + 68 = 188.
            //drawingTools.OffsetRight += 60;
            //hbox.OffsetLeft -= 30;
            //hbox.OffsetRight += 30;

            // var clearIcon = clearButton.GetChild<TextureRect>(0, true);

    
            var icon = new TextureRect();
            nButton.AddChild(icon);
            icon.SelfModulate = new Color(1, 1, 1, 0.501961f);
            // icon.SetMaterial(clearIcon.GetMaterial());
            icon.SetUseParentMaterial(true);
            icon.LayoutMode = 1;
            icon.AnchorsPreset = 15;
            icon.AnchorRight = 1.0f;
            icon.AnchorBottom = 1.0f;
            icon.GrowHorizontal = GrowDirection.Both;
            icon.GrowVertical = GrowDirection.Both;
            icon.Scale = new Vector2(1.1f, 1.1f);
            icon.PivotOffset = new Vector2(30, 30);
            icon.MouseFilter = MouseFilterEnum.Ignore;
            // icon.SetTexture(clearIcon.GetTexture());
            icon.SetUseParentMaterial(true);
            icon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
            
            return nButton;
        });

    /*{
    //SimpleLoc.EnableSimpleLoc("MapArtist");

    var mapScreen = ResourceLoader.Load<PackedScene>(MapScreenScenePath).GetLocalScene();
    var drawingTools = mapScreen.GetNode<NinePatchRect>("DrawingTools");
    var hbox = mapScreen.GetNode<HBoxContainer>("DrawingTools/HBoxContainer");



    var nButton = new NMapColorPickerButton {

        Name = "_mapColorPickerButton",
        MouseFilter = Control.MouseFilterEnum.Ignore
    };
    nButton.SetUniqueNameInOwner(true);

    // Match the existing drawing buttons.
    nButton.CustomMinimumSize = new Vector2(60, 60);
    nButton.LayoutMode = 2;
    nButton.FocusMode = Control.FocusModeEnum.All;


    // Put it after ClearButton.
    hbox.AddChild(nButton);

    // Focus navigation.

    var clearButton = drawingTools.GetNode<NButton>("ClearButton");

    clearButton.FocusNeighborRight = new NodePath("../ColorButton");
    nButton.FocusNeighborLeft = new NodePath("../ClearButton");

    // Optional: expand the background/container because the original
    // was sized exactly for 3 buttons: 60 + 60 + 68 = 188.
    //drawingTools.OffsetRight += 60;
    //hbox.OffsetLeft -= 30;
    //hbox.OffsetRight += 30;

    // var clearIcon = clearButton.GetChild<TextureRect>(0, true);
    return nButton;


});

/*
    var icon = new TextureRect();
    colorButton.AddChild(icon);
    icon.SelfModulate = new Color(1, 1, 1, 0.501961f);
    // icon.SetMaterial(clearIcon.GetMaterial());
    icon.SetUseParentMaterial(true);
    icon.LayoutMode = 1;
    icon.AnchorsPreset = 15;
    icon.AnchorRight = 1.0f;
    icon.AnchorBottom = 1.0f;
    icon.GrowHorizontal = GrowDirection.Both;
    icon.GrowVertical = GrowDirection.Both;
    icon.Scale = new Vector2(1.1f, 1.1f);
    icon.PivotOffset = new Vector2(30, 30);
    icon.MouseFilter = MouseFilterEnum.Ignore;
    // icon.SetTexture(clearIcon.GetTexture());
    icon.SetUseParentMaterial(true);
    icon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
    icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;

    var vScene = ResourceLoader.Load<PackedScene>(ColorButtonScenePath);
    var v = vScene.Instantiate<Control>();
    v.Name = "Visual";
    v.MouseFilter = Control.MouseFilterEnum.Ignore;
    icon.AddChild(v);
*/

}