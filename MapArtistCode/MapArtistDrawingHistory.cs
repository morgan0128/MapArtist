using BaseLib;
using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.MapDrawing;

namespace MapArtist.MapArtistCode;

public sealed class MapArtistDrawingHistory
{
    static MapArtistDrawingHistory()
    {
    }

    private MapArtistDrawingHistory()
    {
    }

    public static MapArtistDrawingHistory Instance { get; } = new MapArtistDrawingHistory();
    
    // private struct CachedLine2DSet
    // {
    //     public List<Line2D>? LineSet; // if LineSet != null then consider each LineSet and ignore Line
    //     public Line2D? Line; // else consider only Line and ignore LineSet
    //     
    //     public CachedLine2DSet(Line2D line)
    //     {
    //         LineSet = null;
    //         Line = line;
    //     }
    //     public CachedLine2DSet(List<Line2D> set)
    //     {
    //         LineSet = set;
    //         Line = null;
    //     }
    // }

    // private static System.Collections.Generic.Dictionary<ulong, Array<Line2D>> _playerDrawings =
        // new System.Collections.Generic.Dictionary<ulong, Array<Line2D>>();

    // private static System.Collections.Generic.Dictionary<ulong, SubViewport> _drawViewports = new System.Collections.Generic.Dictionary<ulong, SubViewport>();
    private SubViewport? _localDrawViewport;
    private Stack<List<Line2D>> _existingLinesCache = new Stack<List<Line2D>>();
    private Stack<List<Line2D>> _deletedLinesCache = new Stack<List<Line2D>>();


    
    // lock "redo line from history" command if:
    //      _localDrawViewport is null
    //      or
    //      the player's _deletedLinesCache is empty
    //      or
    //      for most recent BeginLine where _drawingState.playerId == LocalPlayerId, drew a new line
    // else: unlock
    private bool _localPlayerLastDrew = false;

    private bool RedoLocked()
    {
        return (_localPlayerLastDrew || _localDrawViewport == null || _deletedLinesCache.Count == 0);
    }

    public void NotifyBeginLine(ulong drawingStatePlayerId, SubViewport drawingStateDrawViewport, Line2D line)
    {
        if (drawingStatePlayerId != Util.GetLocalPlayerId()) return;
        
        CheckUpdateLocalViewport(drawingStatePlayerId, drawingStateDrawViewport);
        AddEntry(drawingStatePlayerId, line);
    }

    public void ResetState()
    {
        _localDrawViewport = null;
        foreach (var cache in _existingLinesCache)
        {
            foreach (var line in cache)
            {
                line.QueueFreeSafely();
            }
        }
        _existingLinesCache.Clear();

        foreach (var cache in _deletedLinesCache)
        {
            foreach (var line in cache)
            {
                line.QueueFreeSafely();
            }
        }
        _deletedLinesCache.Clear();
    }

    public void NotifyPlayerCleared(ulong drawingStatePlayerId, SubViewport drawingStateDrawViewport, List<Line2D> linesToSave)
    {
        if (drawingStatePlayerId != Util.GetLocalPlayerId()) return;
        CheckUpdateLocalViewport(drawingStatePlayerId, drawingStateDrawViewport);
        _existingLinesCache.Push(linesToSave);
        // pushed onto _existingLinesCache; a clear operation is denoted by a list of length > 1,
        // and this list as an entry on _existingLinesCache 'exists' as a set of line erases
    }
    
    private void CheckUpdateLocalViewport(ulong playerId, SubViewport svp)
    {
        if (playerId != Util.GetLocalPlayerId()) return;
        
        /* SubViewport svp belongs to local player */
        if (_localDrawViewport != null) return; // Note: ResetRunState() sets _localDrawViewport to null
        
        _localDrawViewport = svp;
    }

    private void RemoveViewport()
    {
        // for where player leaves or game ends... do later... maybe.
        // depends on approach (CheckUpdateLocalViewport)
        
                
    }

    private void AddEntry(ulong playerId, Line2D? line)
    {
        if (_localDrawViewport == null) return;
        if (playerId != Util.GetLocalPlayerId()) return;
        if (line == null) return;

        var list = new List<Line2D> { line };
        _existingLinesCache.Push(list);
        _localPlayerLastDrew = true;
    }

    public void Undo()
    {
        if (_localDrawViewport == null || _existingLinesCache.Count == 0) return;
        
        var cached = _existingLinesCache.Pop();
        if (cached.Count > 1)
        {
            // this entry represents a 'cleared set'
            if (!AddLine(cached)) return; // error, but don't break 'undo history' by looping line back on stack
        }
        else
        {
            // regular undo drawn line operation
            if (!RemoveLine(cached)) return; // error, but don't break 'undo history' by looping line back on stack
        }
        _deletedLinesCache.Push(cached);
        _localPlayerLastDrew = false;

    }
    
    public void Redo()
    {
        if (RedoLocked()) return;
        
        var cached = _deletedLinesCache.Pop();
        if (cached.Count > 1)
        {
            // this entry represents a 'cleared set,' in which the clear operation has been undone
            if (!RemoveLine(cached)) return; // error, but don't break 'redo history' by looping line back on stack
        }
        else
        {
            // regular redo drawn line operation
            if (!AddLine(cached)) return; // error, but don't break 'redo history' by looping line back on stack
        }
        _existingLinesCache.Push(cached);
        _localPlayerLastDrew = false;
    }

    private bool RemoveLine(List<Line2D> toRemove)
    {
        foreach (var line in toRemove)
        {
            _localDrawViewport?.RemoveChildSafely(line);
        }
        
        // TODO
        // send message to perform same operation to other players
        
        return true;
    }
    
    private bool AddLine(List<Line2D> toAdd)
    {
        foreach (var line in toAdd){
            _localDrawViewport?.AddChildSafely((Node) line);
        }
        
        // TODO
        // send message to perform same operation to other players

        return true;
    }

}