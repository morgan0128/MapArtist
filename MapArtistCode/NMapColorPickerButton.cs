using System.ComponentModel;
using BaseLib.Abstracts;
using BaseLib.Patches.Localization;
using BaseLib.Utils;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Extensions;
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
    // private static readonly Control loadedMapScreen = (Control)ResourceLoader.Load<PackedScene>("res://scenes/screens/map/map_screen.tscn").Instantiate();
    // private static readonly HBoxContainer loadedDrawingContainer = loadedMapScreen.GetNode<HBoxContainer>("DrawingTools/HBoxContainer");
    // private static readonly NButton loadedClearButton = (NButton)loadedDrawingContainer.GetNode("ClearButton");
    // private static readonly TextureRect loadedClearIcon = (TextureRect)loadedClearButton.GetNode("Icon");
        
    
    private bool HasControllerHotkey => this.Hotkeys.Length != 0;
    private static readonly StringName ImagePath = "res://images/packed/map/drawing_clear.png";
    private static readonly StringName GlowImagePath = "res://images/packed/map/drawing_clear_glow.png";
    // private Control _drawingToolHolder = loadedDrawingContainer;
    private Control _drawingToolHolder;
    private TextureRect _icon;
    private HoverTip _hoverTip;
    private Tween? _tween;
    private static readonly Color ActiveColor = new Color("FFE57DFF");
    private static readonly Color InactiveColor = new Color("FFFFFF80");

    // private ColorPicker _colorPickerGUI;

    private NMapColorPickerButton()
    {
        Name = "ColorPickerButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(68, 60);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;

        _icon = InitIcon();
        this.AddChild(_icon);
        
        // _colorPickerGUI =  InitColorButton();
        // this.AddChild(_colorPickerGUI);

        _drawingToolHolder = (Control)ResourceLoader.Load<PackedScene>("res://scenes/screens/map/map_screen.tscn")
            .Instantiate().GetNode<HBoxContainer>("DrawingTools/HBoxContainer");
    }

    private static TextureRect InitIcon()
    {
        var loadedClearIcon = (TextureRect)ResourceLoader.Load<PackedScene>("res://scenes/screens/map/map_screen.tscn").Instantiate().
            GetNode<HBoxContainer>("DrawingTools/HBoxContainer").GetNode<NButton>("ClearButton").GetNode<TextureRect>("Icon");
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

    // private static ColorPicker InitColorButton()
    // {
    //     var gui = new ColorPicker();
    //     gui.GlobalPosition = new Vector2(0f, 0f);
    //     gui.SetVisible(true);
        // gui.ShowBehindParent(true);
        // gui.SetMouseFilter(MouseFilterEnum.Ignore);
        

        
        // test
        // var icon = (TextureRect)ResourceLoader.Load<PackedScene>("res://scenes/screens/map/map_screen.tscn").Instantiate().
            // GetNode<HBoxContainer>("DrawingTools/HBoxContainer").GetNode<NButton>("ClearButton").GetNode<TextureRect>("Icon");
        // gui.SelfModulate = icon.SelfModulate;
        // gui.SetMaterial(icon.GetMaterial());
        // gui.SetUseParentMaterial(icon.GetUseParentMaterial());
        // gui.LayoutMode = icon.LayoutMode;
        // gui.AnchorsPreset = icon.AnchorsPreset;
        // gui.AnchorRight = icon.AnchorRight;
        // gui.AnchorBottom = icon.AnchorBottom;
        // gui.GrowHorizontal = icon.GrowHorizontal;
        // gui.GrowVertical = icon.GrowVertical;
        // gui.Scale =  new Vector2(icon.Scale.X, icon.Scale.Y);
        // gui.PivotOffset = new Vector2(icon.PivotOffset.X, icon.PivotOffset.Y);
        // gui.MouseFilter = icon.MouseFilter;
        // // gui.SetTexture(icon.GetTexture());
        // gui.SetUseParentMaterial(icon.GetUseParentMaterial());
        // gui.ExpandMode = icon.ExpandMode;
        // gui.StretchMode = icon.StretchMode;

    //     return gui;
    // }

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
    
    public static readonly AddedNode<NMapScreen, NMapColorPickerButton> Map = new((mapScreen) =>
    {
        // grab drawing tools display container node
        var parent = mapScreen.GetNode<HBoxContainer>("DrawingTools/HBoxContainer");
        
        // initialize color picker button
        var button = new NMapColorPickerButton();
        
        // add this node to the drawing tools container
        parent.AddChild(button);
        
        // introduce the new neighbors
        var clearButton = (NButton)parent.GetNode("ClearButton");
        clearButton.FocusNeighborRight = button.GetPath();
        button.FocusNeighborLeft = new NodePath("../ClearButton");
        
        // drawing tools hbox resizing
        parent.OffsetRight += 68;
        parent.OffsetLeft -= 34;
        parent.OffsetRight += 34;
        
        // return the newly created color picker button
        return button;
    });

    
    
    
    // test
    protected override void OnPress()
    {
        base.OnPress();

        // display the color picker here
        // EmitSignal("backing_DisplayGUI");
        NColorPickerGUI.hideGUI();
        
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
