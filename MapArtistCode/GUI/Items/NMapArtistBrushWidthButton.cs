using Godot;
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
    // private TextureRect? _icon;
    private HoverTip _hoverTip;
    private Tween? _tween;

    // public TextEdit? WidthSelection;
    private HBoxContainer EditContainer = new HBoxContainer();
    private HSlider _widthSlider = new HSlider();
    private LineEdit _widthEdit = new LineEdit();
    public float BrushWidth;
    
    
    public NMapArtistBrushWidthButton()
    {
        Name = "MapArtistBrushWidthButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(35f, 35f);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;
        
        // WidthSelection = new TextEdit();
        // WidthSelection.CustomMinimumSize = new Vector2(100f, 35f);
        //
        // WidthSelection.GlobalPosition = this.GlobalPosition + new Vector2(35f, 0f);
        // WidthSelection.PlaceholderText = "Pen Width";
        // WidthSelection.Visible = false;
        // AddChild(WidthSelection);
        
        EditContainer.GlobalPosition = this.GlobalPosition + new Vector2(35f, 0);
        EditContainer.CustomMinimumSize = new Vector2(150f, 35f);
        EditContainer.Visible = false;
        AddChild(EditContainer);

        // _widthSlider = new HSlider();
        // _widthSlider.MinValue = 0;
        // _widthSlider.MaxValue = 100;
        _widthSlider.Step = 0.01;
        _widthSlider.SetHSizeFlags(SizeFlags.ExpandFill);
        _widthSlider.SetAnchorsPreset(LayoutPreset.CenterLeft);
        EditContainer.AddChild(_widthSlider);
        
        // _widthEdit = new LineEdit();
        _widthEdit.Size = new Vector2(35f, 35f);
        EditContainer.AddChild(_widthEdit);
    }

    public override void _Ready()
    {
        // Localization
        LocString locDesc = new LocString("static_hover_tips", "MAPARTIST-BRUSH_WIDTH.description");
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-BRUSH_WIDTH.title"), locDesc);

        _widthSlider.ValueChanged += OnSliderValueChanged;
        _widthEdit.TextChanged += OnTextValueChanged;
        
        ConnectSignals();
    }

    private void OnSliderValueChanged(double value)
    {
        BrushWidth = (float)value;
        _widthEdit.Set(LineEdit.PropertyName.Text, BrushWidth); // does not emit TextChanged signal
    }
    
    private void OnTextValueChanged(string text)
    {
        if (!double.TryParse(text, out double result)) return;
        BrushWidth = (float)Math.Round(result);
        _widthSlider.SetValueNoSignal(BrushWidth);
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
        EditContainer.Visible = !EditContainer.Visible;
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
