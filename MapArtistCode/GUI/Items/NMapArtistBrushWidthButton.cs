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
    // private TextureRect? _icon;
    private HoverTip _hoverTip;
    private Tween? _tween;

    // public TextEdit? WidthSelection;
    private HBoxContainer _editContainer = new HBoxContainer();
    private HSlider _widthSlider = new HSlider();
    // private LineEdit _widthEdit = new LineEdit();
    private Label _widthSliderLabel = new Label();
    public int BrushWidth;
    
    
    public NMapArtistBrushWidthButton()
    {
        Name = "MapArtistBrushWidthButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(35f, 35f);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;

        _editContainer.Name = "LabelledSlideContainer";
        _editContainer.UniqueNameInOwner = true;
        _widthSlider.Name = "WidthSlider";
        _widthSlider.UniqueNameInOwner = true;
        _widthSliderLabel.Name = "WidthSliderLabel";
        _widthSliderLabel.UniqueNameInOwner = true;
        
        _editContainer.CustomMinimumSize = new Vector2(150f, 35f);
        _editContainer.Visible = false;
        
        // _widthSlider.MinValue = 0;
        // _widthSlider.MaxValue = 100;
        // _widthSlider.Step = 1;
        // _widthSlider.SetHSizeFlags(SizeFlags.ExpandFill);
        // _widthSlider.SetVSizeFlags(SizeFlags.ShrinkCenter);
        // _widthSlider.Scrollable = false;
        // // EditContainer.AddChild(_widthSlider);
        // _widthSlider.OffsetLeft = 3.0f;
        

        
        // _widthSlider.AddChild(_widthEdit);
        // // _widthEdit.Size = new Vector2(0f, 35f);
        // _widthEdit.Editable = false;
        // _widthEdit.FocusMode = FocusModeEnum.None;
        // _widthEdit.SelectingEnabled = false;
        // _widthEdit.MouseFilter = MouseFilterEnum.Ignore;
        // // var beginPos = (_widthSlider.GetSize().X / 2) - (_widthEdit.GetSize().X / 2); // need to instead get node I think
        // // _widthEdit.Position = new Vector2(beginPos, 15f);
        // _widthEdit.Position = new Vector2((_widthSlider.GetSize().X / 2), 15f);
        // // _widthEdit.SetAnchorsPreset(LayoutPreset.VcenterWide);
        // _widthEdit.Flat = true;
        // _widthEdit.MaxLength = 3;
        // _widthEdit.ContextMenuEnabled = false;
        // _widthEdit.EmojiMenuEnabled = false;
        // _widthEdit.DragAndDropSelectionEnabled = false;
        // _widthEdit.AddThemeColorOverride("_widthEditOverride", Colors.Black);


        // _widthSlider.AddChild(_widthSliderLabel);
        // _widthSliderLabel.SetAnchor(Side.Left, 1, false, true);
        // _widthSliderLabel.AddThemeColorOverride("width_label_font", Colors.Black);
    }

    // public void SetContainer(Container container)
    // {
    //     MapArtistButtonContainer = container;
    //     MapArtistButtonContainer.AddChild(_editContainer);
    //     _editContainer.AddChild(_widthSlider);
    //     _editContainer.AddChild(_widthSliderLabel);
    // }

    public override void _Ready()
    {
        // base._Ready();
        // Localization
        LocString locDesc = new LocString("static_hover_tips", "MAPARTIST-BRUSH_WIDTH.description");
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-BRUSH_WIDTH.title"), locDesc);
        
        MapArtistButtonContainer.AddChild(_editContainer);
        _editContainer.AddChild(_widthSlider);
        _editContainer.AddChild(_widthSliderLabel);
        
        // MapArtistController.MapArtistController.Instance.ConstructBrushWidthSlider();

        // _editContainer.Position = new Vector2((_itemBrushWidthButton.Size.X + 7f), 0f);
        
        _widthSlider.MinValue = 1;
        _widthSlider.MaxValue = 20;
        _widthSlider.Step = 1;
        _widthSlider.SetHSizeFlags(Control.SizeFlags.ExpandFill);
        _widthSlider.SetVSizeFlags(Control.SizeFlags.ShrinkCenter);
        _widthSlider.Scrollable = false;

        _widthSliderLabel.CustomMinimumSize = new Vector2(27f, 0f);
        _widthSliderLabel.ClipText = true;
        _widthSliderLabel.FocusMode = Control.FocusModeEnum.None;
        _widthSliderLabel.MouseFilter =  Control.MouseFilterEnum.Pass;
        _widthSliderLabel.VerticalAlignment = VerticalAlignment.Center;
        _widthSliderLabel.SetLabelSettings(new LabelSettings());
        _widthSliderLabel.GetLabelSettings().FontColor = Colors.Gainsboro;
        
        this.BrushWidth = 4;
        _widthSlider.Value = this.BrushWidth;
        _widthSliderLabel.Text = this.BrushWidth.ToString();
        
        _widthSlider.ValueChanged += OnSliderValueChanged;
        
        ConnectSignals();
    }

    private void OnSliderValueChanged(double value)
    {
        BrushWidth = (int)value;
        // _widthEdit.Set(LineEdit.PropertyName.Text, BrushWidth); // does not emit TextChanged signal
        _widthSliderLabel.Text = BrushWidth.ToString();
        if (MapArtistConfig.SynchronizedWidthSlider)
        {
            MapArtistController.MapArtistController.Instance.ApplySettingWidth();
        }
    }
    
    // Editable disabled
    // private void OnTextValueChanged(string text)
    // {
    //     if (!double.TryParse(text, out double result)) return;
    //     BrushWidth = (float)Math.Round(result);
    //     _widthSlider.SetValueNoSignal(BrushWidth);
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
        _editContainer.Visible = !_editContainer.Visible;
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
