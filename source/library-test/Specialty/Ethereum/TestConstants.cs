using Microsoft.Extensions.Configuration;

public static partial class TestConstants
{
    public static IConfigurationRoot Configuration
    {
        get
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .AddUserSecrets(typeof(TestConstants).Assembly);
            IConfigurationRoot configuration = configBuilder.Build();
            return configuration;
        }
    }
    public static string TestMnemonic
    {
        get
        {
            if (testMnemonic == null)
            {
                testMnemonic = Configuration["TestMnemonic"];
            }

            return testMnemonic!;
        }
    }
    private static string? testMnemonic;

    public static string TestInfuraProjectId
    {
        get
        {
            if (testInfuraProjectId == null)
            {
                testInfuraProjectId = Configuration["TestInfuraProjectId"];
            }

            return testInfuraProjectId!;
        }
    }
    private static string? testInfuraProjectId;
    public const string TestSystemContractAddress = "0x7fd24e5dc5a96852d617f5c412d62f637481ba2f";
    public const string TestIdentityContractAddress = "0xbcd71806179845c9c5cc8c42cde33c241e706c23";    
    public const string TestNetName = "rinkeby";

    public const string ZeroAddress = "0x0000000000000000000000000000000000000000";

    public const string TestOperatorIdentifier = "0xabcdee5dc5a96852d617f5c412d62f63748edcba";

    public const string TestServiceIdentifier = "0xedcbae5dc5a96852d617f5c412d62f63748abcde";

    public static string GetInfuraUrl()
    {
        return $"https://{TestNetName}.infura.io/v3/{TestConstants.TestInfuraProjectId}";
    }      
}