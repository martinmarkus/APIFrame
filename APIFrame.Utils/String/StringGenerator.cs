using System;
using System.Text;

namespace APIFrame.Utils.String
{
    public class StringGenerator
    {
        public string GetRandomString(int size = 8)
        {
            char offset = 'A';
            int lettersOffset = 26;
            var codeBuilder = new StringBuilder(size);
            var random = new Random();

            for (var i = 0; i < size; i++)
            {
                try
                {
                    var @char = (char)random.Next(offset, offset + lettersOffset);
                    codeBuilder.Append(@char);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return codeBuilder.ToString();
        }
    }
}
