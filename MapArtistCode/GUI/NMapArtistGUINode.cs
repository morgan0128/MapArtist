using Godot;
using MapArtist.MapArtistCode.GUI.Items;

namespace MapArtist.MapArtistCode.GUI;

[ScriptPath("res://MapArtistCode/GUI/NMapArtistGUINode.cs")]
public partial class NMapArtistGUINode : VBoxContainer
{
    // Both a row and an item; no container exclusively for this item; first row of the MapArtist GUI container
    private NColorPicker? _rowitemColorPicker;
    
    // Container for buttons row of the MapArtist GUI container
    private HBoxContainer? _rowButtonsContainer;
    
    private NMapArtistApplyButton? _itemApplyButton;
    private NMapArtistResetButton? _itemResetButton;
    private NMapArtistBrushWidthButton? _itemBrushWidthButton;
    private HBoxContainer? _bWidthSliderContainer;
    private HSlider? _bWidthSlider;
    private Label? _bWidthLabel;
    
    public NMapArtistGUINode()
    {
      Name = "MapArtistGUI";
      UniqueNameInOwner = true;
      Visible = false; 
      LayoutMode = 2;
      MouseFilter = MouseFilterEnum.Pass;
      SetAnchorsPreset(LayoutPreset.TopLeft);
      GlobalPosition = new Vector2(12f, 158f);
    }
    
    public override void _Ready()
    {
        // this.GuiInput += OnGuiInput;
    }

    public NColorPicker? GetRowItemColorPicker()
    {
        return _rowitemColorPicker;
    }

    public HBoxContainer? GetRowButtonsContainer()
    {
        return _rowButtonsContainer;
    }

    public NMapArtistApplyButton? GetItemApplyButton()
    {
        return _itemApplyButton;
    }

    public NMapArtistResetButton? GetItemResetButton()
    {
        return _itemResetButton;
    }

    public NMapArtistBrushWidthButton? GetItemBrushWidthButton()
    {
        return _itemBrushWidthButton;
    }
    
    public HBoxContainer? GetItemWidthSliderContainer()
    {
        return _bWidthSliderContainer;
    }
    
    public HSlider? GetItemWidthSlider()
    {
        return _bWidthSlider;
    }
    
    public Label? GetItemWidthLabel()
    {
        return _bWidthLabel;
    }
    

    public void AssignRowitemColorPicker(NColorPicker colorPicker)
    {
        _rowitemColorPicker = colorPicker;
        this.AddChild(_rowitemColorPicker);
    }
    
    public void AssignRowButtonsContainer(HBoxContainer container)
    {
        _rowButtonsContainer = container;
        this.AddChild(_rowButtonsContainer);
    }

    public void AssignItemApplyButton(NMapArtistApplyButton button)
    {
        _itemApplyButton = button;
        _rowButtonsContainer.AddChild(_itemApplyButton);
        _itemApplyButton.MapArtistButtonContainer = _rowButtonsContainer;

    }

    public void AssignItemResetButton(NMapArtistResetButton button)
    {
        _itemResetButton = button;
        _rowButtonsContainer.AddChild(_itemResetButton);
        _itemResetButton.MapArtistButtonContainer = _rowButtonsContainer;
    }
    
    public void AssignItemWidthButton(NMapArtistBrushWidthButton button)
    {
        _itemBrushWidthButton = button;
        _itemBrushWidthButton.MapArtistButtonContainer = _rowButtonsContainer;
        _rowButtonsContainer.AddChild(_itemBrushWidthButton);
        
        _bWidthSliderContainer = _rowButtonsContainer.GetNode<HBoxContainer>("LabelledSlideContainer");
        _bWidthSlider = _bWidthSliderContainer.GetNode<HSlider>("WidthSlider");
        _bWidthLabel = _bWidthSliderContainer.GetNode<Label>("WidthSliderLabel");
        
        _bWidthSliderContainer.Position = new Vector2((_itemBrushWidthButton.Size.X + 7f), 0f);

    }
    
}