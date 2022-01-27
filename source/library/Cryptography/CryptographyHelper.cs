using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using Nethereum.Signer.Crypto;
using Nethereum.Util;

namespace UniversalIdentity.Library.Cryptography
{
    public static class CryptographyHelper
    {
        public static byte[] GetHashBytes(this string data)
        {
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
            Console.WriteLine($"UTF8 bytes: {dataBytes.ToHex(true)}");
            byte[] hashBytes = dataBytes.GetHashBytes();
            return hashBytes;
        }

        public static string GetHashString(this string data)
        {
            byte[] hashBytes = GetHashBytes(data);
            string hashString = hashBytes.ToHex(true);
            Console.WriteLine($"Hash string: {hashString}");
            return hashString;
        }

        public static string GetHashAddressFormat(this string data)
        {
            string hashString = GetHashString(data);
            string hashAddressFormat = hashString.Substring(0,42);
            Console.WriteLine($"Hash string address format: {hashAddressFormat}");
            return hashAddressFormat;
        }

        public static byte[] GetHashBytes(this byte[] data)
        {
            byte[] hash = Sha3Keccack.Current.CalculateHash(data);
            return hash;
        }
    }
}