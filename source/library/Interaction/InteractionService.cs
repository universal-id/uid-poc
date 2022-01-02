using UniversalIdentity.Library.Cryptography;
using UniversalIdentity.Library.Storage;
using UniversalIdentity.Library;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

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
        var keyPath = System.IO.Path.Combine(keysPath, $"{newKey.GetIdentifier()}.json");
        var keyJsonString = keyJson.ToString();
        File.WriteAllText(keyPath, keyJsonString);

        var publicKey = new EthKey(newKey.GetPublicKey());
        return publicKey;
    }

    private EthKey GetStoredPrivateKey(string keyIdentifier)
    {
        var keysPath = System.IO.Path.Combine(this.Path, KeysFolder);
        var keyPath = System.IO.Path.Combine(keysPath, keyIdentifier);
        var fileContents = File.ReadAllText(keyPath);
        var keyJson = JObject.Parse(fileContents);
        var privateKey = (string)keyJson["privateKey"];
        var storedKey = EthKey.CreateFromPrivateKey(privateKey);
        return storedKey;
    }

    private EthKey GetStoredKey(string keyIdentifier)
    {
        var storedPrivateKey = GetStoredPrivateKey(keyIdentifier);
        var storedPublicKey = new EthKey(storedPrivateKey.GetPublicKey());
        return storedPublicKey;
    }

    public IEnumerable<string> GetStoredKeyIdentifiers()
    {
        throw new NotImplementedException();
    }

    public bool IsKeyStored(string keyIdentifier)
    {
        throw new NotImplementedException();
    }

    public byte[] SignData(string keyIdentifier, byte[] data)
    {
        throw new NotImplementedException();
    }
}