using BaseLib.Patches.Localization;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;

namespace MapArtist.MapArtistCode;

[ScriptPath("res://MapArtistCode/NMapColorPickerButton.cs")]
public partial class NMapColorPickerButton : NButton
{
    
    private static readonly StringName _imagePath = (StringName) "res://images/packed/map/drawing_clear.png";
    private static readonly StringName _glowImagePath = (StringName) "res://images/packed/map/drawing_clear_glow.png";
    // private Control _drawingToolHolder;
    // private static TextureRect _icon;
    // private HoverTip _hoverTip;
    // private Tween? _tween;
    // private static readonly Color _activeColor = new Color("FFE57DFF");
    // private static readonly Color _inactiveColor = new Color("FFFFFF80");

    
    public override void _Ready()
    {
        
    }
    
    // protected override void OnFocus()
    // {
    //     base.OnFocus();
    //     _icon.Texture = PreloadManager.Cache.GetTexture2D((string) NMapColorPickerButton._glowImagePath);
    //     this._tween?.Kill();
    //     this._tween = this.CreateTween().SetParallel();
    //     this._tween.TweenProperty((GodotObject) _icon, (NodePath) "scale", (Variant) (Vector2.One * 1.2f), 0.05);
    //     this._tween.TweenProperty((GodotObject) _icon, (NodePath) "self_modulate", (Variant) NMapColorPickerButton._activeColor, 0.05);
    //     NHoverTipSet.CreateAndShow(this._drawingToolHolder, (IHoverTip) this._hoverTip).GlobalPosition = this._drawingToolHolder.GlobalPosition + new Vector2(10f, -132f);
    // }
    //
    // protected override void OnUnfocus()
    // {
    //     base.OnUnfocus();
    //     _icon.Texture = PreloadManager.Cache.GetTexture2D((string) NMapColorPickerButton._imagePath);
    //     this._tween?.Kill();
    //     this._tween = this.CreateTween().SetParallel();
    //     this._tween.TweenProperty((GodotObject) _icon, (NodePath) "scale", (Variant) (Vector2.One * 1.1f), 0.05);
    //     this._tween.TweenProperty((GodotObject) _icon, (NodePath) "self_modulate", (Variant) NMapColorPickerButton._inactiveColor, 0.05);
    //     NHoverTipSet.Remove(this._drawingToolHolder);
    // }
    
    private const string MapScreenScenePath = "res://scenes/screens/map/map_screen.tscn";

    // private const string HoverGlowPngPath = "res://images/packed/map/drawing_clear_glow.png";

    public static readonly AddedNode<NMapScreen, NMapColorPickerButton> Map = new((mapScreen) =>
    {
        var parent = mapScreen.GetNode<HBoxContainer>("DrawingTools/HBoxContainer");

        var button = new NMapColorPickerButton
        {
            Name = "ColorPickerButton",
            UniqueNameInOwner = true,
            CustomMinimumSize = new Vector2(60, 60),
            LayoutMode = 2,
            FocusMode = FocusModeEnum.All
        };

        parent.AddChild(button);

        // Focus navigation.
        var clearButton = (NButton)parent.GetNode("ClearButton");
        clearButton.FocusNeighborRight = new NodePath("../ColorButton");
        button.FocusNeighborLeft = new NodePath("../ClearButton");

        // Glow on Hover
        // var d = new CompressedTexture2D();
        // d.LoadPath = "res://.godot/imported/drawing_clear_glow.png-2cc1edd02f02941cd8c59ad612a06f04.ctex";
        // button.HoveredSfx = d;

        // Optional: expand the background/container because the original
        // was sized exactly for 3 buttons: 60 + 60 + 68 = 188.
        // parent.OffsetRight += 60;
        // parent.OffsetLeft -= 30;
        // parent.OffsetRight += 30;

        var clearIcon = (TextureRect)clearButton.GetNode("Icon");


        var icon = new TextureRect();
        button.AddChild(icon);
        icon.SelfModulate = new Color(1, 1, 1, 0.501961f);
        icon.SetMaterial(clearIcon.GetMaterial());
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
        icon.SetTexture(clearIcon.GetTexture());
        icon.SetUseParentMaterial(true);
        icon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        button.FocusEntered += (() => {
            icon.Texture = PreloadManager.Cache.GetTexture2D((string)NMapColorPickerButton._glowImagePath);
        });
        

        return button;

    });

    /*
    public static readonly AddedNode<NMapScreen, NMapColorPickerButton> Node =
        new(_mapColorPickerButton =>
        {
            var mapScreen = ResourceLoader.Load<PackedScene>(MapScreenScenePath).Instantiate();
            var drawingTools = (NinePatchRect)mapScreen.GetNode("DrawingTools");
            var hbox = (HBoxContainer)mapScreen.GetNode("DrawingTools/HBoxContainer");


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
            var clearButton = (NButton)drawingTools.GetNode("ClearButton");
            clearButton.FocusNeighborRight = new NodePath("../ColorButton");
            nButton.FocusNeighborLeft = new NodePath("../ClearButton");

            // Optional: expand the background/container because the original
            // was sized exactly for 3 buttons: 60 + 60 + 68 = 188.
            drawingTools.OffsetRight += 60;
            hbox.OffsetLeft -= 30;
            hbox.OffsetRight += 30;

            var clearIcon = (TextureRect)drawingTools.GetNode("Icon");


            var icon = new TextureRect();
            nButton.AddChild(icon);
            icon.SelfModulate = new Color(1, 1, 1, 0.501961f);
            icon.SetMaterial(clearIcon.GetMaterial());
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
            icon.SetTexture(clearIcon.GetTexture());
            icon.SetUseParentMaterial(true);
            icon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;

            return nButton;
        });


*/

}