namespace TNLPacketAnalyzer.CLI
{
    public static class Utils
    {
        public static bool IsPow2(int number)
        {
            return number > 0 && (number & (number - 1)) == 0;
        }

        public static int GetBinLog2(int value)
        {
            var floatValue = (float) value;
            //var ret = Math.Floor(Math.Log(value, 2));

            unsafe
            {
                return (*((int*)&floatValue) >> 23) - 127;
            }
        }

        public static int GetNextBinLog2(int number)
        {
            return GetBinLog2(number) + (IsPow2(number) ? 0 : 1);
        }

        public static int GetNextPow2(int value)
        {
            return IsPow2(value) ? value : (1 << (GetBinLog2(value) + 1));
        }
    }
}
