using System;
using System.Collections.Generic;

namespace TNLPacketAnalyzer.CLI;

public class GhostConnection : EventConnection
{
    public bool Ghosting { get; set; }
    public bool DoesGhostTo { get; set; }
    public bool DoesGhostFrom { get; set; }
    public Dictionary<long, GhostObject> Objects { get; } = new();

    public override void ReadPacket(OutputBuilder builder, Reader reader, bool isClient)
    {
        base.ReadPacket(builder, reader, isClient);

        builder.AppendLine().AppendLine("GhostConnection::ReadPacket").AppendLine("[").IncreaseDepth();

        if (DebugObjectSizes)
            builder.AppendLine($"DebugCheckSum: {reader.ReadInt(32)}");

        if (!DoesGhostFrom)
            return;

        var ghosting = reader.ReadBit();
        builder.AppendLine($"Ghosting: {ghosting}");

        if (ghosting)
        {
            var idSize = reader.ReadInt(3) + 3;

            builder.AppendLine($"IdSize: {idSize}");

            while (reader.ReadBit())
            {
                builder.AppendLine("[").IncreaseDepth();

                var index = reader.ReadInt(idSize);

                builder.AppendLine($"Index: {index}");

                if (reader.ReadBit())
                {
                    builder.AppendLine("Removing ghost");
                }
                else
                {
                    if (DebugObjectSizes)
                        builder.AppendLine($"BitStreamEndPos: {reader.ReadInt(16)}");

                    if (Objects.ContainsKey(index))
                    {
                        builder.AppendLine("Update").AppendLine("[").IncreaseDepth();

                        Objects[index].UnpackUpdate(builder, reader, false);

                        builder.DecreaseDepth().AppendLine("]");
                    }
                    else
                    {
                        builder.AppendLine("Initial update").AppendLine("[").IncreaseDepth();

                        var classId = reader.ReadInt(3); // TODO: based on class counts

                        builder.AppendLine($"ClassId: {classId} ({GetClassName(classId)})");

                        var obj = Create(classId);

                        Objects.Add(index, obj);

                        obj.UnpackUpdate(builder, reader, true);

                        builder.DecreaseDepth().AppendLine("]");
                    }
                }

                builder.DecreaseDepth().AppendLine("]");
            }
        }

        builder.DecreaseDepth().AppendLine("]");
    }

    public override void ReadConnectAccept(OutputBuilder builder, Reader reader, bool isClient)
    {
        base.ReadConnectAccept(builder, reader, isClient);

        DoesGhostFrom = true;
    }

    public override void ReadConnectRequest(OutputBuilder builder, Reader reader, bool isClient)
    {
        base.ReadConnectRequest(builder, reader, isClient);

        DoesGhostTo = true;
    }

    public override void OnEvent(long classId, OutputBuilder builder, Reader reader, bool isClient)
    {
        switch (classId)
        {
            case 0:
                //AnalyzeRPCMsgEndGhosting(builder, reader);
                if (!isClient)
                    Ghosting = false;
                break;

            case 1:
                AnalyzeRPCMsgReadyForNormalGhosts(builder, reader);

                if (isClient)
                    Ghosting = true;
                break;

            case 2:
                AnalyzeRPCMsgStartGhosting(builder, reader);

                if (!isClient)
                    Ghosting = true;
                break;

            default:
                base.OnEvent(classId, builder, reader, isClient);
                break;
        }
    }

    private void AnalyzeRPCMsgReadyForNormalGhosts(OutputBuilder builder, Reader reader)
    {
        builder.AppendLine($"Sequence: {reader.ReadInt(32)}");
    }

    private void AnalyzeRPCMsgStartGhosting(OutputBuilder builder, Reader reader)
    {
        builder.AppendLine($"Sequence: {reader.ReadInt(32)}");
    }

    private static GhostObject Create(long classId)
    {
        return classId switch
        {
            0 => new GhostCharacter(),
            1 => new GhostCreature(),
            2 => new GhostObject(),
            3 => new GhostVehicle(),
            _ => throw new NotSupportedException()
        };
    }

    private static string GetClassName(long classId)
    {
        return classId switch
        {
            0 => "GhostCharacter",
            1 => "GhostCreature",
            2 => "GhostObject",
            3 => "GhostVehicle",
            _ => throw new NotSupportedException()
        };
    }
}
