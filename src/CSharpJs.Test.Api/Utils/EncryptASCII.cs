using System;

namespace CSharpJs.Test.Api.Utils
{
    public static class EncryptASCII
    {
        public static string Encrypt(string text)
        {
            var cryptic = "";

            for (int i = 0; i < text.Length; i++)
            {
                int ASCII = Convert.ToInt32(text.Substring(i, 1));

                int ASCIIC = ASCII + 10;

                cryptic += Char.ConvertFromUtf32(ASCIIC);
            }

            return cryptic;
        }

        public static string Decrypt(string text)
        {
            var desCryptic = "";

            for (int i = 0; i < text.Length; i++)
            {
                int ASCII = Convert.ToInt32(text.Substring(i, 1));

                int ASCIIC = ASCII - 10;

                desCryptic += Char.ConvertFromUtf32(ASCIIC);
            }

            return desCryptic;
        }
    }
}
