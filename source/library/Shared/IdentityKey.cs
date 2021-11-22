using System;

namespace UniversalIdentity.Library;

public class IdentityKey
{
    public string Identifier { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Expires { get; set; }
    public ValueLevel Level { get; set; }
    public string PublicKey { get; set; }
}
