using Godot;
using MapArtist.MapArtistCode.Config;

namespace MapArtist.MapArtistCode.GUI.Items;

public partial class NMapArtistBrushWidth : HBoxContainer
{
    public const int DefaultBrushWidth = 4;
    
    public NMapArtistBrushWidthButton WidthButton;
    
    private HBoxContainer _adjustContainer = new HBoxContainer();
    private HSlider _slider = new HSlider();
    private Label _label = new Label();
    
    public int BrushWidth;

    public NMapArtistBrushWidth() {}

    public NMapArtistBrushWidth(Control mapArtistParent)
    {
        Name = "NMapArtistBrushWidthInterface";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(185f, 35f);
        // SetHSizeFlags(Control.SizeFlags.ExpandFill);
        // SetVSizeFlags(Control.SizeFlags.ExpandFill);


        WidthButton = new NMapArtistBrushWidthButton(mapArtistParent);
        
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
        _slider.SetHSizeFlags(Control.SizeFlags.ExpandFill);
        _slider.SetVSizeFlags(Control.SizeFlags.ShrinkCenter);
        _slider.Scrollable = false;

        _label.Name = "MapArtistBrushWidthLabel";
        _label.UniqueNameInOwner = true;
        _label.CustomMinimumSize = new Vector2(27f, 0f);
        _label.ClipText = true;
        _label.FocusMode = Control.FocusModeEnum.None;
        _label.MouseFilter =  Control.MouseFilterEnum.Pass;
        _label.VerticalAlignment = VerticalAlignment.Center;
        _label.SetLabelSettings(new LabelSettings());
        _label.GetLabelSettings().FontColor = Colors.Gainsboro;

        BrushWidth = DefaultBrushWidth;
    }

    public override void _Ready()
    {
        AddChild(WidthButton);
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
        if (MapArtistConfig.SynchronizedWidthSlider)
        {
            MapArtistController.MapArtistController.Instance.ApplySettingWidth();
        }
    }

    public void ToggleAdjustVisibility()
    {
        _adjustContainer.Visible = !_adjustContainer.Visible;
    }

    public void ResetValueBrushWidth()
    {
        _slider.Value = DefaultBrushWidth;
    }
    
    
    
}