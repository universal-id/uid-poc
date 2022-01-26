public static partial class TestConstants
{
    public const string IdentityContractAbi = @"[
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""_owner"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""enum Identity.ValueLevel"",
          ""name"": ""_valueLevel"",
          ""type"": ""uint8""
        }
      ],
      ""payable"": false,
      ""stateMutability"": ""nonpayable"",
      ""type"": ""constructor""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": false,
          ""internalType"": ""address"",
          ""name"": ""agentKeyAddress"",
          ""type"": ""address""
        }
      ],
      ""name"": ""AgentKeyAdded"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": false,
          ""internalType"": ""address"",
          ""name"": ""agentKeyAddress"",
          ""type"": ""address""
        },
        {
          ""indexed"": false,
          ""internalType"": ""enum Identity.AgentKeyState"",
          ""name"": ""beforeState"",
          ""type"": ""uint8""
        },
        {
          ""indexed"": false,
          ""internalType"": ""enum Identity.AgentKeyState"",
          ""name"": ""afterState"",
          ""type"": ""uint8""
        }
      ],
      ""name"": ""AgentKeyStateChanged"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""nameHash"",
          ""type"": ""uint256""
        },
        {
          ""indexed"": false,
          ""internalType"": ""string"",
          ""name"": ""value"",
          ""type"": ""string""
        }
      ],
      ""name"": ""PropertySet"",
      ""type"": ""event""
    },
    {
      ""constant"": true,
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": """",
          ""type"": ""address""
        }
      ],
      ""name"": ""agentKeyMap"",
      ""outputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""keyAddress"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""startTime"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""expiryTime"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""enum Identity.ValueLevel"",
          ""name"": ""valueLevel"",
          ""type"": ""uint8""
        },
        {
          ""internalType"": ""enum Identity.AgentKeyState"",
          ""name"": ""state"",
          ""type"": ""uint8""
        }
      ],
      ""payable"": false,
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""constant"": true,
      ""inputs"": [],
      ""name"": ""owner"",
      ""outputs"": [
        {
          ""internalType"": ""address"",
          ""name"": """",
          ""type"": ""address""
        }
      ],
      ""payable"": false,
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""constant"": true,
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""propertiesMap"",
      ""outputs"": [
        {
          ""internalType"": ""string"",
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""payable"": false,
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""constant"": true,
      ""inputs"": [],
      ""name"": ""system"",
      ""outputs"": [
        {
          ""internalType"": ""address"",
          ""name"": """",
          ""type"": ""address""
        }
      ],
      ""payable"": false,
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""constant"": true,
      ""inputs"": [],
      ""name"": ""valueLevel"",
      ""outputs"": [
        {
          ""internalType"": ""enum Identity.ValueLevel"",
          ""name"": """",
          ""type"": ""uint8""
        }
      ],
      ""payable"": false,
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""constant"": false,
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""_nameHash1"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""_nameHash2"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""_nameHash3"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""_nameHash4"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""_nameHash5"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""string"",
          ""name"": ""_value1"",
          ""type"": ""string""
        },
        {
          ""internalType"": ""string"",
          ""name"": ""_value2"",
          ""type"": ""string""
        },
        {
          ""internalType"": ""string"",
          ""name"": ""_value3"",
          ""type"": ""string""
        },
        {
          ""internalType"": ""string"",
          ""name"": ""_value4"",
          ""type"": ""string""
        },
        {
          ""internalType"": ""string"",
          ""name"": ""_value5"",
          ""type"": ""string""
        }
      ],
      ""name"": ""setProperties"",
      ""outputs"": [],
      ""payable"": false,
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""constant"": false,
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""_keyAddress"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""_startTime"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""_expiryTime"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""enum Identity.ValueLevel"",
          ""name"": ""_valueLevel"",
          ""type"": ""uint8""
        },
        {
          ""internalType"": ""enum Identity.AgentKeyState"",
          ""name"": ""_state"",
          ""type"": ""uint8""
        }
      ],
      ""name"": ""addAgentKey"",
      ""outputs"": [],
      ""payable"": false,
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""constant"": false,
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""_keyAddress"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""enum Identity.AgentKeyState"",
          ""name"": ""_keyState"",
          ""type"": ""uint8""
        }
      ],
      ""name"": ""changeAgentKeyState"",
      ""outputs"": [],
      ""payable"": false,
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    }
  ]";
}