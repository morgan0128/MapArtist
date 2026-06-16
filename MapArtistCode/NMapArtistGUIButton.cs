using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MapArtist.MapArtistCode;

[ScriptPath("res://MapArtistCode/NMapArtistGUIButton.cs")]
public partial class NMapArtistGUIButton : NButton
{
    private bool HasControllerHotkey => this.Hotkeys.Length != 0;
    private static readonly StringName ImagePath = "res://images/packed/map/drawing_clear.png";
    private static readonly StringName GlowImagePath = "res://images/packed/map/drawing_clear_glow.png";
    private static readonly Color ActiveColor = new Color("FFE57DFF");
    private static readonly Color InactiveColor = new Color("FFFFFF80");
    
    private NMapScreen? _mapScene;

    private Control? _drawingToolHolder;
    private TextureRect? _icon;
    private HoverTip _hoverTip;
    private Tween? _tween;
    
    // The existing, instantiated NMapScreen passed by constructor (because using lambda: AddedNode)
    private NMapArtistGUIButton(NMapScreen mapScene)
    {
        Name = "MapArtistGUIButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(68, 60);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;

        _mapScene = mapScene;
    }

    private NMapArtistGUIButton()
    {

    }

    public void SetIcon(TextureRect icon)
    {
        _icon = icon;
    }
    
    public static readonly AddedNode<NMapScreen, NMapArtistGUIButton> Map = new((mapScreen) =>
    {
        // grab the drawing tools node
        var drawingTools = mapScreen.GetNode<NinePatchRect>("DrawingTools");
        
        // grab drawing tools display container node
        var parent = drawingTools.GetNode<HBoxContainer>("HBoxContainer");
        
        // grab the (to be) neighboring button
        var clearButton = (NButton)parent.GetNode("ClearButton");
        
        // initialize, grabbing the instantiated NMapScreen node to give to controller for gui initialization process
        var button = new NMapArtistGUIButton(mapScreen);
        
        // add this node to the drawing tools container
        parent.AddChild(button);
        
        // introduce the new neighbors
        clearButton.FocusNeighborRight = button.GetPath();
        button.FocusNeighborLeft = new NodePath("../ClearButton");
        
        // drawing tools hbox resizing
        // parent.OffsetRight += 68;
        // parent.OffsetLeft -= 34;
        // parent.OffsetRight += 34;
        // drawingTools.AxisStretchHorizontal = NinePatchRect.AxisStretchMode.Stretch;
        // drawingTools.PatchMarginLeft = 18;
        // drawingTools.PatchMarginRight = 18;
        
        button._drawingToolHolder = parent;
        
        // return the newly created color picker button
        return button;
    });

    public override void _Ready()
    {
        LocString locDesc = new LocString("static_hover_tips", "MAPARTIST-GUI_BUTTON.description");
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-GUI_BUTTON.title"), locDesc);

        // MapArtistController.Instance.InitializeGui(_mapScene);
        
        ConnectSignals();
    }
    
    protected override void ConnectSignals()
    {
        base.ConnectSignals();
        if (this.HasControllerHotkey)
            this.RegisterHotkeys();
        this._controllerHotkeyIcon = this.GetNodeOrNull<TextureRect>((NodePath) "%ControllerIcon");
        this.UpdateControllerButton();
    }
    
    public override void _EnterTree()
    {
        base._EnterTree();
        if (_mapScene.GetNodeOrNull<VBoxContainer>("MapArtistGUI") == null)
        {
            MapArtistController.Instance.InitializeGui(_mapScene);
        }
    }
    
    protected override void OnPress()
    {
        base.OnPress();
        MapArtistController.Instance.ToggleGui();
    }

    protected override void OnFocus()
    {
        base.OnFocus();
 
        if (_icon == null || this._drawingToolHolder == null)
        {
            // PrintUninitializedError();
            return;
        }
        
        _icon.Texture = PreloadManager.Cache.GetTexture2D((string) GlowImagePath);
        this._tween?.Kill();
        this._tween = this.CreateTween().SetParallel();
        this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "scale", (Variant) (Vector2.One * 1.2f), 0.05);
        this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "self_modulate", (Variant) ActiveColor, 0.05);
        NHoverTipSet.CreateAndShow(this._drawingToolHolder, (IHoverTip) this._hoverTip).GlobalPosition = this._drawingToolHolder.GlobalPosition + new Vector2(10f, -132f);
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();

        if (_icon == null || this._drawingToolHolder == null)
        {
            // PrintUninitializedError();
            return;
        }
        this._icon.Texture = PreloadManager.Cache.GetTexture2D((string) ImagePath);
        this._tween?.Kill();
        this._tween = this.CreateTween().SetParallel();
        this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "scale", (Variant) (Vector2.One * 1.1f), 0.05);
        this._tween.TweenProperty((GodotObject) this._icon, (NodePath) "self_modulate", (Variant) InactiveColor, 0.05);
        NHoverTipSet.Remove(this._drawingToolHolder);
    }
    
}
