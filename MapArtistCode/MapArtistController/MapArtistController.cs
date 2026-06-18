using BaseLib;
using Godot;
using MapArtist.MapArtistCode.GUI.Items;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;

namespace MapArtist.MapArtistCode.MapArtistController;

public sealed class MapArtistController
{
//--------------------------------------------------- Singleton ------------------------------------------------
    static MapArtistController() { }
    private MapArtistController() { }
    public static MapArtistController Instance { get; } = new MapArtistController();
//--------------------------------------------------------------------------------------------------------------
//-------------------------------------- Relevant NMapScreen Scene Nodes ---------------------------------------
    private NMapScreen? _existingMapScene; // The single, instantiated NMapScreen scene itself
    
    // An existing Icon pulled from the Map Scene. Make new icons by shallow copying and modifying Texture.
    private TextureRect? _prototypeIcon;

    private static readonly StringName ApplyImagePath = "res://MapArtist/Images/CustomIcons/mapartist_apply.png";
    // private static readonly StringName ApplyGlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_apply_glow.png";
    private static readonly StringName ResetImagePath = "res://MapArtist/Images/CustomIcons/mapartist_reset.png";
    // private static readonly StringName ResetGlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_reset_glow.png";
    private static readonly StringName WidthImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width.png";
    // private static readonly StringName WidthGlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width_glow.png";
    
    // The button added to the existing DrawingTools/HBoxContainer to display the MapArtist GUI
    private GUI.NMapArtistGUIButton? _guiDisplayButton;
    
    // Container for the MapArtist GUI
    private GUI.NMapArtistGUIContainer? _guiContainer;

    // Both a row and an item; no container exclusively for this item; first row of the MapArtist GUI container
    private NColorPicker? _rowitemColorPicker;

    // Use in future when more buttons added
    // // Container for the second row of the MapArtist GUI container: buttons to adjust brush properties beyond Color
    // private HBoxContainer? _rowPropertyButtonsContainer;
    // private NMapArtistBrushWidthButton? _itemWidthButton; // Row 2, Item 1
    //
    // // Container for the third row of the MapArtist GUI container: apply selections button and reset properties button
    // private HBoxContainer? _rowApplyResetButtonsContainer;
    // private NMapArtistApplyButton? _itemApplyButton; // Row 3, Item 1
    // private NMapArtistResetButton? _itemResetButton; // Row 3, Item 2
    
    // Container for the second row of the MapArtist GUI container: all buttons
    private HBoxContainer? _rowButtonsContainer;
    private NMapArtistBrushWidthButton? _itemWidthButton; // Row 2, Item 1
    private NMapArtistApplyButton? _itemApplyButton; // Row 2, Item 2
    private NMapArtistResetButton? _itemResetButton; // Row 2, Item 3


    private HBoxContainer? _bWidthSliderContainer;
    private HSlider? _bWidthSlider;
    private Label? _bWidthLabel;
    
//--------------------------------------------------------------------------------------------------------------
//---------------------------------------- Additional Member Variables -----------------------------------------
    private Player? _localPlayer; // Needed for controller logic
//--------------------------------------------------------------------------------------------------------------

//----------------------------------- GUI Initialization Methods and Helpers -----------------------------------
    public void InitializeGui(NMapScreen? mapScene)
    {
        if (mapScene == null)
        {
            BaseLibMain.Logger.Error("[MapArtistController] Null mapScene passed to InitializeExisting.");
            return;
        }
        // if (_existingMapScene != null)
        // {
        //     // Allow initialization only once
        //     BaseLibMain.Logger.Info("[MapArtistController] Attempted to re-initialize pre-existing node(s)" +
        //                              " in MapArtistController. InitializeExisting should be called only once");
        //     return;
        // }
        
        _existingMapScene =  mapScene;
        InitializeAddedNodeGuiButton();
        ConstructGui();
    }
    
    private void InitializeAddedNodeGuiButton()
    {
        if (_existingMapScene == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] Attempted to call InitializeAddedNodeGuiButton() before" +
                                     " assigning _existingMapScene.");
            return;
        }
        
        _guiDisplayButton = _existingMapScene.GetNode<GUI.NMapArtistGUIButton>("DrawingTools/HBoxContainer/MapArtistGUIButton");
        if (_guiDisplayButton == null)
        {
            BaseLibMain.Logger.Error("[MapArtistController] Failed to fetch or assign _guiDisplayButton from _existingMapScene.");
            return;
        }
        
        InitializePrototypeIcon();
        var childIcon = ShallowCopyIcon(_prototypeIcon);
        _guiDisplayButton.SetIcon(childIcon);
        _guiDisplayButton.AddChild(childIcon);
    }

    private void InitializePrototypeIcon()
    {
        if (_existingMapScene == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] Attempted to call DebugInitializePlaceholderIcon() before" +
                                     " assigning _existingMapScene.");
            return;
        }
        
        _prototypeIcon = _existingMapScene.GetNode<TextureRect>("DrawingTools/HBoxContainer/ClearButton/Icon");
    }
    
    private static TextureRect ShallowCopyIcon(TextureRect toCopy)
    {
        var icon = new TextureRect();

        icon.Name = "Icon";
        
        icon.SelfModulate = toCopy.SelfModulate;
        icon.SetMaterial(toCopy.GetMaterial());
        icon.SetUseParentMaterial(toCopy.GetUseParentMaterial());
        icon.LayoutMode = toCopy.LayoutMode;
        icon.AnchorsPreset = toCopy.AnchorsPreset;
        icon.AnchorRight = toCopy.AnchorRight;
        icon.AnchorBottom = toCopy.AnchorBottom;
        icon.GrowHorizontal = toCopy.GrowHorizontal;
        icon.GrowVertical = toCopy.GrowVertical;
        icon.Scale =  new Vector2(toCopy.Scale.X, toCopy.Scale.Y);
        icon.PivotOffset = new Vector2(toCopy.PivotOffset.X, toCopy.PivotOffset.Y);
        icon.MouseFilter = toCopy.MouseFilter;
        icon.SetTexture(toCopy.GetTexture());
        icon.SetUseParentMaterial(toCopy.GetUseParentMaterial());
        icon.ExpandMode = toCopy.ExpandMode;
        icon.StretchMode = toCopy.StretchMode;
        
        return icon;
    }

    private static TextureRect ShallowCopyIcon(TextureRect toCopy, StringName imagePath)
    {
        var icon = ShallowCopyIcon(toCopy);
        icon.Texture = ResourceLoader.Load<Texture2D>(imagePath);

        return icon;
    }
    
    private static void InitializeIconUseShallow(TextureRect toCopy, StringName imagePath, GUI.Items.Abstract.NMapArtistButton forButton)
    {
        var icon = ShallowCopyIcon(toCopy, imagePath);
        forButton.SetIcon(icon);
        forButton.AddChild(icon);
    }

    private void ConstructGui()
    {
        if (_existingMapScene == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] Attempted to call ConstructGui() before" +
                                    " assigning _existingMapScene.");
            return;
        }
        
        _guiContainer = new GUI.NMapArtistGUIContainer();
        _existingMapScene.AddChild(_guiContainer);
        
        // GUI row 1: color picker
        _rowitemColorPicker = new NColorPicker();
        _rowitemColorPicker.Name = "ItemColorPicker";
        _rowitemColorPicker.UniqueNameInOwner = true;
        var player = FetchLocalPlayer();
        if (player != null)
        {
            _rowitemColorPicker.Color = FetchLocalPlayer().Character.MapDrawingColor;
        }
        _guiContainer.AddChild(_rowitemColorPicker);
      
        // GUI row 2: additional brush property (buttons)
        _rowButtonsContainer =  InitHBoxContainer();
        _rowButtonsContainer.Name = "BrushPropertyButtonContainer";
        _guiContainer.AddChild(_rowButtonsContainer);


        _itemApplyButton = new NMapArtistApplyButton();
        InitializeIconUseShallow(_prototypeIcon, ApplyImagePath, _itemApplyButton);
        _rowButtonsContainer.AddChild(_itemApplyButton);
        _itemApplyButton.MapArtistButtonContainer = _rowButtonsContainer;
        
        _itemResetButton = new NMapArtistResetButton();
        InitializeIconUseShallow(_prototypeIcon, ResetImagePath, _itemResetButton);
        _rowButtonsContainer.AddChild(_itemResetButton);
        _itemResetButton.MapArtistButtonContainer = _rowButtonsContainer;
        
        _itemWidthButton = new NMapArtistBrushWidthButton(_rowButtonsContainer);
        InitializeIconUseShallow(_prototypeIcon, WidthImagePath, _itemWidthButton);
        _rowButtonsContainer.AddChild(_itemWidthButton);
    }

    public void ConstructBrushWidthSlider()
    {
        _bWidthSliderContainer = _rowButtonsContainer.GetNode<HBoxContainer>("LabelledSlideContainer");
        _bWidthSlider = _bWidthSliderContainer.GetNode<HSlider>("WidthSlider");
        // _bWidthLabel = _bWidthSlider.GetNode<Label>("WidthSliderLabel");
        _bWidthLabel = _bWidthSliderContainer.GetNode<Label>("WidthSliderLabel");
        
        _bWidthSliderContainer.Position = new Vector2((_itemWidthButton.Size.X + 7f), 0f);
        
        _bWidthSlider.MinValue = 1;
        _bWidthSlider.MaxValue = 20;
        _bWidthSlider.Step = 1;
        _bWidthSlider.SetHSizeFlags(Control.SizeFlags.ExpandFill);
        _bWidthSlider.SetVSizeFlags(Control.SizeFlags.ShrinkCenter);
        _bWidthSlider.Scrollable = false;

        // _bWidthSlider.OffsetLeft = 3.0f;


        _bWidthLabel.CustomMinimumSize = new Vector2(27f, 0f);
        _bWidthLabel.ClipText = true;
        _bWidthLabel.FocusMode = Control.FocusModeEnum.None;
        _bWidthLabel.MouseFilter =  Control.MouseFilterEnum.Pass;
        _bWidthLabel.VerticalAlignment = VerticalAlignment.Center;

        // _bWidthLabel.Position = new Vector2(0f, 0f);
        // _bWidthLabel.SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
        _bWidthLabel.VerticalAlignment = VerticalAlignment.Center;
        
        
        // _bWidthLabel.SetAnchorsPreset(Control.LayoutPreset.CenterBottom);
        _bWidthLabel.SetLabelSettings(new LabelSettings());
        _bWidthLabel.GetLabelSettings().FontColor = Colors.Gainsboro;

        // Initial value to display (game's default)
        _itemWidthButton.BrushWidth = 4;
        _bWidthSlider.Value = _itemWidthButton.BrushWidth;
        _bWidthLabel.Text = _itemWidthButton.BrushWidth.ToString();

    }
    
    private static HBoxContainer InitHBoxContainer()
    {
        var hbc = new HBoxContainer();
        hbc.UniqueNameInOwner = true;
        hbc.SizeFlagsHorizontal = Control.SizeFlags.Fill;
        hbc.SizeFlagsVertical = Control.SizeFlags.Fill;
      
        return hbc;
    }
//--------------------------------------------------------------------------------------------------------------

//---------------------------------------------- Controller Logic ----------------------------------------------
    private Player? FetchLocalPlayer()
    {
        // Not yet tested/suitable for Multiplayer
        
        if (_localPlayer != null)
        {
            // results in ResetSettings() grabbing the wrong color to display in ColorPicker where character changed
            return _localPlayer;
        }

        var currState = RunManager.Instance.DebugOnlyGetState();
        if (currState == null)
        {
            BaseLibMain.Logger.Error("[MapArtistController] Failed to load current state");
            return null;
        }

        _localPlayer = currState.GetPlayer(RunManager.Instance.NetService.NetId);
        return  _localPlayer;
    }

    public void ToggleGui()
    {
        if (_guiContainer == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] _guiContainer == null on ToggleGui() call.");
            return;
        }
        
        _guiContainer.Visible = !_guiContainer.Visible;
    }




    public void ApplySettings()
    {
        if (FetchLocalPlayer() == null)
        {
            BaseLibMain.Logger.Error("[MapArtistController] Failed to fetch player.");
            return;
        }

        if (_rowitemColorPicker == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] _rowitemColorPicker == null on ApplySettings() call.");
            return;
        }
        
        if (_itemWidthButton == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] _itemWidthButton == null on ApplySettings() call.");
            return;
        }

        // apply pen color
        MapArtistDictionaries.SetColor(FetchLocalPlayer(), _rowitemColorPicker.Color);

        // apply pen width
        try {
            var widthVal = _itemWidthButton.BrushWidth;
            MapArtistDictionaries.SetPenWidth(FetchLocalPlayer(), (float)widthVal);
        } catch (FormatException notFloat)
        {
            // no valid pen width to apply
        }
    }
    
    
    
    public void ResetSettings(){
        var player = FetchLocalPlayer();
        if (player == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] Failed to fetch player.");
            return;
        }

        if (_rowitemColorPicker == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] _rowitemColorPicker == null on ResetSettings() call.");
            return;
        }
        
        MapArtistDictionaries.ClearAll(FetchLocalPlayer());
        _rowitemColorPicker.Color = player.Character.MapDrawingColor;
        
        _bWidthSlider.Value = 4; // changing slider value without Brush width; ValueChanged signal to update BrushWidth
        var widthVal = _itemWidthButton.BrushWidth;
        MapArtistDictionaries.SetPenWidth(FetchLocalPlayer(), (float)widthVal);
    }

}