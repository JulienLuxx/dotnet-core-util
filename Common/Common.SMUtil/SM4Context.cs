namespace Common.SMUtil
{
    internal class Sm4Context
    {
        public int mode;

        public long[] sk;

        public bool isPadding;

        public Sm4Context()
        {
            mode = 1;
            isPadding = true;
            sk = new long[32];
        }
    }
}
