using Godot;

namespace MapArtist.MapArtistCode;
using MegaCrit.Sts2.Core.Entities.Players;

public static class MapArtistDrawingColors
{
    private static readonly Dictionary<ulong, Color> ColorsByPlayer = new();

    public static void Set(Player player, Color color)
    {
        ColorsByPlayer[player.NetId] = color;
    }

    public static bool TryGet(Player player, out Color color)
    {
        return ColorsByPlayer.TryGetValue(player.NetId, out color);
    }
}