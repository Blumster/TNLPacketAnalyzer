namespace TNLPacketAnalyzer.CLI;

public class GhostCreature : GhostObject
{
    public override void UnpackUpdate(OutputBuilder builder, Reader reader, bool initial)
    {
        if (initial)
            UnpackCommon(builder, reader);

        builder.AppendLine().AppendLine("GhostCreature::UnpackUpdate").AppendLine("[").IncreaseDepth();

        if (initial)
        {
            if (reader.ReadBit())
                builder.AppendLine($"EnhancementID: {reader.ReadInt(20)}");

            if (reader.ReadBit())
                builder.AppendLine($"CoidOnUseTrigger: {reader.ReadInt(20)}");

            if (reader.ReadBit())
                builder.AppendLine($"CoidOnUseReaction: {reader.ReadInt(20)}");

            if (reader.ReadBit())
            {
                builder.AppendLine($"SummonerCoid: {reader.ReadInt(64)}");
                builder.AppendLine($"SummonerGlobal: {reader.ReadBit()}");
            }

            if (reader.ReadBit())
                builder.AppendLine($"SpawnOwnerCoid: {reader.ReadInt(64)}");

            builder.AppendLine($"DoesntCountAsSummon: {reader.ReadBit()}");
            builder.AppendLine($"Level: {reader.ReadInt(8)}");
            builder.AppendLine($"IsElite: {reader.ReadBit()}");

            UnpackSkills(builder, reader);
        }

        if (reader.ReadBit())
            builder.AppendLine($"Murderer: {reader.ReadInt(64)}");

        if (reader.ReadBit())
        {
            builder.AppendLine($"HP: {reader.ReadInt(18)}");
            builder.AppendLine($"IsCorpse: {reader.ReadBit()}");
        }

        if (reader.ReadBit())
            builder.AppendLine($"MaxHP: {reader.ReadInt(18)}");

        if (reader.ReadBit())
            builder.AppendLine($"AIState: {reader.ReadInt(8)}");

        if (reader.ReadBit())
        {
            builder.AppendLine().AppendLine("Movement").AppendLine("[").IncreaseDepth();

            builder.AppendLine("Position").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"X: {reader.ReadSingle(32)}");
            builder.AppendLine($"Y: {reader.ReadSingle(32)}");
            builder.AppendLine($"Z: {reader.ReadSingle(32)}");
            builder.DecreaseDepth().AppendLine("]");

            builder.AppendLine().AppendLine("Rotation").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"X: {reader.ReadSingle(32)}");
            builder.AppendLine($"Y: {reader.ReadSingle(32)}");
            builder.AppendLine($"Z: {reader.ReadSingle(32)}");
            builder.AppendLine($"W: {reader.ReadSingle(32)}");
            builder.DecreaseDepth().AppendLine("]");

            builder.AppendLine().AppendLine("Linear velocity").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"X: {reader.ReadSingle(32)}");
            builder.AppendLine($"Y: {reader.ReadSingle(32)}");
            builder.AppendLine($"Z: {reader.ReadSingle(32)}");
            builder.DecreaseDepth().AppendLine("]");

            builder.AppendLine().AppendLine("Angular velocity").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"X: {reader.ReadSingle(32)}");
            builder.AppendLine($"Y: {reader.ReadSingle(32)}");
            builder.AppendLine($"Z: {reader.ReadSingle(32)}");
            builder.DecreaseDepth().AppendLine("]");

            builder.DecreaseDepth().AppendLine("]").AppendLine();
        }

        if (reader.ReadBit())
        {
            builder.AppendLine("Target").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"TargetCoid: {reader.ReadInt(64)}");
            builder.AppendLine($"TargetGlobal: {reader.ReadBit()}");
            builder.DecreaseDepth().AppendLine("]");
        }

        builder.DecreaseDepth().AppendLine("]");
    }
}
