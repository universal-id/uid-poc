using System;
using UniversalIdentity.Library.Cryptography;

namespace UniversalIdentity.Library;

public static class Helper
{
    public static string GetCoreIdentifierFromAddress(this EthKey key)
    {
        var address = key.GetPublicAddress();
        return GetCoreIdentifierFromAddress(address);
    }

    public static string GetCoreIdentifierFromAddress(this string address)
    {
        return $"{CoreIdentifierPrefix}{address}";
    }

    public const string CoreIdentifierPrefix = "did:uid:";
    public const string PublicIdentifierPrefix = "did:eth:";
    public const string KeyIdentifierPrefix = "did:key:eth:";
    public const string ZeroAddress = "0x00000000000000000000000000000000";
    public const string ZeroCoreIdentifier = CoreIdentifierPrefix + ZeroAddress;

    public static string GetPublicIdentifier(this EthKey key)
    {
        var address = key.GetPublicAddress();
        return $"{PublicIdentifierPrefix}{address}";
    }

    public static string GetKeyIdentifier(this EthKey key)
    {
        var address = key.GetPublicAddress();
        return $"{KeyIdentifierPrefix}{address}";
    }

    public static DateTime UnixEpochDateTime = new DateTime(1970, 1, 1);

    public static long ConvertToUnixTime(this DateTime startDateTime, int additionalSeconds = 0)
    {
        if (startDateTime == DateTime.MinValue)
        {
            return 0;
        }

        var targetDateTime = startDateTime.AddSeconds(additionalSeconds);
        var unixTime = (int)(targetDateTime - UnixEpochDateTime).TotalSeconds;
        return unixTime;
    }

    public static DateTime ConvertFromUnixTime(this long unixTime)
    {
        var dateTime = UnixEpochDateTime.AddSeconds(unixTime);
        return dateTime;
    }
}