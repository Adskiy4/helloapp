using System;
using System.Text;

namespace HelloApp
{
    class Programm
    {
        public static void PrintHelloWhite()
        {
            Console.WriteLine("hello white");
        }

        public static void PrintHelloBlack()
        {
            Console.WriteLine("Hello black");
        }
        
        public static void Main()
        {
            Console.WriteLine("hello everyone");
            PrintHelloBlack();
            PrintHelloWhite();
        }
    }
}