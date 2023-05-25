namespace TNLPacketAnalyzer.CLI;

public class GhostCharacter : GhostObject
{
    public override void UnpackUpdate(OutputBuilder builder, Reader reader, bool initial)
    {
        if (initial)
            UnpackCommon(builder, reader);

        builder.AppendLine().AppendLine("GhostCharacter::UnpackUpdate").AppendLine("[").IncreaseDepth();

        if (initial)
        {
            builder.AppendLine($"Name: {reader.ReadString()}");
            builder.AppendLine($"ClanName: {reader.ReadString()}");
            builder.AppendLine($"Level: {reader.ReadInt(8)}");
            builder.AppendLine($"CurrentVehicle: {reader.ReadInt(64)}");
            builder.AppendLine($"HeadId: {reader.ReadInt(16)}");
            builder.AppendLine($"BodyId: {reader.ReadInt(16)}");
            builder.AppendLine($"HeadDetail1: {reader.ReadInt(16)}");
            builder.AppendLine($"HeadDetail2: {reader.ReadInt(16)}");
            builder.AppendLine($"MouthId: {reader.ReadInt(16)}");
            builder.AppendLine($"EyesId: {reader.ReadInt(16)}");
            builder.AppendLine($"HelmetId: {reader.ReadInt(16)}");
            builder.AppendLine($"HairId: {reader.ReadInt(16)}");
            builder.AppendLine($"ScaleOffset: {reader.ReadSingle(32)}");
            builder.AppendLine($"PrimaryColor: {reader.ReadInt(24)}");
            builder.AppendLine($"SecondaryColor: {reader.ReadInt(24)}");
            builder.AppendLine($"SkinColor: {reader.ReadInt(24)}");
            builder.AppendLine($"HairColor: {reader.ReadInt(3)}");
            builder.AppendLine();

            UnpackSkills(builder, reader);

            builder.AppendLine();
        }

        if (reader.ReadBit())
            builder.AppendLine($"GMLevel: {reader.ReadInt(4)}");

        if (reader.ReadBit())
        {
            builder.AppendLine().AppendLine("Clan").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"Id: {reader.ReadInt(32)}");
            builder.AppendLine($"Rank: {reader.ReadInt(32)}");
            builder.AppendLine($"Name: {reader.ReadString()}");
            builder.DecreaseDepth().AppendLine("]");
        }

        if (reader.ReadBit())
            builder.AppendLine($"PetCBID: {reader.ReadInt(16)}");

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

        if (reader.ReadBit())
            builder.AppendLine($"GivesToken: {reader.ReadBit()}");

        builder.DecreaseDepth().AppendLine("]");
    }
}
