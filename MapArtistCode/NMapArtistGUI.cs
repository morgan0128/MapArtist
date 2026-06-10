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
    // private NMapScreen _mapScene;
    
    // private NColorPicker _rowColorPicker;
    
    // private HBoxContainer _rowButtonsHboxContainer;
    // private HBoxContainer _rowButtonsHboxContainer2;
    
    // public NButton _itemButtonPenSettings;
    // public NButton _itemButtonApplySettings;
    private int _genericButtonCounter = 0;

    private static readonly string ScenePath = "res://scenes/screens/map/map_screen.tscn";

    
    // public NMapArtistGUI(NMapScreen mapScene)
    public NMapArtistGUI()

    {
      Name = "MapArtistGUI";
      UniqueNameInOwner = true;
      Visible = false; 
      LayoutMode = 2;
      SetAnchorsPreset(LayoutPreset.TopLeft);
      GlobalPosition = new Vector2(50f, 150f);
      
      // // GUI row 1: color picker
      // _rowColorPicker = new NColorPicker();
      // _rowColorPicker.Name = "ItemColorPicker";
      // _rowColorPicker.UniqueNameInOwner = true;
      // this.AddChild(_rowColorPicker);
      //
      // // GUI row 2: additional brush property (buttons)
      // _rowButtonsHboxContainer =  InitHBoxContainer();
      // _rowButtonsHboxContainer.Name = "BrushPropertyButtonContainer";
      // this.AddChild(_rowButtonsHboxContainer);
      //
      // // GUI row 3: apply/reset brush color and properties
      // _rowButtonsHboxContainer2 =  InitHBoxContainer();
      // _rowButtonsHboxContainer2.Name = "ApplyResetButtonContainer";
      // this.AddChild(_rowButtonsHboxContainer2);
      
    }

    // private static HBoxContainer InitHBoxContainer()
    // {
    //   var hbc = new HBoxContainer();
    //   // hbc.Name = "HBC_MapArtistGUI";
    //   hbc.UniqueNameInOwner = true;
    //   hbc.SizeFlagsHorizontal = SizeFlags.Fill;
    //   hbc.SizeFlagsVertical = SizeFlags.Fill;
    //   
    //   return hbc;
    // }
    
    public override void _Ready()
    {
        // var icon = _mapScene.GetNode<TextureRect>("DrawingTools/HBoxContainer/ClearButton/Icon");
        // _itemButtonApplySettings = new NMapArtistApplyButton(_mapScene, _rowButtonsHboxContainer, icon);
        // _rowButtonsHboxContainer.AddChild(_itemButtonApplySettings);
        
        
        
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
       this.Visible = !this.Visible;
    }
}