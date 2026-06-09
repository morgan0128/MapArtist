namespace MapArtist.MapArtistCode;

using System;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

[HarmonyPatch(typeof(NMapDrawings), "CreateLineForPlayer", new Type[] { typeof(Player), typeof(bool) })]
public static class MapArtistDrawingPatch
{
    private static void Postfix(Player player, bool isErasing, Line2D __result)
    {
        // NMapColorPickerButton.SetOwner __result.GetOwner();
        if (isErasing || __result == null)
        {
            return;
        }

        if (MapArtistDrawingColors.TryGet(player, out var color))
        {
            __result.DefaultColor = color;
        }

        if (MapArtistDrawingPenWidth.TryGet(player, out var width))
        {
            __result.Width = width;
        }

    }
}