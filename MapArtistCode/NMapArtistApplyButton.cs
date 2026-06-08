using BaseLib;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;

namespace MapArtist.MapArtistCode;

[ScriptPath("res://MapArtistCode/NMapArtistApplyButton.cs")]
public partial class NMapArtistApplyButton : NButton
{
    
    private bool HasControllerHotkey => this.Hotkeys.Length != 0;
    private static readonly StringName ImagePath = "res://images/packed/map/drawing_clear.png";
    private static readonly StringName GlowImagePath = "res://images/packed/map/drawing_clear_glow.png";
    private static readonly Color ActiveColor = new Color("FFE57DFF");
    private static readonly Color InactiveColor = new Color("FFFFFF80");
    
    private NMapScreen? _mapScene;
    // private readonly NButton? _neighborButton;
    // private Control? _drawingToolHolder;

    private ColorPicker? _itemColorPicker;
    
    private Control? _mapArtistButtonContainer;
    private TextureRect? _placeholderIcon;
    private TextureRect? _icon;
    private HoverTip _hoverTip;
    private Tween? _tween;

    private Player? _localPlayer;
    
    // private NMapArtistApplyButton(NMapScreen mapScene, HBoxContainer parent, NButton neighbor)
    public NMapArtistApplyButton(NMapScreen mapScene, HBoxContainer parent, TextureRect placeholderIcon)

    {
        Name = "MapArtistApplyButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(35f, 35f);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;

        _mapScene = mapScene;
        _mapArtistButtonContainer = parent;
        _placeholderIcon = placeholderIcon;
        
        _itemColorPicker = mapScene.GetNode<ColorPicker>("MapArtistGUI/ItemColorPicker");
        
        _icon = InitIcon(placeholderIcon);
        this.AddChild(_icon);
    }

    private NMapArtistApplyButton()
    {

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

    // private bool CheckIsUninitialized()
    // {
        // return _mapScene != null && _neighborButton != null && _drawingToolHolder != null && _icon != null && _tween != null;
    // }

    private static void PrintUninitializedError()
    {
        BaseLibMain.Logger.Error("[MapArtist] Tried to unsafely access uninitialized NMapArtistGUIButton. Use the parameterized constructor.");
    }
    
    // public static readonly AddedNode<NMapScreen, NMapArtistApplyButton> Map = new((mapScreen) =>
    // {
    //     var parent = mapScreen.GetNode<HBoxContainer>("MapArtistGUI/HBC_MapArtistGUI");
    //
    //     
    //     // grab the (to be) neighboring button
    //     var placeholder = (TextureRect)mapScreen.GetNode<TextureRect>("DrawingTools/HBoxContainer/ClearButton/Icon").GetNode("ClearButton");
    //     
    //     // initialize color picker button
    //     
    //     var button = new NMapArtistApplyButton(mapScreen, parent, placeholder);
    //     
    //
    //     parent.AddChild(button);
    //     
    //     // introduce the new neighbors
    //     // clearButton.FocusNeighborRight = button.GetPath();
    //     // button.FocusNeighborLeft = new NodePath("../ClearButton");
    //     
    //     // drawing tools hbox resizing
    //     // parent.OffsetRight += 68;
    //     // parent.OffsetLeft -= 34;
    //     // parent.OffsetRight += 34;
    //     
    //     // button._drawingToolHolder = parent;
    //
    //
    //     
    //     // return the newly created color picker button
    //     return button;
    // });

    private Player? FetchLocalPlayer()
    {
        // if (RunManager.Instance.NetService.Type == NetGameType.Singleplayer)
        // {
            if (_localPlayer != null)
            {
                return _localPlayer;
            }

            var currState = RunManager.Instance.DebugOnlyGetState();
            if (currState == null)
            {
                BaseLibMain.Logger.Error("[MapArtist] Failed to load current state");
                return null;
            }

            _localPlayer = currState.GetPlayer(RunManager.Instance.NetService.NetId);
            return  _localPlayer;

            // }
        // else
        // {
        //    // not yet implemented
            // return null;
        // }
        
    }

    public override void _Ready()
    {
        // _drawingToolHolder = this.GetParent<HBoxContainer>();
        
        LocString locDesc = new LocString("static_hover_tips", "MAPARTIST-APPLY_BUTTON.description");
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-APPLY_BUTTON.title"), locDesc);

        // temporarily passing the "clear icon" texture (which I globally use as placeholder) here through an accommodating temporary version of the constructor
        // var gui = new NMapArtistGUI(_icon.Texture);
        // var icon1 = InitIcon(_icon);
        // var icon2 = InitIcon(_icon);
        // var gui = new NMapArtistGUI(icon1, icon2);
        // _mapScene.AddChild(gui);
        // gui._itemButtonPenSettings.FocusNeighborRight = gui._itemButtonApplySettings.GetPath();
        // gui._itemButtonApplySettings.FocusNeighborLeft = gui._itemButtonApplySettings.GetPath();
        
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
        // var localPlayer = FetchLocalPlayer();
        if (FetchLocalPlayer() == null)
        {
            BaseLibMain.Logger.Error("[MapArtist] Failed to fetch player");
            return;
        }

        if (_mapScene == null)
        {
            BaseLibMain.Logger.Error("[MapArtist] The Map Artist button failed to store the Map Scene");
            return;
        }
        
        MapArtistDrawingColors.Set(FetchLocalPlayer(), _itemColorPicker.Color);
        // NMapArtistGUI.Instance.ToggleGui(_localPlayer);
    }

    protected override void OnFocus()
    {
        base.OnFocus();
 
        if (_icon == null)
        {
            PrintUninitializedError();
            return;
        }
        
        _icon.Texture = PreloadManager.Cache.GetTexture2D((string) GlowImagePath);
        this._tween?.Kill();
        this._tween = this.CreateTween().SetParallel();
        this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "scale", (Variant) (Vector2.One * 1.2f), 0.05);
        this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "self_modulate", (Variant) ActiveColor, 0.05);
        NHoverTipSet.CreateAndShow(this._mapArtistButtonContainer, (IHoverTip) this._hoverTip).GlobalPosition = this._mapArtistButtonContainer.GlobalPosition + new Vector2(10f, -132f);
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();

        if (_icon == null)
        {
            PrintUninitializedError();
            return;
        }
        this._icon.Texture = PreloadManager.Cache.GetTexture2D((string) ImagePath);
        this._tween?.Kill();
        this._tween = this.CreateTween().SetParallel();
        this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "scale", (Variant) (Vector2.One * 1.1f), 0.05);
        this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "self_modulate", (Variant) InactiveColor, 0.05);
        NHoverTipSet.Remove(this._mapArtistButtonContainer);
    }
  




}
