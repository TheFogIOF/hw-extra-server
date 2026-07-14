using System;
using System.Collections.Generic;
using System.Text;

namespace HWExtra_Server
{
    public static class ConsoleExt
    {
        public static void WriteLine(String text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Write(String text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
