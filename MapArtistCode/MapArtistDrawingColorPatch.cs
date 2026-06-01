namespace MapArtist.MapArtistCode;

using System;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

[HarmonyPatch(typeof(NMapDrawings), "CreateLineForPlayer", new Type[] { typeof(Player), typeof(bool) })]
public static class MapArtistDrawingColorPatch
{
    private static void Postfix(Player player, bool isErasing, Line2D __result)
    {
        if (isErasing || __result == null)
            return;

        if (MapArtistDrawingColors.TryGet(player, out var color))
            __result.DefaultColor = color;
    }
}