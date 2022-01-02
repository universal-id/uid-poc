using UniversalIdentity.Library.Storage;

namespace OfflineClient.Extensions
{
    public static class IdentityStorageExtensions
    {
        public static void DisplayIdentities(this IdBoxStorage idBoxStorage, string summary= "--summary")
        {
            List<IdentityStorage>? identities = idBoxStorage.Identities?.ToList();

            Console.WriteLine($"{identities?.Count} identities, Primary identity:{idBoxStorage.PrimaryIdentity}");

            if (identities != null)
            {
                if (summary== "--summary")
                {
                    int i = 0;
                    foreach (IdentityStorage? identity in identities)
                    {
                        i++;
                        Console.WriteLine($"Identity{i} | Identifier: {identity.Identifier}");
                    }
                }
                else
                {
                    int i = 0;
                    foreach (IdentityStorage identity in identities)
                    {
                        i++;
                        Console.WriteLine($"Identity{i}:");
                        Console.WriteLine($" Identifier: {identity.Identifier}");
                        Console.WriteLine($" Level: {identity.Level}");

                        Console.WriteLine(" Keys:");
                        foreach (KeyStorage key in identity.Keys)
                        {
                            Console.WriteLine($"  Identifier: {key.Identifier}");
                            Console.WriteLine($"  Level: {key.Level}");
                            Console.WriteLine($"  Created: {key.Created}");
                            Console.WriteLine($"  PublicKey: {key.PublicKey}");
                        }
                    }
                }
            }
        }
    }
}
