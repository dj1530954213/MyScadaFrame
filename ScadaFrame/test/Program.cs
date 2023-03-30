using System;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            ushort a = 55;
            int b = 55;
            float c = (float)55.00;
            object e = a;
            if (c  == Convert.ToSingle(e))
            {
                Console.WriteLine("YESa");
            }
            e = b;
            if (c == Convert.ToSingle(e))
            {
                Console.WriteLine("YESb");
            }
            Console.Write(true);

        }
    }
}
