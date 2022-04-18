using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsharpLeetCode
{
    public class Majority_Element
    {
        public void SelectionSort(int[] arr)
        {
            int temp, smallest;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                smallest = i;
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (arr[j] < arr[smallest])
                    {
                        smallest = j;
                    }
                }
                temp = arr[smallest];
                arr[smallest] = arr[i];
                arr[i] = temp;
            }
        }
        public int MajorityElement(int[] nums)
        {
            SelectionSort(nums);
            int maxCount = 0;
            int index = -1;
            for (int i = 0; i < nums.Length; i++)
            {
            int count = 0;
                for (int j = 0; j < nums.Length; j++)
                {
                    if (nums[i] == nums[j])
                        count++;
                }
                if(count > maxCount)
                    maxCount = count;
                    index = i;
            }
            if (maxCount > nums.Length / 2)
                return nums[index];
            else
                return 0;
        }
    }
}
