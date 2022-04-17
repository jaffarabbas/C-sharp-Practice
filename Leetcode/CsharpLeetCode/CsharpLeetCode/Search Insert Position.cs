using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsharpLeetCode
{
    public class Search_Insert_Position
    {
        public int SearchInsert(int[] nums, int target)
        {
            for (int i = 0; i < nums.Length; i++)
                if (nums[i] == target) return i;
            for (int i = 0; i < nums.Length - 1; i++)
                if (nums[i] < target && nums[i + 1] > target) return i + 1;
            if (target < nums[0]) return 0;
            else return nums.Length;
        }
        static void Main(string[] args)
        {
            Roman_to_Integer p = new Roman_to_Integer();
            int a = p.RomanToInt("IV");
            Console.WriteLine(a);
            Console.Read();
        }

    }
}
