using System;
using System.Text;
using System.Security.Cryptography;

namespace FtpSharp.Server
{
    public sealed class SessionIdGenerator
    {
        public static readonly Random _random = new Random();
	
		public static string RandomString(int size, bool lowerCase = false)  
		{ 
			var builder = new StringBuilder(size);  

			// Unicode/ASCII Letters are divided into two blocks
			// (Letters 65–90 / 97–122):   
			// The first group containing the uppercase letters and
			// the second group containing the lowercase.  

			// char is a single Unicode character  
			char offset = lowerCase ? 'a' : 'A'; 
			const int lettersOffset = 26; // A...Z or a..z: length = 26  

			for (var i = 0; i < size; i++)  
			{  
			var charPos = (char)_random.Next(offset, offset + lettersOffset);  
			builder.Append(charPos);
			}
			
			return lowerCase ? builder.ToString().ToLower() : builder.ToString();  
		} 

        public static string Generate()
		{
			using SHA256 sha256Hash = SHA256.Create();
			byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(RandomString(10, true)));
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				builder.Append(bytes[i].ToString());
			}
			return builder.ToString().Slice(0, 15);
		}
    }

}