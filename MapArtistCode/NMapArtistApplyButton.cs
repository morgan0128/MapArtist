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
public partial class NMapArtistApplyButton : NMapArtistButton
{
    
    private bool HasControllerHotkey => this.Hotkeys.Length != 0;
    // private static readonly StringName ImagePath = "res://images/packed/map/drawing_clear.png";
    private static readonly StringName ImagePath = "res://MapArtist/Images/CustomIcons/mapartist_apply.png";
    // private static readonly StringName GlowImagePath = "res://images/packed/map/drawing_clear_glow.png";
    private static readonly StringName GlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_apply_glow.png";
    private static readonly Color ActiveColor = new Color("FFE57DFF");
    private static readonly Color InactiveColor = new Color("FFFFFF80");
    
    public Control? MapArtistButtonContainer;
    // private TextureRect? _icon;
    private HoverTip _hoverTip;
    private Tween? _tween;
    
    public NMapArtistApplyButton()
    {
        Name = "MapArtistApplyButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(35f, 35f);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;
    }
    
    private static void PrintUninitializedError()
    {
        BaseLibMain.Logger.Error("[MapArtist] Tried to unsafely access uninitialized NMapArtistGUIButton. Use the parameterized constructor.");
    }
    
    public override void _Ready()
    {
        LocString locDesc = new LocString("static_hover_tips", "MAPARTIST-APPLY_BUTTON.description");
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-APPLY_BUTTON.title"), locDesc);
        
        // _icon.Texture =  PreloadManager.Cache.GetTexture2D((string) ImagePath);
        
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
    
    protected override void OnPress()
    {
        base.OnPress();
        MapArtistController.Instance.ApplySettings();
    }

    protected override void OnFocus()
    {
        base.OnFocus();
 
        if (Icon == null)
        {
            PrintUninitializedError();
            return;
        }
        
        Icon.Texture = PreloadManager.Cache.GetTexture2D((string) GlowImagePath);
        this._tween?.Kill();
        this._tween = this.CreateTween().SetParallel();
        this._tween.TweenProperty((GodotObject) this.Icon, (NodePath) "scale", (Variant) (Vector2.One * 1.2f), 0.05);
        this._tween.TweenProperty((GodotObject) this.Icon, (NodePath) "self_modulate", (Variant) ActiveColor, 0.05);
        NHoverTipSet.CreateAndShow(this.MapArtistButtonContainer, (IHoverTip) this._hoverTip).GlobalPosition = this.MapArtistButtonContainer.GlobalPosition + new Vector2(10f, -132f);
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();

        if (Icon == null)
        {
            PrintUninitializedError();
            return;
        }
        this.Icon.Texture = PreloadManager.Cache.GetTexture2D((string) ImagePath);
        this._tween?.Kill();
        this._tween = this.CreateTween().SetParallel();
        this._tween.TweenProperty((GodotObject) this.Icon, (NodePath) "scale", (Variant) (Vector2.One * 1.1f), 0.05);
        this._tween.TweenProperty((GodotObject) this.Icon, (NodePath) "self_modulate", (Variant) InactiveColor, 0.05);
        NHoverTipSet.Remove(this.MapArtistButtonContainer);
    }
    
}
