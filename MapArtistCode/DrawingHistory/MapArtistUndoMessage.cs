// using Godot;
//
// namespace MapArtist.MapArtistCode.Multiplayer.DrawingHistory;
//
// public class MapArtistUndoMessage
// {
//     public bool ShouldBroadcast => true;
//
//     public SubViewport DrawViewport;
//
//     public MapArtistUndoMessage()
//     {
//     }
//
//     public MapArtistUndoMessage(SubViewport drawViewport)
//     {
//         
//     }
//
//     public static MapArtistUndoMessage Reset()
//     {
//         return new MapArtistUndoMessage
//         {
//             IsReset = true
//         };
//     }
//
//     public void HandleMessage(ulong senderId)
//     {
//         // if (IsReset)
//         // {
//         //     MapArtistDictionaries.ClearAll(senderId);
//         //     return;
//         // }
//         //
//         // MapArtistDictionaries.SetColor(senderId, Color);
//         // MapArtistDictionaries.SetPenWidth(senderId, Width);
//     }
//
//     public void Serialize(PacketWriter writer)
//     {
//         writer.WriteBool(IsReset);
//         writer.WriteFloat(Color.R);
//         writer.WriteFloat(Color.G);
//         writer.WriteFloat(Color.B);
//         writer.WriteFloat(Color.A);
//         writer.WriteFloat(Width);
//     }
//
//     public void Deserialize(PacketReader reader)
//     {
//         IsReset = reader.ReadBool();
//         Color = new Color(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
//         Width = reader.ReadFloat();
//     }
// }