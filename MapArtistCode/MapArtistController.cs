using BaseLib;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;

namespace MapArtist.MapArtistCode;

public sealed class MapArtistController
{
//--------------------------------------------------- Singleton ------------------------------------------------
    static MapArtistController() { }
    private MapArtistController() { }
    public static MapArtistController Instance { get; } = new MapArtistController();
//--------------------------------------------------------------------------------------------------------------
//-------------------------------------- Relevant NMapScreen Scene Nodes ---------------------------------------
    private NMapScreen? _existingMapScene; // The single, instantiated NMapScreen scene itself
    
    private TextureRect? _debugPlaceholderIcon; // Temporary
    
    // The button added to the existing DrawingTools/HBoxContainer to display the MapArtist GUI
    private NMapArtistGUIButton? _guiDisplayButton;
    
    // Container for the MapArtist GUI
    private NMapArtistGUIContainer? _guiContainer;

    // Both a row and an item; no container exclusively for this item; first row of the MapArtist GUI container
    private NColorPicker? _rowitemColorPicker;
    
    // Container for the second row of the MapArtist GUI container: buttons to adjust brush properties beyond Color
    private HBoxContainer? _rowPropertyButtonsContainer;
    private NMapArtistBrushWidthButton? _itemWidthButton; // Row 2, Item 1
    
    // Container for the third row of the MapArtist GUI container: apply selections button and reset properties button
    private HBoxContainer? _rowApplyResetButtonsContainer;
    private NMapArtistApplyButton? _itemApplyButton; // Row 3, Item 1
    private NMapArtistResetButton? _itemResetButton; // Row 3, Item 2
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
        if (_existingMapScene != null)
        {
            // Allow initialization only once
            BaseLibMain.Logger.Info("[MapArtistController] Attempted to re-initialize pre-existing node(s)" +
                                     " in MapArtistController. InitializeExisting should be called only once");
            return;
        }
        
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
        
        _guiDisplayButton = _existingMapScene.GetNode<NMapArtistGUIButton>("DrawingTools/HBoxContainer/MapArtistGUIButton");
        if (_guiDisplayButton == null)
        {
            BaseLibMain.Logger.Error("[MapArtistController] Failed to fetch or assign _guiDisplayButton from _existingMapScene.");
            return;
        }
        
        DebugInitializePlaceholderIcon();
        var childIcon = ShallowCopyIcon(_debugPlaceholderIcon);
        _guiDisplayButton.SetIcon(childIcon);
        _guiDisplayButton.AddChild(childIcon);
    }

    private void DebugInitializePlaceholderIcon()
    {
        if (_existingMapScene == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] Attempted to call DebugInitializePlaceholderIcon() before" +
                                     " assigning _existingMapScene.");
            return;
        }
        
        _debugPlaceholderIcon = _existingMapScene.GetNode<TextureRect>("DrawingTools/HBoxContainer/ClearButton/Icon");
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

    private void ConstructGui()
    {
        if (_existingMapScene == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] Attempted to call ConstructGui() before" +
                                    " assigning _existingMapScene.");
            return;
        }
        
        _guiContainer = new NMapArtistGUIContainer();
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
        _rowPropertyButtonsContainer =  InitHBoxContainer();
        _rowPropertyButtonsContainer.Name = "BrushPropertyButtonContainer";
        _guiContainer.AddChild(_rowPropertyButtonsContainer);

        // Item 1: Brush width button
        _itemWidthButton = new NMapArtistBrushWidthButton();
        var widthButtonIcon = ShallowCopyIcon(_debugPlaceholderIcon);
        _itemWidthButton.SetIcon(widthButtonIcon);
        _itemWidthButton.AddChild(widthButtonIcon);
        _rowPropertyButtonsContainer.AddChild(_itemWidthButton);
        _itemWidthButton.MapArtistButtonContainer = _rowPropertyButtonsContainer;
      
        // GUI row 3: apply/reset brush color and properties
        _rowApplyResetButtonsContainer =  InitHBoxContainer();
        _rowApplyResetButtonsContainer.Name = "ApplyResetButtonContainer";
        _guiContainer.AddChild(_rowApplyResetButtonsContainer);
        
        // Item 1: Apply Button
        _itemApplyButton = new NMapArtistApplyButton();
        var applyButtonIcon = ShallowCopyIcon(_debugPlaceholderIcon);
        _itemApplyButton.SetIcon(applyButtonIcon);
        _itemApplyButton.AddChild(applyButtonIcon);
        _rowApplyResetButtonsContainer.AddChild(_itemApplyButton);
        _itemApplyButton.MapArtistButtonContainer = _rowApplyResetButtonsContainer;
        
        // Item 2: Reset Button
        _itemResetButton = new NMapArtistResetButton();
        var resetButtonIcon = ShallowCopyIcon(_debugPlaceholderIcon);
        _itemResetButton.SetIcon(resetButtonIcon);
        _itemResetButton.AddChild(resetButtonIcon);
        _rowApplyResetButtonsContainer.AddChild(_itemResetButton);
        _itemResetButton.MapArtistButtonContainer = _rowApplyResetButtonsContainer;
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
        
        if (_itemWidthButton.WidthSelection == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] _itemWidthButton.WidthSelection == null" +
                                    " on ApplySettings() call.");
            return;
        }

        // apply pen color
        MapArtistDictionaries.SetColor(FetchLocalPlayer(), _rowitemColorPicker.Color);

        // apply pen width
        try {
            var widthVal = _itemWidthButton.WidthSelection.GetLine(0).ToFloat();
            MapArtistDictionaries.SetPenWidth(FetchLocalPlayer(), widthVal);
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
    }

}