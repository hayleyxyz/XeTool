using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace XeLib.Security
{
    public static class XeCrypt
    {
        public static byte[] XeCryptHmacSha(byte[] key, byte[] input, int inputOffset, int inputCount) {
            var sha = new HMACSHA1(key);
            
            sha.TransformFinalBlock(input, inputOffset, inputCount);

            return sha.Hash;
        }

        public class XeCryptRc4State {
            public byte[] perm;
            public byte index1;
            public byte index2;

            public XeCryptRc4State() {
                perm = new byte[256];
            }
        }

        public static XeCryptRc4State XeCryptRc4Key(byte[] key) {
            var state = new XeCryptRc4State();

            byte j, temp;
            int i;

            /* Initialize state with identity permutation */
            for (i = 0; i < 256; i++) {
                state.perm[i] = (byte)i;
            }

            state.index1 = 0;
            state.index2 = 0;

            /* Randomize the permutation using key data */
            for (j = 0, i = 0; i < 256; i++) {
                j += (byte)(state.perm[i] + key[i % key.Length]);

                temp = state.perm[i];
                state.perm[i] = state.perm[j];
                state.perm[j] = temp;
            }

            return state;
        }

        public static void XeCryptRc4Ecb(XeCryptRc4State state, ref byte[] inOut, int inputOffset, int inputLength) {
            int i;
            byte j, temp;

            for (i = 0; i < inputLength; i++) {

                /* Update modification indicies */
                state.index1++;
                state.index2 += state.perm[state.index1];

                /* Modify permutation */
                temp = state.perm[state.index1];
                state.perm[state.index1] = state.perm[state.index2];
                state.perm[state.index2] = temp;

                /* Encrypt/decrypt next byte */
                j = (byte)(state.perm[state.index1] + state.perm[state.index2]);
                inOut[inputOffset + i] ^= state.perm[j];
            }
        }
    }
}
