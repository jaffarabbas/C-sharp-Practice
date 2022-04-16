using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsharpLeetCode
{
    public class twoSum
    {
        public int[] TwoSum(int[] nums, int target)
        {
            if(nums == null && nums.Length < 2)
            {
                return Array.Empty<int>();
            }
            for (int i = 0; i < nums.Length + 1; i++)
            {
                for (int j = i + 1; j < nums.Length; j++)
                {
                    if (nums[i] + nums[j] == target)
                    {
                        return new int[] {i,j};
                    }
                }
            }
            return Array.Empty <int> ();
        }
        //static void Main(string[] args)
        //{
        //    int[] array = { 1, 2, 3, 5 };
        //    int target = 3;
        //    twoSum s = new twoSum();
        //    foreach (var item in s.TwoSum(array, target))
        //    {
        //        Console.WriteLine(item);
        //    }
        //    Console.Read();
        //}
    }
}
