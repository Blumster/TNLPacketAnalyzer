using System;
using System.Text;

namespace TNLPacketAnalyzer.RUN
{
    public class Analyzer
    {
        public Reader Reader { get; set; }

        public Analyzer(Byte[] array)
        {
            Reader = new Reader(array);
        }

        public String Run()
        {
            var sb = new StringBuilder();

            if (Reader.PeekBit(7))
                AnalyzePacket(sb);
            else
                AnalyzeConnectionPacket(sb);

            if (!Reader.IsFinished())
                sb.AppendLine("Packet isn't finished!");

            return sb.AppendLine().ToString();
        }

        #region Data Packets
        private void AnalyzePacket(StringBuilder sb)
        {
            sb.AppendLine("Data packet!").AppendLine("[");

            // Packet Header
            var type = Reader.ReadInt(2);
            sb.Append("    Opcode: ").AppendLine(Const.NetPacketType[Math.Min(type, 3)]);

            var sequenceNum = Reader.ReadInt(5);
            Reader.ReadBit();

            sb.Append("    Sequence Number: ").Append(sequenceNum | (Reader.ReadInt(6) << 5)).AppendLine();
            sb.Append("    Highest Ack: ").Append(Reader.ReadInt(10)).AppendLine();

            var ackBCount = Reader.ReadRangedInt(0, 4);
            sb.Append("    Ack Byte Count: ").Append(ackBCount).AppendLine();

            var ackWCount = (ackBCount + 3) >> 2;
            for (var i = 0; i < ackWCount; ++i)
                sb.Append("    Ack Mask[").Append(i).Append("]: ").Append(Reader.ReadInt(i == ackWCount - 1 ? (ackBCount - (i * 4)) * 8 : 32)).AppendLine();

            sb.Append("    Send Delay: ").Append((Reader.ReadInt(8) << 3) + 4).AppendLine();

            if (type != 0)
            {
                sb.Append("]");
                return;
            }

            // Rate Info
            var rate = Reader.ReadBit();
            sb.Append("    Rate Info: ").Append(rate).AppendLine();

            if (rate)
            {
                var adaptive = Reader.ReadBit();
                sb.Append("    Adaptive: ").Append(adaptive).AppendLine();

                if (!adaptive)
                {
                    sb.Append("    Max Recv Bandwith: ").Append(Reader.ReadRangedInt(0, 65535)).AppendLine();
                    sb.Append("    Max Send Bandwith: ").Append(Reader.ReadRangedInt(0, 65535)).AppendLine();
                    sb.Append("    Min Packet Recv Period: ").Append(Reader.ReadRangedInt(1, 2047)).AppendLine();
                    sb.Append("    Min Packet Send Period: ").Append(Reader.ReadRangedInt(1, 2047)).AppendLine();
                }
            }

            // NetConnection::ReadPacket

            // EventConnetion::ReadPacket
            var prevSeq = -2L;
            var ungaranteedPhase = true;
            while (true)
            {
                var bit = Reader.ReadBit();
                if (ungaranteedPhase && !bit)
                {
                    ungaranteedPhase = false;
                    bit = Reader.ReadBit();
                }

                if (!ungaranteedPhase && !bit)
                    break;

                sb.Append("    Event Block").AppendLine().AppendLine("    [");

                if (!ungaranteedPhase)
                {
                    Int64 seq;
                    if (Reader.ReadBit())
                        seq = (prevSeq + 1) & 0x7F;
                    else
                        seq = Reader.ReadInt(7);

                    sb.Append("        Sequence: ").Append(seq).AppendLine();
                    prevSeq = seq;
                }

                var classId = Reader.ReadInt(4);
                sb.Append("        Class Id: ").Append(classId).Append(" (").Append(Const.ClassNames[Math.Min(classId, 8)]).AppendLine(")");

                AnalyzeByClassId(sb, classId);

                sb.AppendLine("    ]");
            }

            sb.Append("]");
        }

        private void AnalyzeByClassId(StringBuilder sb, Int64 classId)
        {
            sb.AppendLine("        Data Block").AppendLine("        [");

            switch (classId)
            {
                case 0:
                    // Empty
                    break;

                case 1:
                    AnalyzeRPCMsgReadyForNormalGhosts(sb);
                    break;

                case 2:
                    AnalyzeRPCMsgStartGhosting(sb);
                    break;

                case 3:
                    AnalyzeRPCMsgGuaranteed(sb);
                    break;

                case 4:
                    AnalyzeRPCMsgGuaranteedFragmented(sb);
                    break;

                case 5:
                    AnalyzeRPCMsgGuaranteedOrdered(sb);
                    break;

                case 6:
                    AnalyzeRPCMsgGuaranteedOrderedFragmented(sb);
                    break;

                case 7:
                    AnalyzeRPCMsgNonGuaranteed(sb);
                    break;

                case 8:
                    AnalyzeRPCMsgNonGuaranteedFragmented(sb);
                    break;
            }

            sb.AppendLine("        ]");
        }

        private void AnalyzeRPCMsgReadyForNormalGhosts(StringBuilder sb)
        {
            sb.Append("            Sequence: ").Append(Reader.ReadInt(32)).AppendLine();
        }

        private void AnalyzeRPCMsgStartGhosting(StringBuilder sb)
        {
            sb.Append("            Sequence: ").Append(Reader.ReadInt(32)).AppendLine();
        }

        private void AnalyzeRPCMsgGuaranteed(StringBuilder sb)
        {
            sb.Append("            Type: ").Append(Reader.ReadInt(32)).AppendLine();
            sb.Append("            Buffer:");

            ReadByteBuffer(sb);
        }

        private void AnalyzeRPCMsgGuaranteedFragmented(StringBuilder sb)
        {
            sb.Append("            Type: ").Append(Reader.ReadInt(32)).AppendLine();
            sb.Append("            Fragment: ").Append(Reader.ReadInt(16)).AppendLine();
            sb.Append("            FragmentId: ").Append(Reader.ReadInt(16)).AppendLine();
            sb.Append("            FragmentCount: ").Append(Reader.ReadInt(16)).AppendLine();
            sb.Append("            Buffer:");

            ReadByteBuffer(sb);
        }

        private void AnalyzeRPCMsgGuaranteedOrdered(StringBuilder sb)
        {
            sb.Append("            Type: ").Append(Reader.ReadInt(32)).AppendLine();
            sb.Append("            Buffer:");

            ReadByteBuffer(sb);
        }

        private void AnalyzeRPCMsgGuaranteedOrderedFragmented(StringBuilder sb)
        {
            sb.Append("            Type: ").Append(Reader.ReadInt(32)).AppendLine();
            sb.Append("            Fragment: ").Append(Reader.ReadInt(16)).AppendLine();
            sb.Append("            FragmentId: ").Append(Reader.ReadInt(16)).AppendLine();
            sb.Append("            FragmentCount: ").Append(Reader.ReadInt(16)).AppendLine();
            sb.Append("            Buffer:");

            ReadByteBuffer(sb);
        }

        private void AnalyzeRPCMsgNonGuaranteed(StringBuilder sb)
        {
            sb.Append("            Type: ").Append(Reader.ReadInt(32)).AppendLine();
            sb.Append("            Buffer:");

            ReadByteBuffer(sb);
        }

        private void AnalyzeRPCMsgNonGuaranteedFragmented(StringBuilder sb)
        {
            sb.Append("            Type: ").Append(Reader.ReadInt(32)).AppendLine();
            sb.Append("            Fragment: ").Append(Reader.ReadInt(16)).AppendLine();
            sb.Append("            FragmentId: ").Append(Reader.ReadInt(16)).AppendLine();
            sb.Append("            FragmentCount: ").Append(Reader.ReadInt(16)).AppendLine();
            sb.Append("            Buffer:");

            ReadByteBuffer(sb);
        }
        #endregion Data Packets
        // sb.Append(": ").Append("").AppendLine();
        #region Connection Packets
        private void AnalyzeConnectionPacket(StringBuilder sb)
        {
            sb.AppendLine("Connection packet!").AppendLine("[");

            var packetType = Reader.ReadInt(8);
            sb.Append("    Opcode: ").AppendLine(Const.PacketType[Math.Min(packetType, 8)]);

            if (packetType >= 8)
                return;

            switch (packetType)
            {
                case 0:
                    AnalyzeConnectChallengeRequestPacket(sb);
                    break;

                case 1:
                    AnalyzeConnectChallengeResponsePacket(sb);
                    break;

                case 2:
                    AnalyzeConnectRequestPacket(sb);
                    break;

                case 3:
                    AnalyzeConnectRejectPacket(sb);
                    break;

                case 4:
                    AnalyzeConnectAcceptPacket(sb);
                    break;

                case 5:
                    AnalyzeDisconnectPacket(sb);
                    break;

                case 6:
                    AnalyzePunchPacket(sb);
                    break;

                case 7:
                    AnalyzeArrangedConnectRequestPacket(sb);
                    break;
            }

            sb.Append("]");
        }

        private void AnalyzeConnectChallengeRequestPacket(StringBuilder sb)
        {
            sb.Append("    Client Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));
            sb.Append("    Wants Key Exchange: ").Append(Reader.ReadBit()).AppendLine();
            sb.Append("    Wants Certificate: ").Append(Reader.ReadBit()).AppendLine();
        }

        private void AnalyzeConnectChallengeResponsePacket(StringBuilder sb)
        {
            sb.Append("    Client Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));
            sb.Append("    Client Identity: ").AppendLine(Reader.ReadInt(32).ToString("X8"));
            sb.Append("    Server Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));
            sb.Append("    Puzzle Difficulty: ").Append(Reader.ReadInt(32)).AppendLine();

            var reqKey = Reader.ReadBit();
            sb.Append("    Requires Key Exchange: ").Append(reqKey).AppendLine();
            if (reqKey)
            {
                var reqCert = Reader.ReadBit();
                sb.Append("    Requires Certificate: ").Append(reqCert).AppendLine();
                sb.Append(reqCert ? "    Certificate: " : "    AssymetricKey: ");

                ReadByteBuffer(sb);
            }
        }

        private void AnalyzeConnectRequestPacket(StringBuilder sb)
        {
            sb.Append("    Client Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));
            sb.Append("    Server Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));
            sb.Append("    Client Identity: ").AppendLine(Reader.ReadInt(32).ToString("X8"));
            sb.Append("    Puzzle Difficulty: ").Append(Reader.ReadInt(32)).AppendLine();
            sb.Append("    Puzzle Solution: ").Append(Reader.ReadInt(32)).AppendLine();

            var usingCrypto = Reader.ReadBit();
            sb.Append("    Using Crypto: ").Append(usingCrypto).AppendLine();
            if (usingCrypto)
            {
                sb.Append("    Assymetric Key: ");
                ReadByteBuffer(sb);

                Reader.ToNextByte();

                sb.Append("    SymmetricCipher: ");
                ReadByteArray(sb, 16);
            }

            sb.Append("    Debug Object Sizes: ").Append(Reader.ReadBit()).AppendLine();
            sb.Append("    Initial Send Sequence: ").Append(Reader.ReadInt(32)).AppendLine();
            sb.Append("    Class Name: ").AppendLine(Reader.ReadString());

            // NetConnection::ConnectRequest
            sb.Append("    Net Class Group: ").Append(Reader.ReadInt(32)).AppendLine();
            sb.Append("    Net Class Group CRC: ").Append((Int32)Reader.ReadInt(32)).AppendLine();

            // EventConnection::ConnectRequest
            sb.Append("    Class Count: ").Append(Reader.ReadInt(32)).AppendLine();

            // GhostConnection::ConnectRequest

            // TNLConnection::ConnectRequest
            sb.Append("    Version: ").Append(Reader.ReadInt(32)).AppendLine();
            sb.Append("    Key: ").Append(Reader.ReadInt(32)).AppendLine();
            sb.Append("    Coid: ").Append(Reader.ReadInt(64)).AppendLine();
        }

        private void AnalyzeConnectRejectPacket(StringBuilder sb)
        {
            sb.Append("    Client Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));
            sb.Append("    Server Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));
            sb.Append("    Reason: ").AppendLine(Reader.ReadString());
        }

        private void AnalyzeConnectAcceptPacket(StringBuilder sb)
        {
            sb.Append("    Client Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));
            sb.Append("    Server Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));

            Reader.ToNextByte();

            sb.Append("    Initial Recv Sequence: ").Append(Reader.ReadInt(32)).AppendLine();

            // NetConnection::ConnectAccept

            // EventConnection::ConnectAccept
            sb.Append("    Event Class Count: ").Append(Reader.ReadInt(32)).AppendLine();
        }

        private void AnalyzeDisconnectPacket(StringBuilder sb)
        {
            sb.Append("    Client Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));
            sb.Append("    Server Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));

            Reader.ToNextByte();

            sb.Append("    Reason: ").AppendLine(Reader.ReadString());
        }

        private void AnalyzePunchPacket(StringBuilder sb)
        {
            sb.Append("    Initiator Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));

            Reader.ToNextByte();

            sb.Append("    Other Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));

            // todo
        }

        private void AnalyzeArrangedConnectRequestPacket(StringBuilder sb)
        {
            sb.Append("    Initiator Nonce: ").AppendLine(Reader.ReadInt(64).ToString("X16"));

            // todo
        }
        #endregion Connection Packets

        #region Helpers

        private void ReadByteBuffer(StringBuilder sb, Boolean newLine = true)
        {
            ReadByteArray(sb, Reader.ReadInt(10), newLine);
        }

        private void ReadByteArray(StringBuilder sb, Int64 length, Boolean newLine = true)
        {
            var arr = new Byte[length];

            for (var i = 0; i < arr.Length; ++i)
                arr[i] = (Byte)Reader.ReadInt(8);

            sb.Append(BitConverter.ToString(arr));

            if (newLine)
                sb.AppendLine();
        }
        #endregion
    }
}
