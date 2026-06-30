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
    
    private struct CachedDrawingOperation
    {
        // proper usage: between Line and LineSet, one and only one should be null
        public readonly Line2D? Line = null;
        public readonly List<Line2D>? LineSet = null;

        public readonly bool IsClearOperation;

        public CachedDrawingOperation(bool isClearOperation, Line2D? line, List<Line2D>? set = null)
        {
            IsClearOperation = isClearOperation;
            if ((line == null && set == null) || (line != null && set != null) ||
                (!IsClearOperation && line == null)) return;

            if (!IsClearOperation || Line != null)
            {
                Line = line;
            }
            else
            {
                LineSet = set;
            }
        }
    }
    
    private SubViewport? _localDrawViewport;
    private Stack<CachedDrawingOperation> _cachedOperations = new Stack<CachedDrawingOperation>();
    private Stack<CachedDrawingOperation> _cachedUndoneOperations = new Stack<CachedDrawingOperation>();

    public bool PerformingOpClear = false;


    
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
        return (_localPlayerLastDrew || _localDrawViewport == null || _cachedUndoneOperations.Count == 0);
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
        foreach (var operation in _cachedOperations)
        {
            QueueFreeAllLines(operation);
        }
        _cachedOperations.Clear();

        foreach (var operation in _cachedUndoneOperations)
        {
            QueueFreeAllLines(operation);
        }
        _cachedUndoneOperations.Clear();
    }

    public void NotifyPlayerCleared(ulong drawingStatePlayerId, SubViewport drawingStateDrawViewport, List<Line2D> linesToSave)
    {
        if (drawingStatePlayerId != Util.GetLocalPlayerId() || linesToSave.Count == 0) return;
        
        PerformingOpClear = true;
        CheckUpdateLocalViewport(drawingStatePlayerId, drawingStateDrawViewport);
        if (linesToSave.Count == 1)
        {
            // for sake of memory management
            var line = linesToSave[0];
            var operation = new CachedDrawingOperation(PerformingOpClear, line);
            _cachedOperations.Push(operation);
        }
        else
        {
            var operation = new CachedDrawingOperation(PerformingOpClear, null, linesToSave);
            _cachedOperations.Push(operation);
        }
        PerformingOpClear = false;
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
        if (_localDrawViewport == null || _cachedOperations.Count == 0) return;
        
        var operation = _cachedOperations.Pop();
        if (operation.IsClearOperation)
        {
            if (operation.Line != null)
            {
                AddLine(operation.Line);
            }
            else if (operation.LineSet != null)
            {
                foreach (var line in operation.LineSet)
                {
                    AddLine(line);
                }
            }
            else
            {
                return;
            }

            _cachedUndoneOperations.Push(operation);
        }
        else
        {
            if (operation.Line != null)
            {
                // TODO begin at line 173
            }
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
        bool failure = false;
        foreach (var line in toAdd)
        {
            if (!AddLine(line))
            {
                failure = true;
            }
        }

        return !failure;
    }
    
    private bool AddLine(Line2D toAdd)
    {
        _localDrawViewport?.AddChildSafely((Node) toAdd);
        
        // TODO
        // send message to perform same operation to other players

        return true;
    }
    
    // helper
    private void QueueFreeAllLines(CachedDrawingOperation operation)
    {
        if (operation.Line != null)
        {
            operation.Line.QueueFreeSafely();
        }
        else if (operation.LineSet != null)
        {
            foreach (var line in operation.LineSet)
            {
                line.QueueFreeSafely();
            }
        }
    }

}