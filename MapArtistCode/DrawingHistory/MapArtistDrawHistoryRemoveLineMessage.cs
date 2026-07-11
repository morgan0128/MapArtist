using BaseLib;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;
using MegaCrit.Sts2.Core.Saves.MapDrawing;

namespace MapArtist.MapArtistCode.DrawingHistory;

public sealed class MapArtistDrawHistoryRemoveLineMessage : ICustomMessage
{
    public bool ShouldBroadcast => true;

    // public SubViewport? DrawViewport;

    // public ulong PlayerId { get; private set; }
    
    private static readonly QuantizeParams _quantizeParamsX = new QuantizeParams(-3f, 3f, 13);
    private static readonly QuantizeParams _quantizeParamsY = new QuantizeParams(-2f, 2f, 16 /*0x10*/);

    public SerializableMapDrawingLine DrawingLine { get; private set; }
    
    // public MapArtistDrawHistoryUndoMessage(SubViewport drawViewport, Line2D line)
    // {
    //     DrawViewport = drawViewport;
    //     Line = line;
    // }
    
    public MapArtistDrawHistoryRemoveLineMessage()
    {
    }
    
    // public MapArtistDrawHistoryRemoveLineMessage(ulong playerId, SerializableMapDrawingLine drawingLine)
    // {
    //     PlayerId = playerId;
    //     DrawingLine = drawingLine;
    // }

    public MapArtistDrawHistoryRemoveLineMessage(SerializableMapDrawingLine drawingLine)
    {
        DrawingLine = drawingLine;
    }

    public NetTransferMode Mode => NetTransferMode.Reliable;

    public LogLevel LogLevel => LogLevel.VeryDebug;

    public bool ShouldBuffer => true;

    public void Serialize(PacketWriter writer)
    {
        // writer.WriteULong(PlayerId);
        DrawingLine.Serialize(writer);
    }

    public void Deserialize(PacketReader reader)
    {
        // reader.ReadULong();
        DrawingLine = new SerializableMapDrawingLine();
        DrawingLine.isEraser = reader.ReadBool();
        int num = reader.ReadInt(16 /*0x10*/);
        for (int index = 0; index < num; ++index)
            DrawingLine.mapPoints.Add(reader.ReadVector2(new QuantizeParams?(_quantizeParamsX), new QuantizeParams?(_quantizeParamsY)));

        
        // DrawingLine.Deserialize(reader);
    }

    public void HandleMessage(ulong senderId)
    {
        BaseLibMain.Logger.Info("Received message.\n");
        // BaseLibMain.Logger.Info("Message has null DrawViewport? " + (DrawViewport == null) + "\n");
        // DrawViewport?.RemoveChildSafely(Line);

        MapArtistLocalDrawingHistory.Instance.RemoveRemoteLine(senderId, DrawingLine);

    }

}