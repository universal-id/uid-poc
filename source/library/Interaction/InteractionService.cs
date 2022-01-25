using UniversalIdentity.Library.Cryptography;
using UniversalIdentity.Library.Storage;
using UniversalIdentity.Library;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Linq;

namespace UniversalIdentity.Library.Interaction;

public class InteractionService
{
    public InteractionService(string path)
    {
        this.Path = path;
    }

    public string Path { get; set; }

    public const string KeysFolder = ".keys";

    public void Init()
    {
        if (!Directory.Exists(this.Path)) throw new Exception($"Expected interaction path '{this.Path}' to exist at init time.");

            var keysPath = System.IO.Path.Combine(this.Path, KeysFolder);
            if (!Directory.Exists(keysPath)) Directory.CreateDirectory(keysPath);
    }

    public bool IsInitialized()
    {
        if (!Directory.Exists(this.Path)) return false;

        var keysPath = System.IO.Path.Combine(this.Path, KeysFolder);
        return Directory.Exists(keysPath);
    }

    public EthKey CreateAndStoreNewKey()
    {
        var newKey = new EthKey();
        JObject keyJson = new()
        {
            ["identifier"] = newKey.GetIdentifier(),
            ["privateKey"] = newKey.GetPrivateKey()
        };

        var keysPath = System.IO.Path.Combine(this.Path, KeysFolder);
        var keyPath = System.IO.Path.Combine(keysPath, $"{newKey.GetIdentifier().Replace(":", "")}.json");
        var keyJsonString = keyJson.ToString();
        File.WriteAllText(keyPath, keyJsonString);

        var publicKey = new EthKey(newKey.GetPublicKey());
        return publicKey;
    }

    public EthKey GetStoredPrivateKey(string keyIdentifier)
    {
        var keysPath = System.IO.Path.Combine(this.Path, KeysFolder);
        var keyPath = System.IO.Path.Combine(keysPath, $"{keyIdentifier.Replace(":", "")}.json");
        var fileContents = File.ReadAllText(keyPath);
        var keyJson = JObject.Parse(fileContents);
        var privateKey = (string)keyJson["privateKey"];
        var storedKey = EthKey.CreateFromPrivateKey(privateKey);
        return storedKey;
    }

    public EthKey GetStoredKey(string keyIdentifier)
    {
        var storedPrivateKey = GetStoredPrivateKey(keyIdentifier);
        var storedPublicKey = new EthKey(storedPrivateKey.GetPublicKey());
        return storedPublicKey;
    }

    public IEnumerable<string> GetStoredKeyIdentifiers()
    {
        var keysPath = System.IO.Path.Combine(this.Path, KeysFolder);
        foreach (var keyFileName in Directory.EnumerateFiles(keysPath))
        {
            var fileContents = File.ReadAllText(keyFileName);
            var keyJson = JObject.Parse(fileContents);
            var identifier = (string)keyJson["identifier"];
            yield return identifier;
        }
        // var keyFileNames = Directory.EnumerateFiles(keysPath)
        //     .Select(fileName => System.IO.Path.GetFileName(fileName));
        // var keyIdentifiers = keyFileNames
        //     .Where(fileName => fileName.LastIndexOf(".json") != -1)
        //     .Select(fileName => fileName.Remove(fileName.LastIndexOf(".json")));
        // return keyIdentifiers;
    }

    public bool IsKeyStored(string keyIdentifier)
    {
        var keysPath = System.IO.Path.Combine(this.Path, KeysFolder);
        var keyPath = System.IO.Path.Combine(keysPath, $"{keyIdentifier.Replace(":", "")}.json");
        if (!File.Exists(keyPath)) return false;
        var fileContents = File.ReadAllText(keyPath);
        var keyJson = JObject.Parse(fileContents);
        var privateKey = (string)keyJson["privateKey"];
        var storedKeyIdentifier = (string)keyJson["identifier"];
        return keyIdentifier.Equals(storedKeyIdentifier, StringComparison.OrdinalIgnoreCase);
    }

    public byte[] SignData(string keyIdentifier, byte[] data)
    {
        throw new NotImplementedException();
    }

    public async Task<byte[]> SignDataAsync(string keyIdentifier, byte[] data)
    {
        throw new NotImplementedException();
    }

    public void SignDataRequest(string keyIdentifier, byte[] data)
    {
        throw new NotImplementedException();
    }

    //public event SignDataRequested()

    public Func<byte[], string, byte[]> SignRequestHandler { get; set; }

    //public event EventHandler<SignRequestEventArgs> SignDataRequested;

}

// public class SignRequestEventArgs : EventArgs
// {
//     public SignRequestEventArgs(string keyIdentifier) {}

//     public byte[] Data { get; set; }
//     public string keyIdentifier { get; set; }
// }