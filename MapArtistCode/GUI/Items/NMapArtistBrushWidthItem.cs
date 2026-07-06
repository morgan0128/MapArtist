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

    public int BrushWidth = Util.DefaultBrushWidth;

    public NMapArtistBrushWidthItem() {}

    public void InitializeIconUseDeepCopy(TextureRect? toCopy)
    {
        if (toCopy == null) return;
        _widthButton.InitializeIconUseDeepCopy(toCopy);
    }
    
    public NMapArtistBrushWidthItem(Control mapArtistParent)
    {
        Name = "NMapArtistBrushWidthInterface";
        UniqueNameInOwner = true;
        // CustomMinimumSize = new Vector2(185f, 35f);
        SetHSizeFlags(SizeFlags.ExpandFill);
        
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
        _label.CustomMinimumSize = new Vector2(0f, 0f);
        // _label.SetHSizeFlags(SizeFlags.Fill);
        _label.ClipText = false;
        _label.FocusMode = FocusModeEnum.None;
        _label.MouseFilter =  MouseFilterEnum.Pass;
        _label.VerticalAlignment = VerticalAlignment.Center;
        _label.HorizontalAlignment = HorizontalAlignment.Left;
        _label.SetLabelSettings(new LabelSettings());
        _label.GetLabelSettings().FontColor = Colors.Gainsboro;

        BrushWidth = Util.DefaultBrushWidth;
    }
    
    // Do not call before or during _Ready()
    public void ResetWidth()
    {
        _slider.Value = Util.DefaultBrushWidth;
    }

    public override void _Ready()
    {
        AddChild(_widthButton);
        AddChild(_adjustContainer);
        _adjustContainer.AddChild(_slider);
        _adjustContainer.AddChild(_label);
        
        _slider.Value = BrushWidth;
        _label.Text = BrushWidth.ToString();
        
        _slider.ValueChanged += OnSliderValueChanged; 
        // _slider.Value = BrushWidth; calling OnSliderValueChanged before _Ready() is unsafe
    }
    
    // setting _slider.Value in code or in UI updates _label.Text automatically
    private void OnSliderValueChanged(double value)
    {
        BrushWidth = (int)value;
        _label.Text = BrushWidth.ToString();
        MapArtistController.MapArtistController.Instance.SelectedWidth = BrushWidth;
        // if (MapArtistConfig.SynchronizedWidthSlider)
        // {
        //     MapArtistController.MapArtistController.Instance.ApplySettingWidth();
        // }
    }

    public void ToggleAdjustVisibility()
    {
        _adjustContainer.Visible = !_adjustContainer.Visible;
    }
    
    public partial class NMapArtistBrushWidthButtonItem : NMapArtistButtonItem
    {
        
        public NMapArtistBrushWidthButtonItem()
        {
        }
    
        /*
         * Note: we pass a Control 'mapArtistParent' to be known by _widthButton in order for _widthButton to display
         * hovertip in correct container, given that _widthButton is generally intended to be doubly nested within a
         * broader button container; unlike most button items which are generally intended to be direct children
         */
        
        public NMapArtistBrushWidthButtonItem(Control mapArtistAncestorItemContainer)
        {
            Name = "MapArtistBrushWidthButton";
            UniqueNameInOwner = true;
            CustomMinimumSize = new Vector2(35f, 35f);
            LayoutMode = 2;
            FocusMode = FocusModeEnum.All;

            MapArtistButtonContainer = mapArtistAncestorItemContainer;
            
            ImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width.png";
            GlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width_glow.png";
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