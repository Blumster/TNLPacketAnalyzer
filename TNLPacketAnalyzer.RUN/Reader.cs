using System;
using System.Text;

namespace TNLPacketAnalyzer.RUN
{
    public class Reader
    {
        private readonly Byte[] _array;
        private readonly Byte[] _stringBuffer = new Byte[256];
        private Int32 _bytePos;
        private Int32 _bitPos;

        public Reader(Byte[] array)
        {
            _array = array;
            _bytePos = 0;
            _bitPos = 0;
        }

        public Boolean PeekBit(Int32 bit)
        {
            return ((_array[_bytePos] >> bit) & 1) == 1;
        }

        public Boolean ReadBit()
        {
            var bit = PeekBit(_bitPos);

            IncreaseBitPos(1);

            return bit;
        }

        public Int64 ReadInt(Int64 bits)
        {
            var ret = 0L;

            for (var i = 0; i < bits; ++i)
                if (ReadBit())
                    ret |= 1L << i;

            return ret;
        }

        public void IncreaseBitPos(Int32 by)
        {
            _bitPos += by;
            _bytePos += _bitPos / 8;
            _bitPos %= 8;
        }

        public void ToNextByte()
        {
            if (_bitPos == 0)
                return;

            _bitPos = 0;
            ++_bytePos;
        }

        public String ReadString()
        {
            String stringBuf;

            ReadHuffBuffer(out stringBuf, (Byte)(ReadBit() ? ReadInt(8) : 0));

            return stringBuf;
        }

        public Int64 ReadRangedInt(Int32 start, Int32 end)
        {
            var size = end - start + 1;
            var bits = Utils.GetNextBinLog2(size);

            return ReadInt(bits) + start;
        }

        public Boolean IsFinished()
        {
            return _bytePos == _array.Length || (_bytePos == _array.Length - 1 && _bitPos != 0);
        }

        private void ReadHuffBuffer(out String stringBuffer, Byte off = 0)
        {
            HuffmanTree.Build();

            Int64 len;

            if (ReadBit())
            {
                len = ReadInt(8);

                for (var i = 0; i < len; ++i)
                {
                    var current = HuffmanTree.Root;

                    while (true)
                    {
                        if (!HuffmanTree.IsLeaf(current))
                        {
                            current = ReadBit() ? current.Right : current.Left;
                            continue;
                        }

                        _stringBuffer[i + off] = current.Symbol;
                        break;
                    }
                }
            }
            else
            {
                len = ReadInt(8);

                var buff = new Byte[len];

                for (var i = 0; i < len; ++i)
                    buff[i] = (Byte) ReadInt(8);

                Array.Copy(buff, 0, _stringBuffer, off, len);

                _stringBuffer[off + len] = 0;
            }

            stringBuffer = Encoding.UTF8.GetString(_stringBuffer, 0, (Int32)len + off);
        }
    }
}
