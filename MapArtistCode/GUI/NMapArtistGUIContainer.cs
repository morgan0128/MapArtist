using Godot;

namespace MapArtist.MapArtistCode.GUI;

[ScriptPath("res://MapArtistCode/GUI/NMapArtistGUIContainer.cs")]
public partial class NMapArtistGUIContainer : VBoxContainer
{
    public NMapArtistGUIContainer()
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

    // private void OnGuiInput(InputEvent @event)
    // {
    //     MapArtistController.MapArtistController.Instance.DisableDrawingMode();
    // }
    
    
}