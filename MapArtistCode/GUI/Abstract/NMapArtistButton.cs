using Godot;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MapArtist.MapArtistCode.GUI.Abstract;

public abstract partial class NMapArtistButton : NButton
{
    protected TextureRect? Icon;
    public void SetIcon(TextureRect icon)
    {
        Icon = icon;
    }

    // public override void _Ready()
    // {
    //     this.MousePressed += OnMousePressed;
    // }
    //
    // private void OnMousePressed(InputEvent @event)
    // {
    //         MapArtistController.MapArtistController.Instance.DisableDrawingMode();
    // }

}