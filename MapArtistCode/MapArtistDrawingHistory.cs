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

    // private static System.Collections.Generic.Dictionary<ulong, Array<Line2D>> _playerDrawings =
        // new System.Collections.Generic.Dictionary<ulong, Array<Line2D>>();

    // private static System.Collections.Generic.Dictionary<ulong, SubViewport> _drawViewports = new System.Collections.Generic.Dictionary<ulong, SubViewport>();
    private SubViewport? _localDrawViewport;
    private Stack<Line2D> _existingLinesCache = new Stack<Line2D>();
    private Stack<Line2D> _deletedLinesCache = new Stack<Line2D>();
    
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
        foreach (var line in _existingLinesCache) line.QueueFreeSafely();
        _existingLinesCache.Clear();

        foreach (var line in _deletedLinesCache) line.QueueFreeSafely();
        _deletedLinesCache.Clear();
    }

    public void NotifyPlayerCleared(ulong drawingStatePlayerId, SubViewport drawingStateDrawViewport, List<Line2D> linesToSave)
    {
        return; // taking a break but testing before commit
        if (drawingStatePlayerId != Util.GetLocalPlayerId()) return;
        CheckUpdateLocalViewport(drawingStatePlayerId, drawingStateDrawViewport);

        var numCleared = 0;
        foreach (var line in linesToSave)
        {
            _deletedLinesCache.Push(line);
            _localPlayerLastDrew = false;
            numCleared++;
        }
        
        
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

    public void AddEntry(ulong playerId, Line2D? line)
    {
        if (_localDrawViewport == null) return;
        if (playerId != Util.GetLocalPlayerId()) return;
        if (line == null) return;
        
        _existingLinesCache.Push(line);
        _localPlayerLastDrew = true;
    }

    public void Undo()
    {
        if (_localDrawViewport == null || _existingLinesCache.Count == 0) return;
        var line = _existingLinesCache.Pop();

        if (!RemoveLineFromUndo(line)) return; // error, but don't break 'undo history' by looping line back on stack
        
        _deletedLinesCache.Push(line);
        _localPlayerLastDrew = false;
    }
    
    public void Redo()
    {
        if (RedoLocked()) return;
        var line = _deletedLinesCache.Pop();

        if (!AddLineFromRedo(line)) return; // error, but don't break 'redo history' by looping line back on stack

        _existingLinesCache.Push(line);
        _localPlayerLastDrew = false;
    }

    private bool RemoveLineFromUndo(Line2D? toRemove)
    {
        if (toRemove == null) return false;
        _localDrawViewport?.RemoveChildSafely(toRemove);
        
        // TODO
        // send message to perform same operation to other players
        
        return true;
    }
    
    private bool AddLineFromRedo(Line2D? toAdd)
    {
        if (toAdd == null) return false;
        _localDrawViewport?.AddChildSafely((Node) toAdd);
        
        // TODO
        // send message to perform same operation to other players

        return true;
    }

}