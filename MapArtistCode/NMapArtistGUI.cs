using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MapArtist.MapArtistCode;

[ScriptPath("res://MapArtistCode/NMapArtistGUI.cs")]
public partial class NMapArtistGUI : VBoxContainer
{
    // private VBoxContainer _childContainer;

    private NColorPicker _itemColorPicker;

    private NMapScreen _mapScene;
    private HBoxContainer _itemButtonsHboxContainer;
    public NButton _itemButtonPenSettings;
    public NButton _itemButtonApplySettings;
    private int _genericButtonCounter = 0;

    private static readonly string ScenePath = "res://scenes/screens/map/map_screen.tscn";
    
    // private TextureRect _genericButton1Texture;
    // private TextureRect _genericButton2Texture;

    
    public NMapArtistGUI(NMapScreen mapScene)
    {
      Name = "MapArtistGUI";
      UniqueNameInOwner = true;
      
      // test
      Visible = false; 
      // Visible = true;
      
      LayoutMode = 2;
      SetAnchorsPreset(LayoutPreset.TopLeft);
      GlobalPosition = new Vector2(50f, 150f);
      
      
      _mapScene = mapScene;
      
      // CustomMinimumSize = new Vector2(1000.0f, 1000.0f);
      
      // row 1, color picker
      _itemColorPicker = new NColorPicker();
      _itemColorPicker.Name = "ItemColorPicker";
      _itemColorPicker.UniqueNameInOwner = true;
      this.AddChild(_itemColorPicker);
      
      // row 2, pen settings gui button and apply changes button
      // _genericButton1Texture = buttonTexture1;
      // _genericButton2Texture = buttonTexture2;
      
      
      _itemButtonsHboxContainer =  InitHBoxContainer();
      this.AddChild(_itemButtonsHboxContainer);

      // _itemButtonPenSettings =  InitGeneric_Button("generic_button1", buttonTexture);
      // _itemButtonApplySettings = InitGeneric_Button("generic_button2", buttonTexture);
      // _itemButtonPenSettings =  InitGeneric_Button();
      // _itemButtonApplySettings = InitGeneric_Button();
      // _itemButtonsHboxContainer.AddChild(_itemButtonPenSettings);
      // _itemButtonsHboxContainer.AddChild(_itemButtonApplySettings);



      // _itemButtonPenSettings._Ready();
      // _itemButtonApplySettings._Ready();
      
      // _itemButtonPenSettings._EnterTree();
      // _itemButtonApplySettings._EnterTree();
      
      // _itemButtonPenSettings.AddChild(buttonTexture1);
      // _itemButtonApplySettings.AddChild(buttonTexture2);
    }

    private static HBoxContainer InitHBoxContainer()
    {
      var hbc = new HBoxContainer();
      hbc.Name = "HBC_MapArtistGUI";
      hbc.UniqueNameInOwner = true;
      hbc.SizeFlagsHorizontal = SizeFlags.Fill;
      hbc.SizeFlagsVertical = SizeFlags.Fill;
      // hbc.CustomMinimumSize = new Vector2(100f, 100f);
      // hbc.Alignment = AlignmentMode.Begin;
      
      return hbc;
    }

    // private NButton InitGeneric_Button()
    // {
    //   var b = new NButton();
    //   // b.Name = name;
    //   b.Name = (StringName)("generic_button" + (++this._genericButtonCounter));
    //   b.UniqueNameInOwner = true;
    //   b.CustomMinimumSize = new Vector2(35f, 35f);
    //   
    //   return b;
    // }
    

    // public static readonly AddedNode<NMapScreen, NMapArtistGUI> Node = new((mapScreen) =>
    // {
        // var parent = mapScreen;
        
        // var gui = new NMapArtistGUI();
        // grab the (to be) neighboring button
        // var placeholder = (TextureRect)mapScreen.GetNode<TextureRect>("DrawingTools/HBoxContainer/ClearButton/Icon").GetNode("ClearButton");
        // initialize color picker button
        // var button = new NMapArtistApplyButton(mapScreen, parent, placeholder);

        // parent.AddChild(gui);
        
        
        // introduce the new neighbors
        // clearButton.FocusNeighborRight = button.GetPath();
        // button.FocusNeighborLeft = new NodePath("../ClearButton");
        // drawing tools hbox resizing
        // parent.OffsetRight += 68;
        // parent.OffsetLeft -= 34;
        // parent.OffsetRight += 34;
        // button._drawingToolHolder = parent;
        
        
        // var parent = mapScreen;
        // parent.AddChild(gui);
        // mapScreen.AddChild(gui);
        // return gui;
    // });
    
    
    public override void _Ready()
    {
        var icon = _mapScene.GetNode<TextureRect>("DrawingTools/HBoxContainer/ClearButton/Icon");
        _itemButtonApplySettings = new NMapArtistApplyButton(_mapScene, _itemButtonsHboxContainer, icon);
        _itemButtonsHboxContainer.AddChild(_itemButtonApplySettings);
    }

    // private void DisplayGui()
    // {
    //    if (!this.IsVisible())
    //    {
    //        this.Visible = true;
    //    }
    // }
    //
    // private void HideGui()
    // {
    //    if (this.IsVisible())
    //    {
    //        this.Visible = false;
    //    }
    // }

    public void ToggleGui()
    {
       // if (this.IsVisible())
       // {
           // MapArtistDrawingColors.Set(localPlayer, _itemColorPicker.Color);
       // }
       this.Visible = !this.Visible;
       
    }
}