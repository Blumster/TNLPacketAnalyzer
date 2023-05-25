namespace TNLPacketAnalyzer.CLI;

public class GhostObject
{
    public void UnpackCommon(OutputBuilder builder, Reader reader)
    {
        builder.AppendLine().AppendLine("GhostObject::UnpackCommon").AppendLine("[").IncreaseDepth();
        builder.AppendLine($"Coid: {reader.ReadInt(64)}");
        builder.AppendLine($"Global: {reader.ReadBit()}");
        builder.AppendLine($"CBID: {reader.ReadInt(20)}");
        builder.AppendLine($"MaxHP: {reader.ReadInt(18)}");
        builder.AppendLine($"Faction: {reader.ReadInt(16)}");
        builder.AppendLine($"TeamFaction: {reader.ReadInt(16)}");

        builder.DecreaseDepth().AppendLine("]");
    }

    public void UnpackSkills(OutputBuilder builder, Reader reader)
    {
        builder.AppendLine("GhostObject::UnpackSkills").AppendLine("[").IncreaseDepth();
        builder.AppendLine($"Count: {reader.ReadInt(8)}");
        builder.DecreaseDepth().AppendLine("]");
    }

    public virtual void UnpackUpdate(OutputBuilder builder, Reader reader, bool initial)
    {
        builder.AppendLine("GhostObject::UnpackUpdate").AppendLine("[").IncreaseDepth();

        if (initial)
        {
            var global = reader.ReadBit();
            builder.AppendLine($"Global: {global}");

            if (global)
                builder.AppendLine($"Coid: {reader.ReadInt(64)}");
            else
                builder.AppendLine($"Coid: {reader.ReadInt(20)}");
        }

        if (reader.ReadBit())
        {
            builder.AppendLine($"HP: {reader.ReadInt(18)}");

            var isCorpse = reader.ReadBit();
            builder.AppendLine($"IsCorpse: {isCorpse}");

            if (isCorpse && reader.ReadBit())
            {
                builder.AppendLine("Murderer").AppendLine("[").IncreaseDepth();
                builder.AppendLine($"DeathType: {reader.ReadInt(3)}");
                builder.AppendLine($"Coid: {reader.ReadInt(64)}");
                builder.AppendLine($"Global: {reader.ReadBit()}");
                builder.DecreaseDepth().AppendLine("]");
            }
        }

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

        builder.DecreaseDepth().AppendLine("]");
    }
}
