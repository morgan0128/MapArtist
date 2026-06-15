using Godot;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MapArtist.MapArtistCode;

public abstract partial class NMapArtistButton : NButton
{
    protected TextureRect? Icon;
    public void SetIcon(TextureRect icon)
    {
        Icon = icon;
    }
}