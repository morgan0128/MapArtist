using Godot;
using MapArtist.MapArtistCode.Config;
using MapArtist.MapArtistCode.GUI.Items.Abstract;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace MapArtist.MapArtistCode.GUI.Items;

public partial class NMapArtistBrushWidthItem : NMapArtistBoxContainerItem
{
    private NMapArtistBrushWidthButtonItem _widthButton;
    
    private HBoxContainer _adjustContainer = new HBoxContainer();
    private HSlider _slider = new HSlider();
    private Label _label = new Label();

    private int _brushWidth;

    public NMapArtistBrushWidthItem() {}
    
    public void InitializeIconUseDeepCopy(TextureRect toCopy, StringName imagePath)
    {
        _widthButton.InitializeIconUseDeepCopy(toCopy, imagePath);
    }

    public NMapArtistBrushWidthItem(Control mapArtistParent)
    {
        Name = "NMapArtistBrushWidthInterface";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(185f, 35f);
        // SetHSizeFlags(Control.SizeFlags.ExpandFill);
        // SetVSizeFlags(Control.SizeFlags.ExpandFill);
        _widthButton = new NMapArtistBrushWidthButtonItem(mapArtistParent);
        
        _adjustContainer.Name = "MapArtistBrushWidthAdjustContainer";
        _adjustContainer.UniqueNameInOwner = true;
        _adjustContainer.Visible = false;
        _adjustContainer.SetHSizeFlags(SizeFlags.ExpandFill);
        // _adjustContainer.SetVSizeFlags(SizeFlags.ShrinkCenter);

        _slider.Name = "MapArtistBrushWidthSlider";
        _slider.UniqueNameInOwner = true;
        _slider.MinValue = 1;
        _slider.MinValue = 1;
        _slider.MaxValue = 20;
        _slider.Step = 1;
        _slider.SetHSizeFlags(SizeFlags.ExpandFill);
        _slider.SetVSizeFlags(SizeFlags.ShrinkCenter);
        _slider.Scrollable = false;

        _label.Name = "MapArtistBrushWidthLabel";
        _label.UniqueNameInOwner = true;
        _label.CustomMinimumSize = new Vector2(27f, 0f);
        _label.ClipText = true;
        _label.FocusMode = FocusModeEnum.None;
        _label.MouseFilter =  MouseFilterEnum.Pass;
        _label.VerticalAlignment = VerticalAlignment.Center;
        _label.SetLabelSettings(new LabelSettings());
        _label.GetLabelSettings().FontColor = Colors.Gainsboro;

        _brushWidth = Util.DefaultBrushWidth;
    }

    public override void _Ready()
    {
        AddChild(_widthButton);
        AddChild(_adjustContainer);
        _adjustContainer.AddChild(_slider);
        _adjustContainer.AddChild(_label);
        
        _slider.Value = _brushWidth;
        _label.Text = _brushWidth.ToString();
        
        _slider.ValueChanged += OnSliderValueChanged; 
        // _slider.Value = BrushWidth; calling OnSliderValueChanged before _Ready() is unsafe
    }
    
    // setting _slider.Value in code or in UI updates _label.Text automatically
    private void OnSliderValueChanged(double value)
    {
        _brushWidth = (int)value;
        _label.Text = _brushWidth.ToString();
        MapArtistController.MapArtistController.Instance.SelectedWidth = _brushWidth;
        // if (MapArtistConfig.SynchronizedWidthSlider)
        // {
        //     MapArtistController.MapArtistController.Instance.ApplySettingWidth();
        // }
    }

    public void ToggleAdjustVisibility()
    {
        _adjustContainer.Visible = !_adjustContainer.Visible;
    }

    public void ResetValueBrushWidth()
    {
        _slider.Value = Util.DefaultBrushWidth;
    }
    
    
    public partial class NMapArtistBrushWidthButtonItem : NMapArtistButtonItem
    {
        private static readonly StringName ImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width.png";
        private static readonly StringName GlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width_glow.png";
        private static readonly Color ActiveColor = new Color("FFE57DFF");
        private static readonly Color InactiveColor = new Color("FFFFFF80");

    
        public NMapArtistBrushWidthButtonItem()
        {
        }
    
        public NMapArtistBrushWidthButtonItem(Control mapArtistAncestorItemContainer)
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
            var locDesc = new LocString("static_hover_tips", "MAPARTIST-BRUSH_WIDTH.description");
            HoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-BRUSH_WIDTH.title"), locDesc);
        
            ConnectSignals();
        }
    
        protected override void OnPress()
        {
            base.OnPress();
            MapArtistController.MapArtistController.Instance.ToggleBrushWidthGui();
        }
    
        protected override void OnFocus()
        {
            ChildIconSfxGlow(GlowImagePath, ActiveColor);
        }

        protected override void OnUnfocus()
        {
            ChildIconSfxUnglow(ImagePath, InactiveColor);
        }

    }
    
}