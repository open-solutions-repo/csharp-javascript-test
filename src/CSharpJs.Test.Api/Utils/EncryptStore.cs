using System;
using System.Linq;

namespace CSharpJs.Test.Api.Utils
{
    public static class EncryptStore
    {
        private static Random random = new Random();

        public static string Encrypt(string code)
        {
            string response = "";

            var value = (Convert.ToInt32(code) + Convert.ToInt32(DateTime.Now.ToString("yyyy"))).ToString();

            var valueArray = value.ToCharArray(0, value.Length);

            valueArray.ToList().ForEach(val =>
            {
                response += RandomString(1) + val + RandomString(1);
            });

            return response;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ%*/=[]{}#$()<>&@";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
