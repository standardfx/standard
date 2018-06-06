using System;
using System.Security;

namespace Standard
{
    /// <summary>
    /// Performance focused serialization service for primitive types.
    /// </summary>
    [SecuritySafeCritical]
    public static partial class FastConvert
    {
    	/*
    	// this is slower than .Substring(...) :(

    	public static string CreateString(string str)
    		=> CreateString(str, 0, str.Length);

    	public static string CreateString(string str, int startIndex)
    		=> CreateString(str, startIndex, str.Length);

		public unsafe static string CreateString(string str, int startIndex, int length) 
		{
			fixed (char* ptr = str)
			{
				return new string(ptr, startIndex, length);
			}
		}
		*/

        internal static unsafe void MemCpy(char* dmem, char* smem, int charCount) 
        {
            if ((((int)dmem) & 2) != 0) 
            {
                dmem[0] = smem[0];
                dmem++;
                smem++;
                charCount--;
            }

            while (charCount >= 8) 
            {
                *((int*)dmem) = *((int*)smem);
                *((int*)(dmem + 2)) = *((int*)(smem + 2));
                *((int*)(dmem + 4)) = *((int*)(smem + 4));
                *((int*)(dmem + 6)) = *((int*)(smem + 6));
                dmem += 8;
                smem += 8;
                charCount -= 8;
            }
            
            if ((charCount & 4) != 0) 
            {
                *((int*)dmem) = *((int*)smem);
                *((int*)(dmem + 2)) = *((int*)(smem + 2));
                dmem += 4;
                smem += 4;
            }
            
            if ((charCount & 2) != 0) 
            {
                *((int*)dmem) = *((int*)smem);
                dmem += 2;
                smem += 2;
            }
            
            if ((charCount & 1) != 0) 
                dmem[0] = smem[0];
        }
    }
}
