using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace nboard
{
    static class ShaCrypter
    {
        public static byte[] Xor(byte[] input, string key)
        {
            byte[] sha = SHA512.Create().ComputeHash(NanoEncoding.GetBytes(key));
            byte[] output = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (byte) (input[i] ^ sha[i & 63]);
            }

            return output;
        }
    }
}