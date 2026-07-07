using Godot;

namespace MapArtist.MapArtistCode.DrawingHistory;

public class DrawHistoryData(SubViewport? svp)
{
    public readonly struct CachedDrawingOperation(bool isClearOperation, Line2D? line = null, List<Line2D>? set = null)
    {
        // proper usage: between Line and LineSet, one and only one should be null
        public Line2D? Line => IsValid ? line : null;
        public List<Line2D>? LineSet => IsValid ? set : null;
        public bool IsClearOperation => isClearOperation;
        public bool IsValid => !((line == null && (set == null || set.Count == 0)) || (line != null && set != null) || (!isClearOperation && (line == null || set != null)));
    }   
    
    public SubViewport? DrawViewport = svp;
    public readonly Stack<CachedDrawingOperation> CachedOperations = new Stack<CachedDrawingOperation>();
    public readonly Stack<CachedDrawingOperation> CachedUndoneOperations = new Stack<CachedDrawingOperation>();
    
    public bool UndoLocked()
    {
        return (DrawViewport == null || CachedOperations.Count == 0);
    }
    
    public bool RedoLocked()
    {
        return (DrawViewport == null || CachedUndoneOperations.Count == 0);
    }
}