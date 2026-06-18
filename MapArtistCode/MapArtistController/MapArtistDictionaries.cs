using Godot;
using MegaCrit.Sts2.Core.Entities.Players;

namespace MapArtist.MapArtistCode.MapArtistController;

public class MapArtistDictionaries
{
    private static readonly Godot.Collections.Dictionary<ulong, Color> ColorsByPlayer = new();
    
    private static readonly Godot.Collections.Dictionary<ulong, float> WidthsByPlayer = new();
    
    
    /*
     * Operations:
     *  Global
     *  Color
     *  PenWidth
     */
    
    //----------------------------------------------------- Global ------------------------------------------------------
    public static void ClearAll()
    {
        ColorsByPlayer.Clear();
        WidthsByPlayer.Clear();
    }

    public static void ClearAll(Player? player)
    {
        if (player == null)
        {
            GD.PushWarning("MapArtist: tried to clear map drawing settings before the local player was available.");
            return;
        }

        ClearAll(player.NetId);
    }

    public static void ClearAll(ulong netId)
    {
        ColorsByPlayer.Remove(netId);
        WidthsByPlayer.Remove(netId);
    }
    //-------------------------------------------------------------------------------------------------------------------

    
    //------------------------------------------------------ Color ------------------------------------------------------
    public static void SetColor(Player? player, Color color)
    {
        if (player == null)
        {
            GD.PushWarning("MapArtist: tried to set a map drawing color before the local player was available.");
            return;
        }

        SetColor(player.NetId, color);
    }

    public static void SetColor(ulong netId, Color color)
    {
        ColorsByPlayer[netId] = color;
    }

    public static bool TryGetColor(Player player, out Color color)
    {
        return ColorsByPlayer.TryGetValue(player.NetId, out color);
    }
    //-------------------------------------------------------------------------------------------------------------------
    
    
    //---------------------------------------------------- PenWidth -----------------------------------------------------
    public static void SetPenWidth(Player? player, float? width)
    {
        if (player == null)
        {
            GD.PushWarning("MapArtist: tried to set a map drawing color before the local player was available.");
            return;
        }
        
        SetPenWidth(player.NetId, width ?? 4.0f);
    }

    public static void SetPenWidth(ulong netId, float width)
    {
        WidthsByPlayer[netId] = width;
    }

    public static bool TryGetPenWidth(Player player, out float width)
    {
        return WidthsByPlayer.TryGetValue(player.NetId, out width);
    }
    //-------------------------------------------------------------------------------------------------------------------
    
    
}
