using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.MapDrawing;

namespace MapArtist.MapArtistCode;

public sealed class MapArtistDrawingHistories
{
    static MapArtistDrawingHistories()
    {
    }

    private MapArtistDrawingHistories()
    {
    }

    public static MapArtistDrawingHistories Instance { get; } = new MapArtistDrawingHistories();

    private static System.Collections.Generic.Dictionary<ulong, Array<Line2D>> _playerDrawings =
        new System.Collections.Generic.Dictionary<ulong, Array<Line2D>>();

    private static System.Collections.Generic.Dictionary<ulong, SubViewport> _drawViewports = new System.Collections.Generic.Dictionary<ulong, SubViewport>();

    public void CheckUpdateDrawViewports(ulong playerId, SubViewport svp)
    {
        if (_drawViewports.ContainsKey(playerId))
        {
            return;
        }
        else
        {
            _drawViewports.Add(playerId, svp);
        }
    }

    private void RemoveViewport()
    {
        // for where player leaves or game ends... do later
    }

    public void AddEntry(ulong playerId, Line2D? line)
    {
        if (line == null) return;
        if (!_playerDrawings.ContainsKey(playerId))
        {
            var createdArray = new Array<Line2D> { line };
            _playerDrawings.Add(playerId, createdArray);
        }
        else
        {
            var existingArray = _playerDrawings.GetValueOrDefault(playerId);
            existingArray.Add(line);
        }
    }

    private void RemoveEntry(ulong playerId, int index)
    {
        if (index < 0) return;
        if (!_playerDrawings.ContainsKey(playerId)) return;
        var array = _playerDrawings.GetValueOrDefault(playerId);
        if (array == null || index > array.Count) return;

        var line = array[index];
        
        // RunManager.Instance.DebugOnlyGetState().Get
        // Player

        foreach (SubViewport subVp in _drawViewports.Values)
        {
            subVp.RemoveChild(line);
        }
        
        array?.RemoveAt(index);
    }

    public void Undo(ulong playerId)
    {
        if (!_playerDrawings.ContainsKey(playerId)) return;
        var array = _playerDrawings.GetValueOrDefault(playerId);
        if (array == null) return;
        
        RemoveEntry(playerId, (array.Count - 1));
    }

}