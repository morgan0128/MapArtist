using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace MapArtist.MapArtistCode.GUI.Items.Abstract;

public abstract partial class NMapArtistButton : NButton
{
    public Control? MapArtistButtonContainer;
    // private TextureRect? _icon;
    protected HoverTip _hoverTip;
    protected Tween? _tween;
    protected TextureRect? Icon;
    private bool HasControllerHotkey => this.Hotkeys.Length != 0;

    
    public void SetIcon(TextureRect icon)
    {
        Icon = icon;
    }

    public override void _Ready()
    {
        // SetVSizeFlags(SizeFlags.ShrinkBegin);
    }
    
    protected override void ConnectSignals()
    {
        base.ConnectSignals();
        if (this.HasControllerHotkey)
            this.RegisterHotkeys();
        this._controllerHotkeyIcon = this.GetNodeOrNull<TextureRect>((NodePath) "%ControllerIcon");
        this.UpdateControllerButton();
    }

    protected void ChildIconSfxGlow(StringName glowPath, Color active)
    {
        if (Icon == null) return;
        
        Icon.Texture = PreloadManager.Cache.GetTexture2D((string) glowPath);
        this._tween?.Kill();
        this._tween = this.CreateTween().SetParallel();
        this._tween.TweenProperty((GodotObject) this.Icon, (NodePath) "scale", (Variant) (Vector2.One * 1.2f), 0.05);
        this._tween.TweenProperty((GodotObject) this.Icon, (NodePath) "self_modulate", (Variant) active, 0.05);
        NHoverTipSet.CreateAndShow(this.MapArtistButtonContainer, (IHoverTip) this._hoverTip).GlobalPosition = this.MapArtistButtonContainer.GlobalPosition + new Vector2(10f, -132f);
    }
    
    protected void ChildIconSfxUnglow(StringName imagePath, Color inactive)
    {
        if (Icon == null) return;
        
        this.Icon.Texture = PreloadManager.Cache.GetTexture2D((string) imagePath);
        this._tween?.Kill();
        this._tween = this.CreateTween().SetParallel();
        this._tween.TweenProperty((GodotObject) this.Icon, (NodePath) "scale", (Variant) (Vector2.One * 1.1f), 0.05);
        this._tween.TweenProperty((GodotObject) this.Icon, (NodePath) "self_modulate", (Variant) inactive, 0.05);
        NHoverTipSet.Remove(this.MapArtistButtonContainer);
    }

    
    
    
}