using System;
using System.Security.Cryptography;

namespace someWorld
{
	public static class PasswordSecurity
	{
		public static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
		{
			HashAlgorithm algorithm = new SHA256Managed();

			byte[] plainTextWithSaltBytes = 
				new byte[plainText.Length + salt.Length];

			for (int i = 0; i < plainText.Length; i++)
			{
				plainTextWithSaltBytes[i] = plainText[i];
			}
			for (int i = 0; i < salt.Length; i++)
			{
				plainTextWithSaltBytes[plainText.Length + i] = salt[i];
			}

			return algorithm.ComputeHash(plainTextWithSaltBytes);            
		}

		public static bool CompareByteArrays(byte[] array1, byte[] array2)
		{
			if (array1.Length != array2.Length)
			{
				return false;
			}

			for (int i = 0; i < array1.Length; i++)
			{
				if (array1[i] != array2[i])
				{
					return false;
				}
			}

			return true;
		}

		public static byte[] GenerateRandomSalt()
		{
			var cryptoprovider = new RNGCryptoServiceProvider();

			byte[] salt = new byte[128];
			cryptoprovider.GetBytes(salt);

			return salt;
		}
	}
}

