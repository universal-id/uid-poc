namespace UniversalIdentity.Library;

public enum ValueLevel
{
    Unspecified = 0,
    None = 1, // aka extremely low - ~$0.0001
    VeryLow = 2, // Order of $0.01
    Low = 3, // Order of $1
    MediumLow = 4, // Order of $100
    Medium = 5, // Order of $10K
    MediumHigh = 6, // Order of $1M
    High = 7, // Order of $100M
    VeryHigh = 8, // Order of $10B
    ExtremelyHigh = 9, // Order of $1T
    UltimatelyHigh = 10 // Order of $100T
}