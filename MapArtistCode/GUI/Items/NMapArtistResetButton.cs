using BaseLib;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace MapArtist.MapArtistCode.GUI.Items;

[ScriptPath("res://MapArtistCode/NMapArtistResetButton.cs")]
public partial class NMapArtistResetButton : GUI.Items.Abstract.NMapArtistButton
{
    
    private bool HasControllerHotkey => this.Hotkeys.Length != 0;
    private static readonly StringName ImagePath = "res://MapArtist/Images/CustomIcons/mapartist_reset.png";
    private static readonly StringName GlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_reset_glow.png";
    private static readonly Color ActiveColor = new Color("FFE57DFF");
    private static readonly Color InactiveColor = new Color("FFFFFF80");
    
    public Control? MapArtistButtonContainer;
    // private TextureRect? _icon;
    private HoverTip _hoverTip;
    private Tween? _tween;

    public NMapArtistResetButton()
    {
        Name = "MapArtistResetButton";
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
        LocString locDesc = new LocString("static_hover_tips", "MAPARTIST-RESET_BUTTON.description");
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-RESET_BUTTON.title"), locDesc);
        
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
        MapArtistController.MapArtistController.Instance.ResetSettings();
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
