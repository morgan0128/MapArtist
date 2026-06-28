using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace MapArtist.MapArtistCode.GUI.Items.Abstract;

public abstract partial class NMapArtistButton : NButton
{
    public Control? MapArtistButtonContainer;
    protected HoverTip HoverTip;
    private Tween? _tween;
    private TextureRect? _icon;
    private bool HasControllerHotkey => this.Hotkeys.Length != 0;

    
    public void SetIcon(TextureRect icon)
    {
        _icon = icon;
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
        if (_icon == null || MapArtistButtonContainer == null) return;
        
        _icon.Texture = PreloadManager.Cache.GetTexture2D(glowPath);
        _tween?.Kill();
        _tween = this.CreateTween().SetParallel();
        _tween.TweenProperty(_icon, (NodePath) "scale", (Vector2.One * 1.2f), 0.05);
        _tween.TweenProperty(_icon, (NodePath) "self_modulate", active, 0.05);
        var hoverTipSet = NHoverTipSet.CreateAndShow(MapArtistButtonContainer, HoverTip);
        if (hoverTipSet == null) return;
        hoverTipSet.GlobalPosition = MapArtistButtonContainer.GlobalPosition + new Vector2(10f, -132f);
    }
    
    protected void ChildIconSfxUnglow(StringName imagePath, Color inactive)
    {
        if (_icon == null || MapArtistButtonContainer == null) return;
        
        _icon.Texture = PreloadManager.Cache.GetTexture2D(imagePath);
        _tween?.Kill();
        _tween = CreateTween().SetParallel();
        _tween.TweenProperty(_icon, (NodePath) "scale", (Vector2.One * 1.1f), 0.05);
        _tween.TweenProperty(_icon, (NodePath) "self_modulate", inactive, 0.05);
        NHoverTipSet.Remove(MapArtistButtonContainer);
    }

    
    
    
}