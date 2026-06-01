using System.ComponentModel;
using System.Reflection;
using BaseLib.Abstracts;
using BaseLib.Patches.Localization;
using BaseLib.Utils;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;
using MegaCrit.Sts2.Core.Runs;

namespace MapArtist.MapArtistCode;

[ScriptPath("res://MapArtistCode/NMapColorPickerButton.cs")]
public partial class NMapColorPickerButton : NButton
{
    
    private bool HasControllerHotkey => this.Hotkeys.Length != 0;
    private static readonly StringName ImagePath = "res://images/packed/map/drawing_clear.png";
    private static readonly StringName GlowImagePath = "res://images/packed/map/drawing_clear_glow.png";
    private readonly NMapScreen _mapScene;
    private readonly NButton _neighborButton;
    private Control _drawingToolHolder;
    private TextureRect _icon;
    private HoverTip _hoverTip;
    private Tween? _tween;
    private static readonly Color ActiveColor = new Color("FFE57DFF");
    private static readonly Color InactiveColor = new Color("FFFFFF80");

    private Player _localPlayer;
    
    private NMapColorPickerButton(NMapScreen mapScene, HBoxContainer parent, NButton neighbor)
    {
        Name = "ColorPickerButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(68, 60);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;

        _mapScene = mapScene;
        _drawingToolHolder = parent;
        _neighborButton = neighbor;
        
        var neighborIcon = neighbor.GetNode<TextureRect>("Icon");
        _icon = InitIcon(neighborIcon);
        this.AddChild(_icon);

        _localPlayer = GetLocalPlayer();
    }

    private static TextureRect InitIcon(TextureRect toCopy)
    {
        var icon = new TextureRect();
        icon.SelfModulate = toCopy.SelfModulate;
        icon.SetMaterial(toCopy.GetMaterial());
        icon.SetUseParentMaterial(toCopy.GetUseParentMaterial());
        icon.LayoutMode = toCopy.LayoutMode;
        icon.AnchorsPreset = toCopy.AnchorsPreset;
        icon.AnchorRight = toCopy.AnchorRight;
        icon.AnchorBottom = toCopy.AnchorBottom;
        icon.GrowHorizontal = toCopy.GrowHorizontal;
        icon.GrowVertical = toCopy.GrowVertical;
        icon.Scale =  new Vector2(toCopy.Scale.X, toCopy.Scale.Y);
        icon.PivotOffset = new Vector2(toCopy.PivotOffset.X, toCopy.PivotOffset.Y);
        icon.MouseFilter = toCopy.MouseFilter;
        icon.SetTexture(toCopy.GetTexture());
        icon.SetUseParentMaterial(toCopy.GetUseParentMaterial());
        icon.ExpandMode = toCopy.ExpandMode;
        icon.StretchMode = toCopy.StretchMode;
        
        return icon;
    }
    
    public static readonly AddedNode<NMapScreen, NMapColorPickerButton> Map = new((mapScreen) =>
    {
        // grab drawing tools display container node
        var parent = mapScreen.GetNode<HBoxContainer>("DrawingTools/HBoxContainer");
        
        // grab the (to be) neighboring button
        var clearButton = (NButton)parent.GetNode("ClearButton");
        
        // initialize color picker button
        var button = new NMapColorPickerButton(mapScreen, parent, clearButton);
        
        // add this node to the drawing tools container
        parent.AddChild(button);
        
        // introduce the new neighbors
        clearButton.FocusNeighborRight = button.GetPath();
        button.FocusNeighborLeft = new NodePath("../ClearButton");
        
        // drawing tools hbox resizing
        parent.OffsetRight += 68;
        parent.OffsetLeft -= 34;
        parent.OffsetRight += 34;
        
        button._drawingToolHolder = parent;
        
        // return the newly created color picker button
        return button;
    });

    private Player GetLocalPlayer()
    {
        LocalContext.NetId = RunManager.Instance.NetService.NetId;
        return LocalContext.GetMe(_mapScene.PlayerVoteDictionary.Keys);
    }

    public override void _Ready()
    {

        _drawingToolHolder = this.GetParent<HBoxContainer>();
        
        LocString locDesc = new LocString("static_hover_tips", "MAPARTIST-COLOR_PICKER.description");
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-COLOR_PICKER.title"), locDesc);

        ConnectSignals();
        // AddUserSignal("backing_DisplayGUI");
    }
    
    protected override void ConnectSignals()
    {
        base.ConnectSignals();
        if (this.HasControllerHotkey)
            this.RegisterHotkeys();
        this._controllerHotkeyIcon = this.GetNodeOrNull<TextureRect>((NodePath) "%ControllerIcon");
        this.UpdateControllerButton();
    }
    
    protected override void OnPress()
    {
        base.OnPress();

        // display the color picker here
        // EmitSignal("backing_DisplayGUI");
        NColorPickerGUI.toggleGUI(_localPlayer);
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
