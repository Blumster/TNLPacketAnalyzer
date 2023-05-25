using System;
using System.Collections.Generic;
using System.Linq;

namespace TNLPacketAnalyzer.CLI;

public class NetInterface
{
    private bool IsClient { get; set; }
    private string IP { get; set; }
    private Reader Reader { get; set; }
    private OutputBuilder Builder { get; set; }

    public List<NetConnection> ClientConnections { get; } = new();
    public List<NetConnection> ServerConnections { get; } = new();

    public static NetConnection Create(string className)
    {
        return className switch
        {
            "TNLConnection" => new TNLConnection(),
            "GhostConnection" => new GhostConnection(),
            "EventConnection" => new EventConnection(),
            "NetConnection" => new NetConnection(),
            _ => throw new NotSupportedException()
        };
    }

    public string ProcessPacket(byte[] data, bool isRecv, string ip)
    {
        IsClient = isRecv;
        IP = ip;
        Reader = new Reader(data);
        Builder = new OutputBuilder();

        try
        {
            if (Reader.PeekBit(7))
                AnalyzeDataPacket();
            else
                AnalyzeConnectionPacket();
        }
        catch (Exception ex)
        {
            Builder.AppendLine($"EXCEPTION: {ex}");
        }

        if (!Reader.IsFinished())
            Builder.Append("ERROR: Packet isn't finished!");

        return Builder.AppendLine().ToString();
    }

    private void AnalyzeDataPacket()
    {
        Builder.AppendLine($"DataPacket (0/{Reader.BitSize - 8})").AppendLine("[");
        Builder.IncreaseDepth();

        // Packet Header
        var type = Reader.ReadInt(2);
        Builder.AppendLine($"Opcode: {Const.NetPacketType[Math.Min(type, 3)]}");

        var sequenceNum = Reader.ReadInt(5);
        Reader.ReadBit();

        Builder.AppendLine($"Sequence Number: {sequenceNum | (Reader.ReadInt(6) << 5)}");
        Builder.AppendLine($"Highest Ack: {Reader.ReadInt(10)}");

        var ackBCount = Reader.ReadRangedInt(0, 4);
        Builder.AppendLine($"Ack Byte Count: {ackBCount}");

        var ackWCount = (ackBCount + 3) >> 2;
        for (var i = 0; i < ackWCount; ++i)
            Builder.AppendLine($"Ack Mask[{i}]: {Reader.ReadInt(i == ackWCount - 1 ? (ackBCount - (i * 4)) * 8 : 32)}");

        Builder.AppendLine($"Send Delay: {(Reader.ReadInt(8) << 3) + 4}");

        if (type == 0)
        {
            // Rate Info
            var rate = Reader.ReadBit();
            Builder.AppendLine($"Rate Info: {rate}");

            if (rate)
            {
                var adaptive = Reader.ReadBit();
                Builder.AppendLine($"Adaptive: {adaptive}");

                if (!adaptive)
                {
                    Builder.AppendLine($"Max Recv Bandwith: {Reader.ReadRangedInt(0, 65535)}");
                    Builder.AppendLine($"Max Send Bandwith: {Reader.ReadRangedInt(0, 65535)}");
                    Builder.AppendLine($"Min Packet Recv Period: {Reader.ReadRangedInt(1, 2047)}");
                    Builder.AppendLine($"Min Packet Send Period: {Reader.ReadRangedInt(1, 2047)}");
                }
            }

            var conn = (IsClient ? ClientConnections : ServerConnections).Single(x => x.IP == IP);
            conn.ReadPacket(Builder, Reader, IsClient);
        }

        Builder.DecreaseDepth();
        Builder.AppendLine("]");
        Builder.AppendLine($"DataPacket ({Reader.BitPos - 8}/{Reader.BitSize - 8})");
    }

    private void AnalyzeConnectionPacket()
    {
        var packetType = Reader.ReadInt(8);

        Builder.AppendLine($"{Const.PacketType[Math.Min(packetType, 8)]} (0/{Reader.BitSize - 8})").AppendLine("[");
        Builder.IncreaseDepth();

        if (packetType >= 8)
            return;

        switch (packetType)
        {
            case 0:
                AnalyzeConnectChallengeRequestPacket();
                break;

            case 1:
                AnalyzeConnectChallengeResponsePacket();
                break;

            case 2:
                AnalyzeConnectRequestPacket();
                break;

            case 3:
                AnalyzeConnectRejectPacket();
                break;

            case 4:
                AnalyzeConnectAcceptPacket();
                break;

            case 5:
                AnalyzeDisconnectPacket();
                break;

            case 6:
                AnalyzePunchPacket();
                break;

            case 7:
                AnalyzeArrangedConnectRequestPacket();
                break;
        }

        Builder.DecreaseDepth();
        Builder.AppendLine("]");
        Builder.Append($"{Const.PacketType[Math.Min(packetType, 8)]} ({Reader.BitPos - 8}/{Reader.BitSize - 8})");
    }

    private void AnalyzeConnectChallengeRequestPacket()
    {
        Builder.AppendLine($"Client Nonce: {Reader.ReadInt(64):X16}");
        Builder.AppendLine($"Wants Key Exchange: {Reader.ReadBit()}");
        Builder.AppendLine($"Wants Certificate: {Reader.ReadBit()}");
    }

    private void AnalyzeConnectChallengeResponsePacket()
    {
        Builder.AppendLine($"Client Nonce: {Reader.ReadInt(64):X16}");
        Builder.AppendLine($"Client Identity: {Reader.ReadInt(32):X8}");
        Builder.AppendLine($"Server Nonce: {Reader.ReadInt(64):X16}");
        Builder.AppendLine($"Puzzle Difficulty: {Reader.ReadInt(32)}");

        var reqKey = Reader.ReadBit();

        Builder.AppendLine($"Requires Key Exchange: {reqKey}");

        if (reqKey)
        {
            var reqCert = Reader.ReadBit();
            Builder.AppendLine($"Requires Certificate: {reqCert}");
            Builder.Append(reqCert ? "Certificate: " : "AssymetricKey: ");

            EventConnection.ReadByteBuffer(Builder, Reader);
        }
    }

    private void AnalyzeConnectRequestPacket()
    {
        var clientNonce = Reader.ReadInt(64);
        Builder.AppendLine($"Client Nonce: {clientNonce:X16}");
        Builder.AppendLine($"Server Nonce: {Reader.ReadInt(64):X16}");
        Builder.AppendLine($"Client Identity: {Reader.ReadInt(32):X8}").AppendLine();
        Builder.AppendLine($"Puzzle Difficulty: {Reader.ReadInt(32)}");
        Builder.AppendLine($"Puzzle Solution: {Reader.ReadInt(32)}");

        var usingCrypto = Reader.ReadBit();

        Builder.AppendLine($"Using Crypto: {usingCrypto}");

        if (usingCrypto)
        {
            Builder.Append("Assymetric Key: ");
            EventConnection.ReadByteBuffer(Builder, Reader);

            Reader.ToNextByte();

            Builder.Append("SymmetricCipher: ");
            EventConnection.ReadByteArray(Builder, Reader, 16);
        }

        var debugObjectSizes = Reader.ReadBit();
        Builder.AppendLine($"Debug Object Sizes: {debugObjectSizes}");
        Builder.AppendLine($"Initial Send Sequence: {Reader.ReadInt(32)}");

        var className = Reader.ReadString();

        Builder.AppendLine($"Class Name: {className}");

        var recvConn = Create(className);
        recvConn.Nonce = clientNonce;
        recvConn.DebugObjectSizes = debugObjectSizes;
        recvConn.IP = IP;
        recvConn.ReadConnectRequest(Builder, Reader, true);
        ClientConnections.Add(recvConn);

        var sendConn = Create(className);
        sendConn.Nonce = clientNonce;
        sendConn.DebugObjectSizes = debugObjectSizes;
        sendConn.IP = IP;
        ServerConnections.Add(sendConn);
    }

    private void AnalyzeConnectRejectPacket()
    {
        var clientNonce = Reader.ReadInt(64);
        Builder.AppendLine($"Client Nonce: {clientNonce:X16}");
        Builder.AppendLine($"Server Nonce: {Reader.ReadInt(64):X16}");
        Builder.AppendLine($"Reason: {Reader.ReadString()}");

        ClientConnections.RemoveAll(x => x.Nonce == clientNonce);
        ServerConnections.RemoveAll(x => x.Nonce == clientNonce);
    }

    private void AnalyzeConnectAcceptPacket()
    {
        var clientNonce = Reader.ReadInt(64);
        Builder.AppendLine($"Client Nonce: {clientNonce:X16}");
        Builder.AppendLine($"Server Nonce: {Reader.ReadInt(64):X16}");

        Reader.ToNextByte();

        Builder.AppendLine($"Initial Recv Sequence: {Reader.ReadInt(32)}");

        var conn = ServerConnections.Single(c => c.Nonce == clientNonce);
        conn.ReadConnectAccept(Builder, Reader, false);
    }

    private void AnalyzeDisconnectPacket()
    {
        var clientNonce = Reader.ReadInt(64);
        Builder.AppendLine($"Client Nonce: {clientNonce:X16}");
        Builder.AppendLine($"Server Nonce: {Reader.ReadInt(64):X16}");

        Reader.ToNextByte();

        Builder.AppendLine($"Reason: {Reader.ReadString()}");

        ClientConnections.RemoveAll(x => x.Nonce == clientNonce);
        ServerConnections.RemoveAll(x => x.Nonce == clientNonce);
    }

    private void AnalyzePunchPacket()
    {
        Builder.AppendLine($"Initiator Nonce: {Reader.ReadInt(64):X16}");

        Reader.ToNextByte();

        Builder.AppendLine($"Other Nonce: {Reader.ReadInt(64):X16}");

        // todo
    }

    private void AnalyzeArrangedConnectRequestPacket()
    {
        Builder.AppendLine($"Initiator Nonce: {Reader.ReadInt(64):X16}");

        // todo
    }
}
