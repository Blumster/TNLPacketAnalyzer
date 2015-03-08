using System;

namespace TNLPacketAnalyzer.RUN
{
    public static class Utils
    {
        public static Boolean IsPow2(Int32 number)
        {
            return number > 0 && (number & (number - 1)) == 0;
        }

        public static Int32 GetBinLog2(Int32 value)
        {
            var floatValue = (Single) value;
            //var ret = Math.Floor(Math.Log(value, 2));

            unsafe
            {
                return (*((Int32*)&floatValue) >> 23) - 127;
            }
        }

        public static Int32 GetNextBinLog2(Int32 number)
        {
            return GetBinLog2(number) + (IsPow2(number) ? 0 : 1);
        }

        public static Int32 GetNextPow2(Int32 value)
        {
            return IsPow2(value) ? value : (1 << (GetBinLog2(value) + 1));
        }
    }
}
