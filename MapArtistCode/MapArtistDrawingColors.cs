using Godot;
using Godot.Collections;

namespace MapArtist.MapArtistCode;
using MegaCrit.Sts2.Core.Entities.Players;

public static class MapArtistDrawingColors
{
    private static readonly Godot.Collections.Dictionary<ulong, Color> ColorsByPlayer = new();

    public static void Set(Player? player, Color color)
    {
        if (player == null)
        {
            GD.PushWarning("MapArtist: tried to set a map drawing color before the local player was available.");
            return;
        }

        ColorsByPlayer[player.NetId] = color;
    }

    public static bool TryGet(Player player, out Color color)
    {
        return ColorsByPlayer.TryGetValue(player.NetId, out color);
    }
}
