using BaseLib;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.MapDrawing;

// using MapArtist.MapArtistCode.DrawingHistory.MapArtistDrawHistoryUndoMessage;

namespace MapArtist.MapArtistCode.DrawingHistory;

// A linear history for a player's map drawings; performing an operation (draw, erase, or clear) overwrites (erases) all redo history
public sealed class MapArtistLocalDrawingHistory
{
    static MapArtistLocalDrawingHistory() {}
    private MapArtistLocalDrawingHistory() {}
    public static MapArtistLocalDrawingHistory Instance { get; } = new MapArtistLocalDrawingHistory();

    public NMapDrawings? MapDrawings;
    
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
    
    private static readonly Dictionary<ulong, DrawHistoryData> NetDrawHistories = new();

    private readonly bool _multiplayerFunctionalityDisabled = false;
    

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
            if (_multiplayerFunctionalityDisabled) return;
            
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
            if (_multiplayerFunctionalityDisabled) return;
            CheckUpdateNetHistories(drawingStatePlayerId, drawingStateDrawViewport);
            // NetPlayerOperationCleared(drawingStatePlayerId, linesToCache);
            //
            // return;
        }
        CheckUpdateLocalViewport(drawingStatePlayerId, drawingStateDrawViewport);
        // LocalPlayerOperationCleared(linesToCache);

        PlayerOperationCleared(drawingStatePlayerId, linesToCache);
    }
    
    private void LocalPlayerOperationDrewOrErased(Line2D line)
    {
        _localDrawData.CachedUndoneOperations.Clear();
        _localDrawData.CachedOperations.Push(new DrawHistoryData.CachedDrawingOperation(false, line));
    }
    
    private void NetPlayerOperationDrewOrErased(ulong playerId, Line2D line)
    {
        if (_multiplayerFunctionalityDisabled) return;
        NetDrawHistories.TryGetValue(playerId, out var history);
        history?.CachedUndoneOperations.Clear();
        history?.CachedOperations.Push(new DrawHistoryData.CachedDrawingOperation(false, line));
    }
    
    // Reusable, with caution; consider parameter calledFromRedo 
    private void PlayerOperationCleared(ulong playerId, List<Line2D> linesToCache, bool calledFromRedo = false)
    {
        DrawHistoryData? history;
        if (playerId == Util.GetLocalPlayerId())
        {
            history = _localDrawData;
        }
        else
        {
            if (_multiplayerFunctionalityDisabled) return;
            NetDrawHistories.TryGetValue(playerId, out history);
            if (history == null) return;
        }
        
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
    
    // // Reusable, with caution; consider parameter calledFromRedo 
    // private void NetPlayerOperationCleared(ulong playerId, List<Line2D> linesToCache, bool calledFromRedo = false)
    // {
    //     NetDrawHistories.TryGetValue(playerId, out var history);
    //     if (history == null) return;
    //     
    //     if (linesToCache.Count == 0) return; // ; ignore this operation
    //     if (!calledFromRedo)
    //     {
    //         history.CachedUndoneOperations.Clear();
    //     }
    //     
    //     // We recover/rebuild cleared (i.e., deleted by ClearAllLinesForPlayer) draw history through the saved data in the cached clear operations
    //     var cachedOperationsList = new List<DrawHistoryData.CachedDrawingOperation>(history.CachedOperations.Count);
    //     while (history.CachedOperations.Count > 0)
    //     {
    //         cachedOperationsList.Insert(0, history.CachedOperations.Pop());
    //     }
    //     for (var i = 0; i < cachedOperationsList.Count; i++)
    //     {
    //         if (cachedOperationsList[i].IsClearOperation)
    //         {
    //             history.CachedOperations.Push(cachedOperationsList[i]);
    //         }
    //     }
    //     
    //     if (linesToCache.Count == 1)
    //     {
    //         // for sake of memory (micro)management
    //         var line = linesToCache[0];
    //         var operation = new DrawHistoryData.CachedDrawingOperation(true, line);
    //         history.CachedOperations.Push(operation);
    //     }
    //     else
    //     {
    //         var operation = new DrawHistoryData.CachedDrawingOperation(true, null, linesToCache);
    //         history.CachedOperations.Push(operation);
    //     }
    // }
    
    private void CheckUpdateLocalViewport(ulong playerId, SubViewport svp)
    {
        if (playerId != Util.GetLocalPlayerId()) return;
        
        /* SubViewport svp belongs to local player */
        if (_localDrawData.DrawViewport != null) return; // Note: ResetRunState() sets _localDrawData.LocalDrawViewport to null
        _localDrawData.DrawViewport = svp;
    }

    private void CheckUpdateNetHistories(ulong playerId, SubViewport svp)
    {
        if (_multiplayerFunctionalityDisabled) return;
        if (!NetDrawHistories.TryGetValue(playerId, out var history))
        {
            NetDrawHistories.Add(playerId, new DrawHistoryData(svp));
        } else
        {
            history.DrawViewport = svp;
        }
    }
    
    // private void LocalViewportAddLine(List<Line2D> toAdd)
    // {
    //     foreach (var line in toAdd)
    //     {
    //         ViewportAddLine(line);
    //     }
    // }
    
    // private void LocalViewportAddLine(Line2D toAdd)
    // {
    //     _localDrawData.DrawViewport?.AddChildSafely((Node) toAdd);
    //     
    //     // TODO
    //     // send message to perform same operation to other players
    //     
    //
    //     return;
    // }
    
    private void ViewportAddLine(List<Line2D> toAdd, SubViewport? subViewport = null)
    {
        // var drawViewport = subViewport ?? _localDrawData.DrawViewport;
        // if (drawViewport == null) return;
        
        foreach (var line in toAdd)
        {
            ViewportAddLine(line, subViewport);
        }
    }
    
    private void ViewportAddLine(Line2D toAdd, SubViewport? subViewport = null)
    {
        var drawViewport = subViewport ?? _localDrawData.DrawViewport;
        drawViewport?.AddChildSafely((Node) toAdd);
    }
    
    // private void NetViewportAddLine(ulong playerId, List<Line2D> toAdd)
    // {
    //     foreach (var line in toAdd)
    //     {
    //         NetViewportAddLine(playerId, line);
    //     }
    // }
    //
    // private void NetViewportAddLine(ulong playerId, Line2D toAdd)
    // {
    //     NetDrawHistories.TryGetValue(playerId, out var history);
    //     if (history == null) return;
    //
    //     history.DrawViewport?.AddChildSafely((Node)toAdd);
    // }
    
    private void ViewportRemoveLine(List<Line2D> toRemove, SubViewport? subViewport = null)
    {
        foreach (var line in toRemove)
        {
            ViewportRemoveLine(line, subViewport);
        }
    }
    
    private void ViewportRemoveLine(Line2D toRemove, SubViewport? subViewport = null)
    {
        var drawViewport = subViewport ?? _localDrawData.DrawViewport;
        drawViewport?.RemoveChildSafely((Node) toRemove);
    }

    // private void NetViewportRemoveLine(ulong playerId, List<Line2D> toRemove)
    // {
    //     foreach (var line in toRemove)
    //     {
    //         NetViewportRemoveLine(playerId, line);
    //     }
    // }
    //
    // private void NetViewportRemoveLine(ulong playerId, Line2D toRemove)
    // {
    //     NetDrawHistories.TryGetValue(playerId, out var history);
    //     if (history == null) return;
    //     
    //     history.DrawViewport?.RemoveChildSafely((Node) toRemove);
    // }

    public void RemoveRemoteLine(ulong playerId, SerializableMapDrawingLine drawingLine)
    {
        NetDrawHistories.TryGetValue(playerId, out var data);
        if (data == null) return;
        var drawingViewport = data.DrawViewport;

        // var serializedPoints = drawingLine.mapPoints;
        var serializedPoints = new List<Vector2>();
        foreach (var line in drawingLine.mapPoints)
        {
            serializedPoints.Add(FromNetPosition(line));
        }
        
        var viewportLines = drawingViewport.GetChildren().OfType<Line2D>();

        foreach (var line in viewportLines)
        {
            var pointsList = line.Points.ToList();

            if (pointsList == serializedPoints)
            {
                ViewportRemoveLine(line, drawingViewport);
            }
        }
    }

    // Do not enter parameter for local
    public void Undo()
    {
        if (_localDrawData.UndoLocked()) return;
        var operation = _localDrawData.CachedOperations.Pop();
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
                ViewportAddLine(operation.Line, _localDrawData.DrawViewport);
                var redrawOperation = new DrawHistoryData.CachedDrawingOperation(false, operation.Line);
                _localDrawData.CachedOperations.Push(redrawOperation);
            } else if (operation.LineSet != null) 
            {
                ViewportAddLine(operation.LineSet, _localDrawData.DrawViewport);
                for (var i = 0; i < operation.LineSet.Count; i++)
                {
                    var redrawOperation = new DrawHistoryData.CachedDrawingOperation(false, operation.LineSet[i]);
                    _localDrawData.CachedOperations.Push(redrawOperation);
                }
            }
            
            _localDrawData.CachedUndoneOperations.Push(operation);
        }
        else
        {
            if (operation.Line != null)
            {
                var serializedLine = new SerializableMapDrawingLine
                {
                    // mapPoints = operation.Line.Points.ToList()
                    mapPoints = new List<Vector2>()
                };
                serializedLine.isEraser = false;
                foreach (Vector2 point in operation.Line.Points)
                    serializedLine.mapPoints.Add(this.ToNetPosition(point));
                CustomMessageWrapper.Send(new MapArtistDrawHistoryRemoveLineMessage(serializedLine));

                ViewportRemoveLine(operation.Line, _localDrawData.DrawViewport);
                _localDrawData.CachedUndoneOperations.Push(operation);
            }
        }
    }
    
    private Vector2 ToNetPosition(Vector2 pos)
    {
        if (MapDrawings == null) return pos;
        pos.X -= MapDrawings.Size.X * 0.5f;
        pos /= new Vector2(960f, MapDrawings.Size.Y);
        return pos;
    }
    
    private Vector2 FromNetPosition(Vector2 pos)
    {
        if (MapDrawings == null) return pos;
        pos *= new Vector2(960f, MapDrawings.Size.Y);
        pos.X += MapDrawings.Size.X * 0.5f;
        return pos;
    }

    // Do not enter parameter for local
    public void Redo(ulong? playerId = null)
    {
        if (_localDrawData.RedoLocked()) return;
        var operation = _localDrawData.CachedUndoneOperations.Pop();
        if (!operation.IsValid) return;
        
        if (operation.IsClearOperation)
        {
            // this operation contains a 'cleared set,' in which the clear operation has been undone
            var lineSet = new List<Line2D>();
            if (operation.Line != null)
            {
                ViewportRemoveLine(operation.Line, _localDrawData.DrawViewport);
                lineSet.Add(operation.Line);
            }
            else if (operation.LineSet != null)
            {
                ViewportRemoveLine(operation.LineSet, _localDrawData.DrawViewport);
                lineSet = operation.LineSet;
            }

            var id = playerId ?? Util.GetLocalPlayerId();
            PlayerOperationCleared(id, lineSet, true);
        }
        else
        {
            if (operation.Line == null) return;
            ViewportAddLine(operation.Line, _localDrawData.DrawViewport);
            _localDrawData.CachedOperations.Push(operation);
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