using BaseLib;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;

namespace MapArtist.MapArtistCode;

public sealed class MapArtistController
{
    //----------------------------------- Singleton -----------------------------------
    static MapArtistController() { }
    private MapArtistController() { }
    public static MapArtistController Instance { get; } = new MapArtistController();
    //---------------------------------------------------------------------------------

    private NMapScreen? _existingMapScene;
    private HBoxContainer? _existingDrawingToolsContainer;
    private NButton? _existingRightmostDrawingToolButton;
    
    private TextureRect? _debugPlaceholderIcon;
    
    private NMapArtistGUIButton? _guiDisplayButton;
    
    private NMapArtistGUI? _guiContainer;

    private NColorPicker? _rowitemColorPicker;
    
    private HBoxContainer? _rowPropertyButtonsContainer;
    private NMapArtistBrushWidthButton? _itemWidthButton;
    
    private HBoxContainer? _rowApplyResetButtonsContainer;
    private NMapArtistApplyButton? _itemApplyButton;
    private NMapArtistResetButton? _itemResetButton;
    
    // Player
    private Player? _localPlayer;


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
    
    private TextureRect ShallowCopyIcon(TextureRect toCopy)
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
        
        _guiContainer = new NMapArtistGUI();
        _existingMapScene.AddChild(_guiContainer);
        
        // GUI row 1: color picker
        _rowitemColorPicker = new NColorPicker();
        _rowitemColorPicker.Name = "ItemColorPicker";
        _rowitemColorPicker.UniqueNameInOwner = true;
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
        
        // Item 1: Reset Button
        _itemResetButton = new NMapArtistResetButton();
        var resetButtonIcon = ShallowCopyIcon(_debugPlaceholderIcon);
        _itemResetButton.SetIcon(resetButtonIcon);
        _itemResetButton.AddChild(resetButtonIcon);
        _rowApplyResetButtonsContainer.AddChild(_itemResetButton);
    }
    
    private static HBoxContainer InitHBoxContainer()
    {
        var hbc = new HBoxContainer();
        hbc.UniqueNameInOwner = true;
        hbc.SizeFlagsHorizontal = Control.SizeFlags.Fill;
        hbc.SizeFlagsVertical = Control.SizeFlags.Fill;
      
        return hbc;
    }
    
    
    
    // Controller Logic
    
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
        // if (FetchLocalPlayer() == null)
        // {
        //     BaseLibMain.Logger.Error("[MapArtistController] Failed to fetch player.");
        //     return;
        // }

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
        MapArtistDictionaries.SetPenWidth(FetchLocalPlayer(), _itemWidthButton.WidthSelection.GetLine(0).ToFloat());
    }
    
    public void ClearAllDictionaries(){
        MapArtistDictionaries.ClearAll(FetchLocalPlayer());
    }
    


}