using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MapArtist.MapArtistCode;

[ScriptPath("res://MapArtistCode/NMapArtistGUI.cs")]
public partial class NMapArtistGUI : Control
{
    
    private NColorPicker _colorPicker;
    
    // public static NMapArtistGUI Instance { get; } = new NMapArtistGUI();
    // public static NMapArtistGUI? Instance()
    // {
    //     return instance;
    // }
    
   private NMapArtistGUI()
   {
      Name = "NMapArtistGUI";
      UniqueNameInOwner = true;
      Visible = false;
      CustomMinimumSize = new Vector2(1000.0f, 1000.0f);
      // instance = this;
      var vb =  InitVbContainer();
      var hb =  InitHBoxContainer();
      var button1 =  InitGeneric_NButton();
      // var button2 =  InitGeneric_NButton();
      _colorPicker = new NColorPicker();

      this.AddChild(vb);
      vb.AddChild(_colorPicker);
      vb.AddChild(hb);
      hb.AddChild(button1);
   }

   private static VBoxContainer InitVbContainer()
   {
      var vbc = new VBoxContainer();
      vbc.Name = "VBC_MapArtistGUI";
      vbc.UniqueNameInOwner = true;
      vbc.LayoutMode = 2;
      vbc.SetAnchorsPreset(LayoutPreset.TopLeft);
      
      return vbc;
   }

   private static HBoxContainer InitHBoxContainer()
   {
      var hbc = new HBoxContainer();
      hbc.Name = "HBC_MapArtistGUI";
      hbc.UniqueNameInOwner = true;
      hbc.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;

      return hbc;
   }

   private static NButton InitGeneric_NButton()
   {
      var nb = new NButton();
      nb.AddThemeIconOverride("genric_icon",PreloadManager.Cache.GetTexture2D("res://images/packed/map/drawing_clear.png"));

      return nb;
   }

   public static readonly AddedNode<NMapScreen, NMapArtistGUI> Map = new((mapScreen) =>
   {
      var gui = new NMapArtistGUI();
      
      // var vb =  InitVbContainer();
      // var hb =  InitHBoxContainer();
      // var button1 =  InitGeneric_NButton();
      // // var button2 =  InitGeneric_NButton();
      // var colorPicker = NColorPicker.Instance;
      //
      // gui.AddChild(vb);
      // vb.AddChild(colorPicker);
      // vb.AddChild(hb);
      // hb.AddChild(button1);
      
      mapScreen.AddChild(gui);
      
      return gui;
   });
   
   private void DisplayGui()
   {
       if (!this.IsVisible())
       {
           this.Visible = true;
       }
   }
   
   private void HideGui()
   {
       if (this.IsVisible())
       {
           this.Visible = false;
       }
   }
   
   public void ToggleGui(Player? localPlayer)
   {
       if (this.IsVisible())
       {
           MapArtistDrawingColors.Set(localPlayer, _colorPicker.Color);
       }
       this.Visible = !this.Visible;
       
   }
}