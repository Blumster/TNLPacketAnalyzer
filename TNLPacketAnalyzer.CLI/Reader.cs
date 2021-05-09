using System;
using System.Text;

namespace TNLPacketAnalyzer.CLI
{
    public class Reader
    {
        private readonly byte[] _array;
        private readonly byte[] _stringBuffer = new byte[256];
        private int _bytePos;
        private int _bitPos;

        public Reader(byte[] array)
        {
            _array = array;
            _bytePos = 0;
            _bitPos = 0;
        }

        public bool PeekBit(int bit)
        {
            return ((_array[_bytePos] >> bit) & 1) == 1;
        }

        public bool ReadBit()
        {
            var bit = PeekBit(_bitPos);

            IncreaseBitPos(1);

            return bit;
        }

        public long ReadInt(long bits)
        {
            var ret = 0L;

            for (var i = 0; i < bits; ++i)
                if (ReadBit())
                    ret |= 1L << i;

            return ret;
        }

        public void IncreaseBitPos(int by)
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

        public string ReadString()
        {
            string stringBuf;

            ReadHuffBuffer(out stringBuf, (byte)(ReadBit() ? ReadInt(8) : 0));

            return stringBuf;
        }

        public long ReadRangedInt(int start, int end)
        {
            var size = end - start + 1;
            var bits = Utils.GetNextBinLog2(size);

            return ReadInt(bits) + start;
        }

        public bool IsFinished()
        {
            return _bytePos == _array.Length || (_bytePos == _array.Length - 1 && _bitPos != 0);
        }

        private void ReadHuffBuffer(out string stringBuffer, byte off = 0)
        {
            HuffmanTree.Build();

            long len;

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

                var buff = new byte[len];

                for (var i = 0; i < len; ++i)
                    buff[i] = (byte) ReadInt(8);

                Array.Copy(buff, 0, _stringBuffer, off, len);

                _stringBuffer[off + len] = 0;
            }

            stringBuffer = Encoding.UTF8.GetString(_stringBuffer, 0, (int)len + off);
        }
    }
}
