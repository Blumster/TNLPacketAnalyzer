namespace TNLPacketAnalyzer.CLI;

public class GhostVehicle : GhostObject
{
    public override void UnpackUpdate(OutputBuilder builder, Reader reader, bool initial)
    {
        if (initial)
            UnpackCommon(builder, reader);

        builder.AppendLine().AppendLine("GhostVehicle::UnpackUpdate").AppendLine("[").IncreaseDepth();

        if (initial)
        {
            builder.AppendLine($"PrimaryColor: {reader.ReadInt(32):X8}");
            builder.AppendLine($"SecondaryColor: {reader.ReadInt(32):X8}");
            builder.AppendLine($"IsActive: {reader.ReadBit()}");
            builder.AppendLine($"Trim: {reader.ReadInt(8)}");

            if (reader.ReadBit())
                builder.AppendLine($"SpeedAdd: {reader.ReadSingle(32)}");

            if (reader.ReadBit())
                builder.AppendLine($"BrakesMaxTorqueFrontMultiplier: {reader.ReadSingle(32)}");

            if (reader.ReadBit())
                builder.AppendLine($"BrakesMaxTorqueRearAdjustMultiplier: {reader.ReadSingle(32)}");

            if (reader.ReadBit())
                builder.AppendLine($"SteeringMaxAngleMultiplier: {reader.ReadSingle(32)}");

            if (reader.ReadBit())
                builder.AppendLine($"SteeringFullSpeedLimitMultiplier: {reader.ReadSingle(32)}");

            if (reader.ReadBit())
                builder.AppendLine($"AVDCollisionSpinDampeningMultiplier: {reader.ReadSingle(32)}");

            if (reader.ReadBit())
                builder.AppendLine($"AVDNormalSpinDampeningMultiplier: {reader.ReadSingle(32)}");

            if (reader.ReadBit())
            {
                builder.AppendLine($"CoidCurrentPathID: {reader.ReadInt(18)}");
                builder.AppendLine($"ExtraPathId: {reader.ReadInt(32)}");
                builder.AppendLine($"PathReversing: {reader.ReadBit()}");
                builder.AppendLine($"PathIsRoad: {reader.ReadBit()}");
                builder.AppendLine($"PatrolDistance: {reader.ReadInt(32)}");
            }

            if (reader.ReadBit())
                builder.AppendLine($"Template: {reader.ReadInt(20)}");

            if (reader.ReadBit())
                builder.AppendLine($"CoidSpawnOwner: {reader.ReadInt(20)}");

            var trickCount = reader.ReadInt(8);
            builder.AppendLine($"TrickCount: {trickCount}");

            for (var i = 0; i < trickCount; ++i)
                builder.AppendLine($"Trick[{i}]: {reader.ReadInt(16)}");

            builder.AppendLine($"IsTrailer: {reader.ReadBit()}");

            if (reader.ReadBit())
            {
                builder.AppendLine("Owner").AppendLine("[").IncreaseDepth();

                builder.AppendLine($"Coid: {reader.ReadInt(64)}");
                builder.AppendLine($"Global: {reader.ReadBit()}");
                builder.AppendLine($"CBID: {reader.ReadInt(20)}");

                if (reader.ReadBit())
                {
                    builder.AppendLine("Character").AppendLine("[").IncreaseDepth();
                    builder.AppendLine($"Name: {reader.ReadString()}");
                    builder.AppendLine($"ClanName: {reader.ReadString()}");
                    builder.AppendLine($"Level: {reader.ReadInt(8)}");
                    builder.AppendLine($"IsPossessingCreature: {reader.ReadBit()}");
                    builder.AppendLine($"VehicleName: {reader.ReadString()}");
                    builder.AppendLine($"HeadId: {reader.ReadInt(16)}");
                    builder.AppendLine($"BodyId: {reader.ReadInt(16)}");
                    builder.AppendLine($"HeadDetail1: {reader.ReadInt(16)}");
                    builder.AppendLine($"HeadDetail2: {reader.ReadInt(16)}");
                    builder.AppendLine($"MouthId: {reader.ReadInt(16)}");
                    builder.AppendLine($"EyesId: {reader.ReadInt(16)}");
                    builder.AppendLine($"HelmetId: {reader.ReadInt(16)}");
                    builder.AppendLine($"HairId: {reader.ReadInt(16)}");
                }
                else
                {
                    builder.AppendLine("Creature").AppendLine("[").IncreaseDepth();

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

                    builder.AppendLine($"DoesntCountAsSummon: {reader.ReadBit()}");
                    builder.AppendLine($"Level: {reader.ReadInt(8)}");
                    builder.AppendLine($"IsElite: {reader.ReadBit()}");
                }

                builder.DecreaseDepth().AppendLine("]");
                builder.DecreaseDepth().AppendLine("]");
            }
        }
        else
        {
            if (reader.ReadBit())
            {
                if (reader.ReadBit())
                    UnpackSkills(builder, reader);

                UnpackSkills(builder, reader);
            }
        }

        if (reader.ReadBit())
        {
            builder.AppendLine().AppendLine("WheelSet").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"CBID: {reader.ReadInt(20)}");
            builder.AppendLine($"Coid: {reader.ReadInt(64)}");
            builder.AppendLine($"Global: {reader.ReadBit()}");
            builder.DecreaseDepth().AppendLine("]");
        }

        if (reader.ReadBit() && reader.ReadBit())
        {
            builder.AppendLine().AppendLine("Front weapon").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"CBID: {reader.ReadInt(20)}");
            builder.AppendLine($"Coid: {reader.ReadInt(64)}");
            builder.AppendLine($"Global: {reader.ReadBit()}");
            builder.DecreaseDepth().AppendLine("]");
        }

        if (reader.ReadBit() && reader.ReadBit())
        {
            builder.AppendLine().AppendLine("Turret weapon").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"CBID: {reader.ReadInt(20)}");
            builder.AppendLine($"Coid: {reader.ReadInt(64)}");
            builder.AppendLine($"Global: {reader.ReadBit()}");
            builder.DecreaseDepth().AppendLine("]");
        }

        if (reader.ReadBit() && reader.ReadBit())
        {
            builder.AppendLine().AppendLine("Rear weapon").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"CBID: {reader.ReadInt(20)}");
            builder.AppendLine($"Coid: {reader.ReadInt(64)}");
            builder.AppendLine($"Global: {reader.ReadBit()}");
            builder.DecreaseDepth().AppendLine("]");
        }

        if (reader.ReadBit() && reader.ReadBit())
        {
            builder.AppendLine().AppendLine("Melee weapon").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"CBID: {reader.ReadInt(20)}");
            builder.AppendLine($"Coid: {reader.ReadInt(64)}");
            builder.AppendLine($"Global: {reader.ReadBit()}");
            builder.DecreaseDepth().AppendLine("]");
        }

        if (reader.ReadBit() && reader.ReadBit())
        {
            builder.AppendLine().AppendLine("Ornament").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"CBID: {reader.ReadInt(20)}");
            builder.AppendLine($"Coid: {reader.ReadInt(64)}");
            builder.AppendLine($"Global: {reader.ReadBit()}");
            builder.DecreaseDepth().AppendLine("]");
        }

        if (reader.ReadBit() && reader.ReadBit())
        {
            builder.AppendLine().AppendLine("Armor").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"CBID: {reader.ReadInt(20)}");
            builder.AppendLine($"Coid: {reader.ReadInt(64)}");
            builder.AppendLine($"Global: {reader.ReadBit()}");
            builder.AppendLine($"Armor[0]: {reader.ReadInt(16)}");
            builder.AppendLine($"Armor[1]: {reader.ReadInt(16)}");
            builder.AppendLine($"Armor[2]: {reader.ReadInt(16)}");
            builder.AppendLine($"Armor[3]: {reader.ReadInt(16)}");
            builder.AppendLine($"Armor[4]: {reader.ReadInt(16)}");
            builder.AppendLine($"Armor[5]: {reader.ReadInt(16)}");
            builder.DecreaseDepth().AppendLine("]");
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
            builder.DecreaseDepth().AppendLine("]").AppendLine();

            builder.AppendLine($"Flags1: {reader.ReadInt(8)}");
            builder.AppendLine($"Flags2: {reader.ReadInt(8)}");
            builder.AppendLine($"Acceleration: {reader.ReadSignedFloat(6)}");
            builder.AppendLine($"Steering: {reader.ReadSignedFloat(6)}");
            builder.AppendLine($"Unk: {reader.ReadSingle(32)}");

            builder.DecreaseDepth().AppendLine("]");
        }

        if (reader.ReadBit())
        {
            builder.AppendLine("Target").AppendLine("[").IncreaseDepth();
            builder.AppendLine($"TargetCoid: {reader.ReadInt(64)}");
            builder.AppendLine($"TargetGlobal: {reader.ReadBit()}");
            builder.DecreaseDepth().AppendLine("]");
        }

        if (reader.ReadBit())
        {
            builder.AppendLine($"AttribCombat: {reader.ReadInt(32)}");
            builder.AppendLine($"AttribPerception: {reader.ReadInt(32)}");
            builder.AppendLine($"AttribTech: {reader.ReadInt(32)}");
            builder.AppendLine($"AttribTheory: {reader.ReadInt(32)}");
        }

        if (reader.ReadBit())
            builder.AppendLine($"Heat: {reader.ReadInt(32)}");

        if (reader.ReadBit())
            builder.AppendLine($"MaxShield: {reader.ReadInt(32)}");

        if (reader.ReadBit())
            builder.AppendLine($"Shield: {reader.ReadInt(32)}");

        if (reader.ReadBit())
        {
            builder.AppendLine($"Power: {reader.ReadInt(32)}"); // TODO: reenable after a new dump
        }

        if (reader.ReadBit())
            builder.AppendLine($"GivesToken: {reader.ReadBit()}");

        builder.DecreaseDepth().AppendLine("]");
    }
}
