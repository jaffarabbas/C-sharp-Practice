using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsharpLeetCode
{
    public class Palindrome_Number
    {
        public bool IsPalindrome(int x)
        {
            return x.ToString().SequenceEqual(x.ToString().Reverse());
        }

        //static void Main(string[] args)
        //{
        //    Palindrome_Number p = new Palindrome_Number();
        //    bool a  = p.IsPalindrome(121);
        //    Console.WriteLine(a);
        //    Console.Read();
        //}
    }
}
