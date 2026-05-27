using System.ComponentModel;
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

namespace MapArtist.MapArtistCode;

[ScriptPath("res://MapArtistCode/NMapColorPickerButton.cs")]
public partial class NMapColorPickerButton : NButton
{
    private static Control loadedMapScreen = (Control) ResourceLoader.Load<PackedScene>("res://scenes/screens/map/map_screen.tscn").Instantiate();
    private static HBoxContainer loadedDrawingContainer = loadedMapScreen.GetNode<HBoxContainer>("DrawingTools/HBoxContainer");
    private static NButton loadedClearButton = (NButton)loadedDrawingContainer.GetNode("ClearButton");
    private static TextureRect loadedClearIcon = (TextureRect)loadedClearButton.GetNode("Icon");
        
    
    private bool HasControllerHotkey => this.Hotkeys.Length != 0;
    private static readonly StringName ImagePath = "res://images/packed/map/drawing_clear.png";
    private static readonly StringName GlowImagePath = "res://images/packed/map/drawing_clear_glow.png";
    private Control _drawingToolHolder = loadedDrawingContainer;
    private TextureRect _icon;
    private HoverTip _hoverTip = new HoverTip(new LocString("map", "CLEAR_DRAWING.title"), new LocString("map", "CLEAR_DRAWING.description"));
    private Tween? _tween;
    private static readonly Color ActiveColor = new Color("FFE57DFF");
    private static readonly Color InactiveColor = new Color("FFFFFF80");

    
    private static TextureRect InitIcon()
    {
        var icon = new TextureRect();
        icon.SelfModulate = loadedClearIcon.SelfModulate;
        icon.SetMaterial(loadedClearIcon.GetMaterial());
        icon.SetUseParentMaterial(loadedClearIcon.GetUseParentMaterial());
        icon.LayoutMode = loadedClearIcon.LayoutMode;
        icon.AnchorsPreset = loadedClearIcon.AnchorsPreset;
        icon.AnchorRight = loadedClearIcon.AnchorRight;
        icon.AnchorBottom = loadedClearIcon.AnchorBottom;
        icon.GrowHorizontal = loadedClearIcon.GrowHorizontal;
        icon.GrowVertical = loadedClearIcon.GrowVertical;
        icon.Scale =  new Vector2(loadedClearIcon.Scale.X, loadedClearIcon.Scale.Y);
        icon.PivotOffset = new Vector2(loadedClearIcon.PivotOffset.X, loadedClearIcon.PivotOffset.Y);
        icon.MouseFilter = loadedClearIcon.MouseFilter;
        icon.SetTexture(loadedClearIcon.GetTexture());
        icon.SetUseParentMaterial(loadedClearIcon.GetUseParentMaterial());
        icon.ExpandMode = loadedClearIcon.ExpandMode;
        icon.StretchMode = loadedClearIcon.StretchMode;
        
        return icon;
    }
    
    private NMapColorPickerButton()
    {
        Name = "ColorPickerButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(68, 60);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;

        _icon = InitIcon();
        this.AddChild(_icon);
        
    }

    public override void _Ready()
    {
    }
    
    protected override void ConnectSignals()
    {
        base.ConnectSignals();
        if (this.HasControllerHotkey)
            this.RegisterHotkeys();
        this._controllerHotkeyIcon = this.GetNodeOrNull<TextureRect>((NodePath) "%ControllerIcon");
        this.UpdateControllerButton();
    }
    
    public static readonly AddedNode<NMapScreen, NMapColorPickerButton> Map = new((mapScreen) =>
    {
        // grab drawing tools display container node
        var parent = mapScreen.GetNode<HBoxContainer>("DrawingTools/HBoxContainer");
        
        // initialize color picker button
        var button = new NMapColorPickerButton();
        
        // add this node to the drawing tools container
        parent.AddChild(button);
        
        // introduce the new neighbors
        // loadedClearButton.FocusNeighborRight = new NodePath("../ColorPickerButton");
        // button.FocusNeighborLeft = new NodePath("../ClearButton");
        var clearButton = (NButton)parent.GetNode("ClearButton");
        clearButton.FocusNeighborRight = new NodePath("../ColorButton");
        button.FocusNeighborLeft = new NodePath("../ClearButton");
        
        // return the newly created color picker button
        return button;
    });


    // ERROR FOUND!
    /*
     * ERROR: System.ArgumentException: An item with the same key has already been added. Key: <HBoxContainer#619783927240>
   at System.Collections.Generic.Dictionary`2.TryInsert(TKey key, TValue value, InsertionBehavior behavior)
   at System.Collections.Generic.Dictionary`2.Add(TKey key, TValue value)
   at MegaCrit.Sts2.Core.Nodes.HoverTips.NHoverTipSet.CreateAndShow(Control owner, IEnumerable`1 hoverTips, HoverTipAlignment alignment)
   at MegaCrit.Sts2.Core.Nodes.HoverTips.NHoverTipSet.CreateAndShow(Control owner, IHoverTip hoverTip, HoverTipAlignment alignment)
   at MegaCrit.Sts2.Core.Nodes.Screens.Map.NMapEraseButton.OnFocus()
   at MegaCrit.Sts2.Core.Nodes.GodotExtensions.NClickableControl.RefreshFocus()
   at MegaCrit.Sts2.Core.Nodes.GodotExtensions.NClickableControl.OnFocusHandler()
   at Godot.Callable.<From>g__Trampoline|1_0(Object delegateObj, NativeVariantPtrArgs args, godot_variant& ret)
   at Godot.DelegateUtils.InvokeWithVariantArgs(IntPtr delegateGCHandle, Void* trampoline, godot_variant** args, Int32 argc, godot_variant* outRet)
   at: void Godot.NativeInterop.ExceptionUtils.LogException(System.Exception) (:0)
   C# backtrace (most recent call first):
       [0] void Godot.GD.PushError(string)
       [1] void Godot.NativeInterop.ExceptionUtils.LogException(System.Exception)
       [2] void Godot.DelegateUtils.InvokeWithVariantArgs(nint, System.Void*, Godot.NativeInterop.godot_variant**, int, Godot.NativeInterop.godot_variant*)
ERROR: Neighbor focus node path is invalid: '../ColorButton'.
   at: _get_focus_neighbor (scene/gui/control.cpp:2609)
ERROR: Neighbor focus node path is invalid: '../ColorButton'.
   at: _get_focus_neighbor (scene/gui/control.cpp:2609)
ERROR: Neighbor focus node path is invalid: '../ColorButton'.
   at: _get_focus_neighbor (scene/gui/control.cpp:2609)
[INFO] Limiting background FPS to 30

     */
    
    
    // test
    protected override void OnPress()
    {
        base.OnFocus();
        this._icon.Texture = PreloadManager.Cache.GetTexture2D((string) GlowImagePath);
        this._tween?.Kill();
        this._tween = this.CreateTween().SetParallel();
        this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "scale", (Variant) (Vector2.One * 1.2f), 0.05);
        this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "self_modulate", (Variant) ActiveColor, 0.05);
        NHoverTipSet.CreateAndShow(this._drawingToolHolder, (IHoverTip) this._hoverTip).GlobalPosition = this._drawingToolHolder.GlobalPosition + new Vector2(10f, -132f);
    }

    protected override void OnFocus()
  {
    base.OnFocus();
    this._icon.Texture = PreloadManager.Cache.GetTexture2D((string) GlowImagePath);
    this._tween?.Kill();
    this._tween = this.CreateTween().SetParallel();
    this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "scale", (Variant) (Vector2.One * 1.2f), 0.05);
    this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "self_modulate", (Variant) ActiveColor, 0.05);
    NHoverTipSet.CreateAndShow(this._drawingToolHolder, (IHoverTip) this._hoverTip).GlobalPosition = this._drawingToolHolder.GlobalPosition + new Vector2(10f, -132f);
  }

  protected override void OnUnfocus()
  {
    base.OnUnfocus();
    this._icon.Texture = PreloadManager.Cache.GetTexture2D((string) ImagePath);
    this._tween?.Kill();
    this._tween = this.CreateTween().SetParallel();
    this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "scale", (Variant) (Vector2.One * 1.1f), 0.05);
    this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "self_modulate", (Variant) InactiveColor, 0.05);
    NHoverTipSet.Remove(this._drawingToolHolder);
  }
  




}




/*
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



}
*/