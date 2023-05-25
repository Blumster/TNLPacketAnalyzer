using System;

namespace TNLPacketAnalyzer.CLI;

public class EventConnection : NetConnection
{
    public int EventClassBitSize { get; set; }

    public override void ReadPacket(OutputBuilder builder, Reader reader, bool isClient)
    {
        builder.AppendLine().AppendLine("EventConnection::ReadPacket").AppendLine("[").IncreaseDepth();

        if (DebugObjectSizes)
            builder.AppendLine($"DebugCheckSum: {reader.ReadInt(32)}");

        var prevSeq = -2L;
        var ungaranteedPhase = true;
        while (true)
        {
            var bit = reader.ReadBit();
            if (ungaranteedPhase && !bit)
            {
                ungaranteedPhase = false;
                bit = reader.ReadBit();
            }

            if (!ungaranteedPhase && !bit)
                break;

            builder.AppendLine("Event Block").AppendLine("[").IncreaseDepth();

            if (!ungaranteedPhase)
            {
                long seq;
                if (reader.ReadBit())
                    seq = (prevSeq + 1) & 0x7F;
                else
                    seq = reader.ReadInt(7);

                builder.AppendLine($"Sequence: {seq}");
                prevSeq = seq;
            }

            if (DebugObjectSizes)
                builder.AppendLine($"DebugCheckSum: {reader.ReadInt(16)}");

            var classId = reader.ReadInt(EventClassBitSize);
            builder.AppendLine($"Class Id: {classId} ({Const.ClassNames[Math.Min(classId, 8)]})");

            builder.AppendLine("Data Block").AppendLine("[").IncreaseDepth();

            OnEvent(classId, builder, reader, isClient);

            builder.DecreaseDepth().AppendLine("]");
            builder.DecreaseDepth().AppendLine("]");
        }

        builder.DecreaseDepth().AppendLine("]");
    }

    public virtual void OnEvent(long classId, OutputBuilder builder, Reader reader, bool isClient)
    {
    }

    public override void ReadConnectRequest(OutputBuilder builder, Reader reader, bool isClient)
    {
        base.ReadConnectRequest(builder, reader, isClient);

        var eventClassCount = reader.ReadInt(32);

        builder.AppendLine().AppendLine("EventConnection::ReadConnectRequest").AppendLine("[").IncreaseDepth();
        builder.AppendLine($"Class Count: {eventClassCount}");
        builder.DecreaseDepth().AppendLine("]");

        EventClassBitSize = Utils.GetNextBinLog2((int)eventClassCount);
    }

    public override void ReadConnectAccept(OutputBuilder builder, Reader reader, bool isClient)
    {
        base.ReadConnectAccept(builder, reader, isClient);

        var eventClassCount = reader.ReadInt(32);

        builder.AppendLine().AppendLine("EventConnection::ReadConnectAccept").AppendLine("[").IncreaseDepth();
        builder.AppendLine($"Event Class Count: {eventClassCount}");
        builder.DecreaseDepth().AppendLine("]");

        EventClassBitSize = Utils.GetNextBinLog2((int)eventClassCount);
    }

    public static void ReadByteBuffer(OutputBuilder sb, Reader reader, bool newLine = true)
    {
        ReadByteArray(sb, reader, reader.ReadInt(10), newLine);
    }

    public static void ReadByteArray(OutputBuilder sb, Reader reader, long length, bool newLine = true)
    {
        var arr = new byte[length];

        for (var i = 0; i < arr.Length; ++i)
            arr[i] = (byte)reader.ReadInt(8);

        sb.Append(BitConverter.ToString(arr));

        if (newLine)
            sb.AppendLine();
    }
}
