using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static class ArrayTools
    {


        public static T[,] TrimArray<T>(int? rowToRemove, int? columnToRemove, T[,] originalArray)            
        {
            int resultRows = rowToRemove.HasValue ? originalArray.GetLength(0) - 1 : originalArray.GetLength(0);
            int resultColumns = columnToRemove.HasValue ? originalArray.GetLength(1) - 1 : originalArray.GetLength(1);

            T[,] result = new T[resultRows, resultColumns];

            for (int i = 0, j = 0; i < originalArray.GetLength(0); i++)
            {
                if (i == rowToRemove)
                    continue;

                for (int k = 0, u = 0; k < originalArray.GetLength(1); k++)
                {
                    if (k == columnToRemove)
                        continue;

                    result[j, u] = originalArray[i, k];
                    u++;
                }
                j++;
            }

            return result;
        }
    }
}
