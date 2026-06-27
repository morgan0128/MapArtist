using Godot;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MapArtist.MapArtistCode.GUI.Items.Abstract;

public abstract partial class NMapArtistButton : NButton
{
    protected TextureRect? Icon;
    public void SetIcon(TextureRect icon)
    {
        Icon = icon;
    }

    public override void _Ready()
    {
        SetVSizeFlags(SizeFlags.ShrinkBegin);
    }
    //
    // private void OnMousePressed(InputEvent @event)
    // {
    //         MapArtistController.MapArtistController.Instance.DisableDrawingMode();
    // }

}