using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace MapArtist.MapArtistCode;

// A linear history for a player's map drawings; performing an operation (draw, erase, or clear) overwrites (erases) all redo history
public sealed class MapArtistLocalDrawingHistory
{
    static MapArtistLocalDrawingHistory() {}
    private MapArtistLocalDrawingHistory() {}
    public static MapArtistLocalDrawingHistory Instance { get; } = new MapArtistLocalDrawingHistory();
    
    private readonly struct CachedDrawingOperation(bool isClearOperation, Line2D? line = null, List<Line2D>? set = null)
    {
        // proper usage: between Line and LineSet, one and only one should be null
        public Line2D? Line => IsValid ? line : null;
        public List<Line2D>? LineSet => IsValid ? set : null;
        public bool IsClearOperation => isClearOperation;
        public bool IsValid => !((line == null && (set == null || set.Count == 0)) || (line != null && set != null) || (!isClearOperation && (line == null || set != null)));
    }   
    
    private SubViewport? _localDrawViewport;
    private readonly Stack<CachedDrawingOperation> _cachedOperations = new Stack<CachedDrawingOperation>();
    private readonly Stack<CachedDrawingOperation> _cachedUndoneOperations = new Stack<CachedDrawingOperation>();

    private bool UndoLocked()
    {
        return (_localDrawViewport == null || _cachedOperations.Count == 0);
    }
    private bool RedoLocked()
    {
        return (_localDrawViewport == null || _cachedUndoneOperations.Count == 0);
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
    
    // To be called only within BeginLine postfix patch.
    public void PatchNotifyBeginLine(ulong drawingStatePlayerId, SubViewport drawingStateDrawViewport, Line2D line)
    {
        if (drawingStatePlayerId != Util.GetLocalPlayerId()) return;
        CheckUpdateLocalViewport(drawingStatePlayerId, drawingStateDrawViewport);
        LocalPlayerOperationDrewOrErased(line);
    }
    
    // To be called only within ClearAllLinesForPlayer prefix patch.
    public void PatchNotifyClearAllLinesForPlayer(ulong drawingStatePlayerId, SubViewport drawingStateDrawViewport, List<Line2D> linesToCache)
    {
        if (drawingStatePlayerId != Util.GetLocalPlayerId()) return;
        CheckUpdateLocalViewport(drawingStatePlayerId, drawingStateDrawViewport);
        LocalPlayerOperationCleared(linesToCache);
    }
    
    private void LocalPlayerOperationDrewOrErased(Line2D line)
    {
        _cachedUndoneOperations.Clear();
        _cachedOperations.Push(new CachedDrawingOperation(false, line));
    }
    
    // Reusable, with caution; consider parameter calledFromRedo 
    private void LocalPlayerOperationCleared(List<Line2D> linesToCache, bool calledFromRedo = false)
    {
        if (linesToCache.Count == 0) return; // ; ignore this operation
        if (!calledFromRedo)
        {
            _cachedUndoneOperations.Clear();
        }
        
        // We recover/rebuild cleared (i.e., deleted by ClearAllLinesForPlayer) draw history through the saved data in the cached clear operations
        var cachedOperationsList = new List<CachedDrawingOperation>(_cachedOperations.Count);
        while (_cachedOperations.Count > 0)
        {
            cachedOperationsList.Insert(0, _cachedOperations.Pop());
        }
        for (var i = 0; i < cachedOperationsList.Count; i++)
        {
            if (cachedOperationsList[i].IsClearOperation)
            {
                _cachedOperations.Push(cachedOperationsList[i]);
            }
        }
        
        if (linesToCache.Count == 1)
        {
            // for sake of memory (micro)management
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
    
    private void CheckUpdateLocalViewport(ulong playerId, SubViewport svp)
    {
        if (playerId != Util.GetLocalPlayerId()) return;
        
        /* SubViewport svp belongs to local player */
        if (_localDrawViewport != null) return; // Note: ResetRunState() sets _localDrawViewport to null
        _localDrawViewport = svp;
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

    public void Undo()
    {
        if (UndoLocked()) return;
        var operation = _cachedOperations.Pop();
        if (!operation.IsValid) return;
        
        if (operation.IsClearOperation)
        {
            // rebuild: re-add the set of all lines cleared,
            // then rebuild line history (clumsy implementation: chronology was maintained unintentionally by current Vanilla game logic).
            /*
                If vanilla logic is changed to be incompatible with approach in future update, -->must<-- be fixed with harmony patching...
               The nature of game updates breaking game logic is unavoidable and map drawing feature seems unlikely to be expanded upon/altered
               by devs anytime soon, given my read on MegaCrit FAQs. 
            */
            if (operation.Line != null)
            {
                ViewportAddLine(operation.Line);
                var redrawOperation = new CachedDrawingOperation(false, operation.Line);
                _cachedOperations.Push(redrawOperation);
            } else if (operation.LineSet != null) 
            {
                ViewportAddLine(operation.LineSet);
                for (var i = 0; i < operation.LineSet.Count; i++)
                {
                    var redrawOperation = new CachedDrawingOperation(false, operation.LineSet[i]);
                    _cachedOperations.Push(redrawOperation);
                }
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
    }
    
    public void Redo()
    {
        if (RedoLocked()) return;
        var operation = _cachedUndoneOperations.Pop();
        if (!operation.IsValid) return;
        
        if (operation.IsClearOperation)
        {
            // this operation contains a 'cleared set,' in which the clear operation has been undone
            var lineSet = new List<Line2D>();
            if (operation.Line != null)
            {
                ViewportRemoveLine(operation.Line);
                lineSet.Add(operation.Line);
            }
            else if (operation.LineSet != null)
            {
                ViewportRemoveLine(operation.LineSet);
                lineSet = operation.LineSet;
            }

            LocalPlayerOperationCleared(lineSet, true);
        }
        else
        {
            if (operation.Line == null) return;
            ViewportAddLine(operation.Line);
            _cachedOperations.Push(operation);
        }
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