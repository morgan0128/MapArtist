using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MapArtist.MapArtistCode;

[ScriptPath("res://MapArtistCode/NMapArtistGUI.cs")]
public partial class NMapArtistGUI : VBoxContainer
{
    // private VBoxContainer _childContainer;

    private NColorPicker _itemColorPicker;

    private HBoxContainer _itemButtonsHboxContainer;
    private Button _itemButtonPenSettings;
    private Button _itemButtonApplySettings;
    
    public NMapArtistGUI(Texture2D buttonTexture)
    {
      Name = "NMapArtistGUI";
      UniqueNameInOwner = true;
      Visible = false; 
      LayoutMode = 2;
      SetAnchorsPreset(LayoutPreset.TopLeft);
      GlobalPosition = new Vector2(250f, 250f);
      CustomMinimumSize = new Vector2(1000.0f, 1000.0f);
      
      // ----- init this child and descendents -----
      // _childContainer =  InitVbContainer();
      
      _itemColorPicker = new NColorPicker();
      this.AddChild(_itemColorPicker);
      
      
      
      _itemButtonsHboxContainer =  InitHBoxContainer();
      _itemButtonPenSettings =  InitGeneric_Button("generic_button1", buttonTexture);
      _itemButtonApplySettings = InitGeneric_Button("generic_button2", buttonTexture);
      _itemButtonsHboxContainer.AddChild(_itemButtonPenSettings);
      _itemButtonsHboxContainer.AddChild(_itemButtonApplySettings);
      
      this.AddChild(_itemButtonsHboxContainer);

      // this.AddChild(vb);
      // vb.AddChild(_colorPicker);
      // vb.AddChild(hb);
      // hb.AddChild(button1);
    }

    // private static VBoxContainer InitVbContainer()
    // {
    //   var vbc = new VBoxContainer();
    //   vbc.Name = "VBC_MapArtistGUI";
    //   vbc.UniqueNameInOwner = true;
    //   vbc.LayoutMode = 2;
    //   vbc.SetAnchorsPreset(LayoutPreset.TopLeft);
    //   
    //   return vbc;
    // }

    private static HBoxContainer InitHBoxContainer()
    {
      var hbc = new HBoxContainer();
      hbc.Name = "HBC_MapArtistGUI";
      hbc.UniqueNameInOwner = true;
      hbc.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;

      return hbc;
    }

    private static Button InitGeneric_Button(StringName name, Texture2D texture)
    {
      var nb = new Button();
      nb.Name = name;
      nb.UniqueNameInOwner = true;
      nb.AddThemeIconOverride("generic", texture);
      // nb.AddThemeIconOverride("genric_icon",PreloadManager.Cache.GetTexture2D("res://images/packed/map/drawing_clear.png"));

      return nb;
    }

    // public static readonly AddedNode<NMapScreen, NMapArtistGUI> Map = new((mapScreen) =>
    // {
      // var gui = new NMapArtistGUI();
      // gui.SetGlobalPosition(new Vector2(250f, 250f));
      
      // gui.AddChild(gui._childContainer);
      // var child = gui.GetNode<VBoxContainer>("VBC_MapArtistGUI");
      
      // child.AddChild(gui._itemColorPicker);
      
      // child.AddChild(gui._itemButtonsHboxContainer);

      // var descendentButtonsContainer1 = child.GetNode<HBoxContainer>("HBC_MapArtistGUI");
      // descendentButtonsContainer1.AddChild(gui._itemButtonPenSettings);
      // descendentButtonsContainer1.AddChild(gui._itemButtonApplySettings);
      
      
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
      
      // mapScreen.AddChild(gui);
      
      // return gui;
    // });
    
    public override void _Ready()
    {
        
    }

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
           MapArtistDrawingColors.Set(localPlayer, _itemColorPicker.Color);
       }
       this.Visible = !this.Visible;
       
    }
}