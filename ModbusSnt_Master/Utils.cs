using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusSnt
{
    static class UtilsSnt
    {
        public static class BitConverterSnt
        {
            //=====================================================================
            public static byte[] GetBytes(ushort input)
            {
                byte[] result = BitConverter.GetBytes(input);

                if (BitConverter.IsLittleEndian != IsLittleEndian)
                    Array.Reverse(result);

                return result;
            }

            //=====================================================================
            public static ushort ToUInt16(byte[] input, int startIndex)
            {
                if(BitConverter.IsLittleEndian == IsLittleEndian)
                    return BitConverter.ToUInt16(input, startIndex);
                else
                {
                    byte[] auxByte = new byte[2];
                    Array.Copy(input, startIndex, auxByte, 0, 2);
                    Array.Reverse(auxByte);
                    return BitConverter.ToUInt16(auxByte, 0);
                }
            }



            public static bool IsLittleEndian { get; set; } = false;
        }

        public static class BoolConverterSnt
        {
            //=====================================================================
            public static byte[] ToByte (bool[] input)
            {
                byte[] result = new byte[(ushort)((input.Length / 8) + ((input.Length % 8) > 0 ? 1 : 0))];

                for (int i = 0; i < result.Length; i++)
                {
                    int lenghtCurrentByte = (input.Length >= (i + 1) * 8) ? 8 : input.Length % 8;
                    for (int j = 0; j < lenghtCurrentByte; j++)                   
                        result[i] = (byte)(result[i] + ((byte)(input[i * 8 + j] ? Math.Pow(2, j) : 0)));
                    
                }
                

                if (!IsLittleEndian) Array.Reverse(result);

                return result;
            }

            //=====================================================================
            public static bool[] ToBool(byte[] input, int boolLenght)
            {
                if (!IsLittleEndian) Array.Reverse(input);

                bool[] result = new bool[boolLenght];

                for(int i = 0; i < input.Length ; i++)                
                    for(int j = 0; j < 8; j++)                    
                        if(result.Length > i * 8 + j)
                            result[i * 8 + j] = (input[i] & (1 << j)) == 0 ? false : true;
                return result;
            }
            public static bool IsLittleEndian { get; set; } = true;
        }
    }
}
