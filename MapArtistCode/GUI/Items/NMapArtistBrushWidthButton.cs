using Godot;
using MapArtist.MapArtistCode.Config;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using Range = Godot.Range;

namespace MapArtist.MapArtistCode.GUI.Items;

[ScriptPath("res://MapArtistCode/GUI/Items/NMapArtistBrushWidthButton.cs")]
public partial class NMapArtistBrushWidthButton : GUI.Items.Abstract.NMapArtistButton
{
    
    private bool HasControllerHotkey => this.Hotkeys.Length != 0;
    private static readonly StringName ImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width.png";
    private static readonly StringName GlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width_glow.png";
    private static readonly Color ActiveColor = new Color("FFE57DFF");
    private static readonly Color InactiveColor = new Color("FFFFFF80");
    
    public Control? MapArtistButtonContainer;
    
    private HoverTip _hoverTip;
    private Tween? _tween;
    
    // moved
    // private HBoxContainer _editContainer = new HBoxContainer();
    // private HSlider _widthSlider = new HSlider();
    // private Label _widthSliderLabel = new Label();
    // public int BrushWidth;
    
    
    public NMapArtistBrushWidthButton()
    {
        // Name = "MapArtistBrushWidthButton";
        // UniqueNameInOwner = true;
        // CustomMinimumSize = new Vector2(35f, 35f);
        // LayoutMode = 2;
        // FocusMode = FocusModeEnum.All;
    }
    
    public NMapArtistBrushWidthButton(Control mapArtistAncestorItemContainer)
    {
        Name = "MapArtistBrushWidthButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(35f, 35f);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;

        MapArtistButtonContainer = mapArtistAncestorItemContainer;
    }

    public override void _Ready()
    {
        base._Ready();
        // Localization
        LocString locDesc = new LocString("static_hover_tips", "MAPARTIST-BRUSH_WIDTH.description");
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-BRUSH_WIDTH.title"), locDesc);
        
        // MapArtistButtonContainer.AddChild(_editContainer);
        // _editContainer.AddChild(_widthSlider);
        // _editContainer.AddChild(_widthSliderLabel);
        
        // _widthSlider.MinValue = 1;
        // _widthSlider.MaxValue = 20;
        // _widthSlider.Step = 1;
        // _widthSlider.SetHSizeFlags(Control.SizeFlags.ExpandFill);
        // _widthSlider.SetVSizeFlags(Control.SizeFlags.ShrinkCenter);
        // _widthSlider.Scrollable = false;
        //
        // _widthSliderLabel.CustomMinimumSize = new Vector2(27f, 0f);
        // _widthSliderLabel.ClipText = true;
        // _widthSliderLabel.FocusMode = Control.FocusModeEnum.None;
        // _widthSliderLabel.MouseFilter =  Control.MouseFilterEnum.Pass;
        // _widthSliderLabel.VerticalAlignment = VerticalAlignment.Center;
        // _widthSliderLabel.SetLabelSettings(new LabelSettings());
        // _widthSliderLabel.GetLabelSettings().FontColor = Colors.Gainsboro;
        //
        // this.BrushWidth = 4;
        // _widthSlider.Value = this.BrushWidth;
        // _widthSliderLabel.Text = this.BrushWidth.ToString();
        //
        // _widthSlider.ValueChanged += OnSliderValueChanged;
        
        ConnectSignals();
    }

    // private void OnSliderValueChanged(double value)
    // {
    //     BrushWidth = (int)value;
    //     // _widthEdit.Set(LineEdit.PropertyName.Text, BrushWidth); // does not emit TextChanged signal
    //     _widthSliderLabel.Text = BrushWidth.ToString();
    //     if (MapArtistConfig.SynchronizedWidthSlider)
    //     {
    //         MapArtistController.MapArtistController.Instance.ApplySettingWidth();
    //     }
    // }
    
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
        // _editContainer.Visible = !_editContainer.Visible;
        MapArtistController.MapArtistController.Instance.ToggleBrushWidthGui();
    }

    protected override void OnFocus()
    {
        base.OnFocus();
 
        if (Icon == null)
        {
            // PrintUninitializedError();
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
            // PrintUninitializedError();
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
