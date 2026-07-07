using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace MapArtist.MapArtistCode.DrawingHistory;

// A linear history for a player's map drawings; performing an operation (draw, erase, or clear) overwrites (erases) all redo history
public sealed class MapArtistLocalDrawingHistory
{
    static MapArtistLocalDrawingHistory() {}
    private MapArtistLocalDrawingHistory() {}
    public static MapArtistLocalDrawingHistory Instance { get; } = new MapArtistLocalDrawingHistory();
    
    // private readonly struct CachedDrawingOperation(bool isClearOperation, Line2D? line = null, List<Line2D>? set = null)
    // {
    //     // proper usage: between Line and LineSet, one and only one should be null
    //     public Line2D? Line => IsValid ? line : null;
    //     public List<Line2D>? LineSet => IsValid ? set : null;
    //     public bool IsClearOperation => isClearOperation;
    //     public bool IsValid => !((line == null && (set == null || set.Count == 0)) || (line != null && set != null) || (!isClearOperation && (line == null || set != null)));
    // }   
    //
    // private SubViewport? _localDrawData.LocalDrawViewport;
    // private readonly Stack<CachedDrawingOperation> _localDrawData.CachedOperations = new Stack<CachedDrawingOperation>();
    // private readonly Stack<CachedDrawingOperation> _localDrawData.CachedUndoneOperations = new Stack<CachedDrawingOperation>();

    private readonly DrawHistoryData _localDrawData = new DrawHistoryData(null);
    
    // private readonly MapArtistNetDrawingHistories _netDrawData = new MapArtistNetDrawingHistories();
    private static readonly Dictionary<ulong, DrawHistoryData> NetDrawHistories = new();

    // private bool UndoLocked()
    // {
    //     return (_localDrawData.DrawViewport == null || _localDrawData.CachedOperations.Count == 0);
    // }
    // private bool RedoLocked()
    // {
    //     return (_localDrawData.DrawViewport == null || _localDrawData.CachedUndoneOperations.Count == 0);
    // }

    public void ResetState()
    {
        _localDrawData.DrawViewport = null;
        foreach (var operation in _localDrawData.CachedOperations)
        {
            QueueFreeAllLines(operation);
        }
        _localDrawData.CachedOperations.Clear();

        foreach (var operation in _localDrawData.CachedUndoneOperations)
        {
            QueueFreeAllLines(operation);
        }
        _localDrawData.CachedUndoneOperations.Clear();
        if (NetDrawHistories.Count == 0) return;
        
        // if multiplayer entries
        var netEnumerator = NetDrawHistories.AsEnumerable().GetEnumerator();
        while (netEnumerator.MoveNext())
        {
            var entry = netEnumerator.Current.Value;
            entry.DrawViewport = null;
            foreach (var operation in entry.CachedOperations)
            {
                QueueFreeAllLines(operation);
            }
            entry.CachedOperations.Clear();

            foreach (var operation in entry.CachedUndoneOperations)
            {
                QueueFreeAllLines(operation);
            }
            entry.CachedUndoneOperations.Clear();
        }
        NetDrawHistories.Clear();
        netEnumerator.Dispose();
        
    }
    
    // To be called only within BeginLine postfix patch.
    public void PatchNotifyBeginLine(ulong drawingStatePlayerId, SubViewport drawingStateDrawViewport, Line2D line)
    {
        if (drawingStatePlayerId != Util.GetLocalPlayerId())
        {
            CheckUpdateNetHistories(drawingStatePlayerId, drawingStateDrawViewport);
            NetPlayerOperationDrewOrErased(drawingStatePlayerId, line);
            return;
        }
        CheckUpdateLocalViewport(drawingStatePlayerId, drawingStateDrawViewport);
        LocalPlayerOperationDrewOrErased(line);
    }
    
    // To be called only within ClearAllLinesForPlayer prefix patch.
    public void PatchNotifyClearAllLinesForPlayer(ulong drawingStatePlayerId, SubViewport drawingStateDrawViewport, List<Line2D> linesToCache)
    {
        if (drawingStatePlayerId != Util.GetLocalPlayerId())
        {
            CheckUpdateNetHistories(drawingStatePlayerId, drawingStateDrawViewport);
            NetPlayerOperationCleared(drawingStatePlayerId, linesToCache);
        }
        CheckUpdateLocalViewport(drawingStatePlayerId, drawingStateDrawViewport);
        LocalPlayerOperationCleared(linesToCache);
    }
    
    private void LocalPlayerOperationDrewOrErased(Line2D line)
    {
        _localDrawData.CachedUndoneOperations.Clear();
        _localDrawData.CachedOperations.Push(new DrawHistoryData.CachedDrawingOperation(false, line));
    }
    
    private void NetPlayerOperationDrewOrErased(ulong playerId, Line2D line)
    {
        NetDrawHistories.TryGetValue(playerId, out var history);
        history?.CachedUndoneOperations.Clear();
        history?.CachedOperations.Push(new DrawHistoryData.CachedDrawingOperation(false, line));
    }
    
    // Reusable, with caution; consider parameter calledFromRedo 
    private void LocalPlayerOperationCleared(List<Line2D> linesToCache, bool calledFromRedo = false)
    {
        if (linesToCache.Count == 0) return; // ; ignore this operation
        if (!calledFromRedo)
        {
            _localDrawData.CachedUndoneOperations.Clear();
        }
        
        // We recover/rebuild cleared (i.e., deleted by ClearAllLinesForPlayer) draw history through the saved data in the cached clear operations
        var cachedOperationsList = new List<DrawHistoryData.CachedDrawingOperation>(_localDrawData.CachedOperations.Count);
        while (_localDrawData.CachedOperations.Count > 0)
        {
            cachedOperationsList.Insert(0, _localDrawData.CachedOperations.Pop());
        }
        for (var i = 0; i < cachedOperationsList.Count; i++)
        {
            if (cachedOperationsList[i].IsClearOperation)
            {
                _localDrawData.CachedOperations.Push(cachedOperationsList[i]);
            }
        }
        
        if (linesToCache.Count == 1)
        {
            // for sake of memory (micro)management
            var line = linesToCache[0];
            var operation = new DrawHistoryData.CachedDrawingOperation(true, line);
            _localDrawData.CachedOperations.Push(operation);
        }
        else
        {
            var operation = new DrawHistoryData.CachedDrawingOperation(true, null, linesToCache);
            _localDrawData.CachedOperations.Push(operation);
        }
    }
    
    // Reusable, with caution; consider parameter calledFromRedo 
    private void NetPlayerOperationCleared(ulong playerId, List<Line2D> linesToCache, bool calledFromRedo = false)
    {
        NetDrawHistories.TryGetValue(playerId, out var history);
        if (history == null) return;
        
        if (linesToCache.Count == 0) return; // ; ignore this operation
        if (!calledFromRedo)
        {
            history.CachedUndoneOperations.Clear();
        }
        
        // We recover/rebuild cleared (i.e., deleted by ClearAllLinesForPlayer) draw history through the saved data in the cached clear operations
        var cachedOperationsList = new List<DrawHistoryData.CachedDrawingOperation>(history.CachedOperations.Count);
        while (history.CachedOperations.Count > 0)
        {
            cachedOperationsList.Insert(0, history.CachedOperations.Pop());
        }
        for (var i = 0; i < cachedOperationsList.Count; i++)
        {
            if (cachedOperationsList[i].IsClearOperation)
            {
                history.CachedOperations.Push(cachedOperationsList[i]);
            }
        }
        
        if (linesToCache.Count == 1)
        {
            // for sake of memory (micro)management
            var line = linesToCache[0];
            var operation = new DrawHistoryData.CachedDrawingOperation(true, line);
            history.CachedOperations.Push(operation);
        }
        else
        {
            var operation = new DrawHistoryData.CachedDrawingOperation(true, null, linesToCache);
            history.CachedOperations.Push(operation);
        }
    }
    
    private void CheckUpdateLocalViewport(ulong playerId, SubViewport svp)
    {
        if (playerId != Util.GetLocalPlayerId()) return;
        
        /* SubViewport svp belongs to local player */
        if (_localDrawData.DrawViewport != null) return; // Note: ResetRunState() sets _localDrawData.LocalDrawViewport to null
        _localDrawData.DrawViewport = svp;
    }

    private void CheckUpdateNetHistories(ulong playerId, SubViewport svp)
    {
        if (!NetDrawHistories.TryGetValue(playerId, out var history))
        {
            NetDrawHistories.Add(playerId, new DrawHistoryData(svp));
        } else
        {
            history.DrawViewport = svp;
        }
    }
    
    private void LocalViewportAddLine(List<Line2D> toAdd)
    {
        foreach (var line in toAdd)
        {
            LocalViewportAddLine(line);
        }
    }
    
    private void LocalViewportAddLine(Line2D toAdd)
    {
        _localDrawData.DrawViewport?.AddChildSafely((Node) toAdd);
        
        // TODO
        // send message to perform same operation to other players

        return;
    }
    
    private void NetViewportAddLine(ulong playerId, List<Line2D> toAdd)
    {
        foreach (var line in toAdd)
        {
            NetViewportAddLine(playerId, line);
        }
    }

    private void NetViewportAddLine(ulong playerId, Line2D toAdd)
    {
        NetDrawHistories.TryGetValue(playerId, out var history);
        if (history == null) return;

        history.DrawViewport?.AddChildSafely((Node)toAdd);
    }
    
    private void LocalViewportRemoveLine(List<Line2D> toRemove)
    {
        foreach (var line in toRemove)
        {
            LocalViewportRemoveLine(line);
        }
    }
    
    private void LocalViewportRemoveLine(Line2D toRemove)
    {
        _localDrawData.DrawViewport?.RemoveChildSafely((Node) toRemove);
        
        // TODO
        // send message to perform same operation to other players

        return;
    }

    private void NetViewportRemoveLine(ulong playerId, List<Line2D> toRemove)
    {
        foreach (var line in toRemove)
        {
            NetViewportRemoveLine(playerId, line);
        }
    }

    private void NetViewportRemoveLine(ulong playerId, Line2D toRemove)
    {
        NetDrawHistories.TryGetValue(playerId, out var history);
        if (history == null) return;
        
        history.DrawViewport?.RemoveChildSafely((Node) toRemove);
    }

    // Do not enter parameter for local
    public void Undo(ulong? playerId = null)
    {
        DrawHistoryData? history = _localDrawData;
        if (playerId != null)
        {
            var id = (ulong)playerId;
            if (!NetDrawHistories.TryGetValue(id, out history)) return;
        }
        
        if (history.UndoLocked()) return;
        var operation = history.CachedOperations.Pop();
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
                LocalViewportAddLine(operation.Line);
                var redrawOperation = new DrawHistoryData.CachedDrawingOperation(false, operation.Line);
                history.CachedOperations.Push(redrawOperation);
            } else if (operation.LineSet != null) 
            {
                LocalViewportAddLine(operation.LineSet);
                for (var i = 0; i < operation.LineSet.Count; i++)
                {
                    var redrawOperation = new DrawHistoryData.CachedDrawingOperation(false, operation.LineSet[i]);
                    history.CachedOperations.Push(redrawOperation);
                }
            }
            
            history.CachedUndoneOperations.Push(operation);
        }
        else
        {
            if (operation.Line != null)
            {
                LocalViewportRemoveLine(operation.Line);
                history.CachedUndoneOperations.Push(operation);
            }
        }
    }

    // Do not enter parameter for local
    public void Redo(ulong? playerId = null)
    {
        DrawHistoryData? history = _localDrawData;
        if (playerId != null)
        {
            var id = (ulong)playerId;
            if (!NetDrawHistories.TryGetValue(id, out history)) return;
        }
        
        if (history.RedoLocked()) return;
        var operation = history.CachedUndoneOperations.Pop();
        if (!operation.IsValid) return;
        
        if (operation.IsClearOperation)
        {
            // this operation contains a 'cleared set,' in which the clear operation has been undone
            var lineSet = new List<Line2D>();
            if (operation.Line != null)
            {
                LocalViewportRemoveLine(operation.Line);
                lineSet.Add(operation.Line);
            }
            else if (operation.LineSet != null)
            {
                LocalViewportRemoveLine(operation.LineSet);
                lineSet = operation.LineSet;
            }

            LocalPlayerOperationCleared(lineSet, true);
        }
        else
        {
            if (operation.Line == null) return;
            LocalViewportAddLine(operation.Line);
            history.CachedOperations.Push(operation);
        }
    }

    
    // helper
    private void QueueFreeAllLines(DrawHistoryData.CachedDrawingOperation operation)
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