// using Godot;
// using Godot.Collections;
//
// namespace MapArtist.MapArtistCode;
// using MegaCrit.Sts2.Core.Entities.Players;
//
// public static class MapArtistDrawingPenWidth
// {
//     private static readonly Godot.Collections.Dictionary<ulong, float> WidthsByPlayer = new();
//
//     public static void Set(Player? player, float? width)
//     {
//         if (player == null)
//         {
//             GD.PushWarning("MapArtist: tried to set a map drawing color before the local player was available.");
//             return;
//         }
//         
//         var w =  width ?? 3.0f; // default == 3.0f
//         WidthsByPlayer[player.NetId] = w;
//     }
//
//     public static bool TryGet(Player player, out float width)
//     {
//         return WidthsByPlayer.TryGetValue(player.NetId, out width);
//     }
// }