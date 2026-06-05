using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MapArtist.MapArtistCode;

public partial class NMapArtistGUI : Control
{
    
    private static NMapArtistGUI instance;

    public static NMapArtistGUI Instance()
    {
        return instance;
    }
    
   private NMapArtistGUI()
   {
      Name = "NMapArtistGUI";
      UniqueNameInOwner = true;
      Visible = false;
   }

   private static VBoxContainer InitVBContainer()
   {
      VBoxContainer vbc = new VBoxContainer();
      vbc.Name = "VBC_MapArtistGUI";
      vbc.UniqueNameInOwner = true;
      vbc.LayoutMode = 2;
      vbc.SetAnchorsPreset(LayoutPreset.TopLeft);
      
      return vbc;
   }

   private static HBoxContainer InitHBoxContainer()
   {
      HBoxContainer hbc = new HBoxContainer();
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
      
      var vb =  InitVBContainer();
      var hb =  InitHBoxContainer();
      var button1 =  InitGeneric_NButton();
      // var button2 =  InitGeneric_NButton();
      var colorPicker = NColorPicker.Instance();

      gui.AddChild(vb);
      vb.AddChild(colorPicker);
      vb.AddChild(hb);
      hb.AddChild(button1);
      
      return gui;
   });
   
   public static void displayGUI()
   {
       if (!instance.IsVisible())
       {
           instance.Visible = true;
       }
   }
   
   public static void hideGUI()
   {
       if (instance.IsVisible())
       {
           instance.Visible = false;
       }
   }
   
   public static void toggleGUI(Player? localPlayer)
   {
       if (instance.IsVisible())
       {
           MapArtistDrawingColors.Set(localPlayer, NColorPicker.Instance().Color);
       }
       instance.Visible = !instance.Visible;
       
   }
}