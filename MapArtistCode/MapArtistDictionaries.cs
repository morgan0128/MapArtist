using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Entities.Players;

namespace MapArtist.MapArtistCode;

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
    public static void ClearAll(Player? player)
    {
        ColorsByPlayer.Remove(player.NetId);
        WidthsByPlayer.Remove(player.NetId);
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

        ColorsByPlayer[player.NetId] = color;
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
        
        var w =  width ?? 4.0f;
        WidthsByPlayer[player.NetId] = w;
    }

    public static bool TryGetPenWidth(Player player, out float width)
    {
        return WidthsByPlayer.TryGetValue(player.NetId, out width);
    }
    //-------------------------------------------------------------------------------------------------------------------
    
    
}