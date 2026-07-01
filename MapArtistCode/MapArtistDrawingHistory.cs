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

        public CachedDrawingOperation(bool isClearOperation, Line2D? line = null, List<Line2D>? set = null)
        {
            IsClearOperation = isClearOperation;
            if ((line == null && set == null) || (line != null && set != null)) return;
            if (!IsClearOperation && line == null) return; // A regular draw/erase operation contains one and only one line

            if (!IsClearOperation || Line != null)
            {
                Line = line;
            }
            else
            {
                LineSet = set;
            }
        }

        public bool IsValid()
        {
            return !((Line == null && LineSet == null) || (Line != null && LineSet != null) || 
                     (!IsClearOperation && (Line == null || LineSet != null)));
        }
    }   
    
    private SubViewport? _localDrawViewport;
    private Stack<CachedDrawingOperation> _cachedOperations = new Stack<CachedDrawingOperation>();
    private Stack<CachedDrawingOperation> _cachedUndoneOperations = new Stack<CachedDrawingOperation>();
    
    private bool _localPlayerLastDrew = false;

    private bool UndoLocked()
    {
        return (_localDrawViewport == null || _cachedOperations.Count == 0);
    }
    private bool RedoLocked()
    {
        return (_localPlayerLastDrew || _localDrawViewport == null || _cachedUndoneOperations.Count == 0);
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
    
    private void CheckUpdateLocalViewport(ulong playerId, SubViewport svp)
    {
        if (playerId != Util.GetLocalPlayerId()) return;
        
        /* SubViewport svp belongs to local player */
        if (_localDrawViewport != null) return; // Note: ResetRunState() sets _localDrawViewport to null
        
        _localDrawViewport = svp;
    }
    
    public void NotifyBeginLine(ulong drawingStatePlayerId, SubViewport drawingStateDrawViewport, Line2D line)
    {
        if (drawingStatePlayerId != Util.GetLocalPlayerId()) return;
        
        CheckUpdateLocalViewport(drawingStatePlayerId, drawingStateDrawViewport);
        CacheOperationDrewLine(drawingStatePlayerId, line);
    }
    
    private CachedDrawingOperation? CacheOperationDrewLine(ulong playerId, Line2D? line)
    {
        if (_localDrawViewport == null) return null;
        if (playerId != Util.GetLocalPlayerId()) return null;
        if (line == null) return null;
        
        var operation = new CachedDrawingOperation(false, line);
        
        _cachedOperations.Push(operation);
        _localPlayerLastDrew = true;
        return operation;
    }
    
    private void ViewportAddLine(List<Line2D> toAdd)
    {
        foreach (var line in toAdd)
        {
            ViewportAddLine(line);
        }
    }
    
    private void ViewportAddLine(Line2D toAdd)
    {
        _localDrawViewport?.AddChildSafely((Node) toAdd);
        
        // TODO
        // send message to perform same operation to other players

        return;
    }
    
    private void ViewportRemoveLine(List<Line2D> toRemove)
    {
        foreach (var line in toRemove)
        {
            ViewportRemoveLine(line);
        }
    }
    
    private void ViewportRemoveLine(Line2D toRemove)
    {
        _localDrawViewport?.RemoveChildSafely((Node) toRemove);
        
        // TODO
        // send message to perform same operation to other players

        return;
    }
    
    public void NotifyPlayerCleared(ulong drawingStatePlayerId, SubViewport drawingStateDrawViewport, List<Line2D> linesToCache)
    {
        if (drawingStatePlayerId != Util.GetLocalPlayerId() || linesToCache.Count == 0) return;

        CheckUpdateLocalViewport(drawingStatePlayerId, drawingStateDrawViewport);
        // var reverseOrder = new Stack<CachedDrawingOperation>();
        // while (_cachedUndoneOperations.Count > 0)
        // {
        //     reverseOrder.Push(_cachedUndoneOperations.Pop());
        // }
        // while (reverseOrder.Count > 0)
        // {
        //     _cachedOperations.Push(reverseOrder.Pop());
        // }

        if (linesToCache.Count == 1)
        {
            // for sake of memory management
            var line = linesToCache[0];
            var operation = new CachedDrawingOperation(true, line);
            _cachedOperations.Push(operation);
        }
        else
        {
            var operation = new CachedDrawingOperation(true, null, linesToCache);
            _cachedOperations.Push(operation);
        }
        
    }

    public void Undo()
    {
        if (UndoLocked()) return;
        var operation = _cachedOperations.Pop();
        if (!operation.IsValid()) return;
        
        if (operation.IsClearOperation)
        {
            if (operation.Line != null)
            {
                ViewportAddLine(operation.Line);
            }
            else
            {
                ViewportAddLine(operation.LineSet!);
            }
            
            _cachedUndoneOperations.Push(operation);
        }
        else
        {
            if (operation.Line != null)
            {
                ViewportRemoveLine(operation.Line);
                _cachedUndoneOperations.Push(operation);
            }
        }
        _localPlayerLastDrew = false;

    }
    
    public void Redo()
    {
        if (RedoLocked()) return;
        var operation = _cachedUndoneOperations.Pop();
        if (!operation.IsValid()) return;
        
        if (operation.IsClearOperation)
        {
            // this entry represents a 'cleared set,' in which the clear operation has been undone
            if (operation.Line != null)
            {
                ViewportRemoveLine(operation.Line);   
            }
            else
            {
                ViewportRemoveLine(operation.LineSet!);
            }

        }
        else
        {
            ViewportAddLine(operation.Line!);
        }
        _cachedOperations.Push(operation);
        _localPlayerLastDrew = false;
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