// SPDX-License-Identifier: MIT
pragma solidity >=0.4.21 <0.8.11;

contract Identity {
    address public owner;
    address public network;
    ValueLevel public valueLevel;

    constructor(address _owner, ValueLevel _valueLevel) {
        owner = _owner;
        network = msg.sender;
        valueLevel = _valueLevel;
    }

    enum ValueLevel {
        Unspecified,
        None,
        VeryLow,
        Low,
        MediumLow,
        Medium,
        MediumHigh,
        High,
        VeryHigh,
        ExtremelyHigh
    }

    mapping (uint256 => string) public propertiesMap;
    function setProperties(
        uint256 _nameHash1,
        uint256 _nameHash2,
        uint256 _nameHash3,
        uint256 _nameHash4,
        uint256 _nameHash5,
        string memory _value1,
        string memory _value2,
        string memory _value3,
        string memory _value4,
        string memory _value5
        ) public {
        require(owner == msg.sender, "sender should be owner");
        if(_nameHash1 != 0) {
            propertiesMap[_nameHash1] = _value1;
            emit InfoSet(_nameHash1, _value1);
        }
        if(_nameHash2 != 0) {
            propertiesMap[_nameHash2] = _value2;
            emit InfoSet(_nameHash2, _value2);
        }
        if(_nameHash3 != 0) {
            propertiesMap[_nameHash3] = _value3;
            emit InfoSet(_nameHash3, _value3);
        }
        if(_nameHash4 != 0) {
            propertiesMap[_nameHash4] = _value4;
            emit InfoSet(_nameHash4, _value4);

        }
        if(_nameHash5 != 0) {
            propertiesMap[_nameHash5] = _value5;
            emit InfoSet(_nameHash5, _value5);
        }
    }

    enum KeyState {Unspecified, Active, Frozen, Terminated}
    mapping(address => Key) public keyMap;
    struct Key {
        address keyAddress;
        uint256 startTime;
        uint256 expiryTime;
        ValueLevel valueLevel;
        KeyState state;
    }

    function addKey(
        address _keyAddress,
        uint256 _startTime,
        uint256 _expiryTime,
        ValueLevel _valueLevel,
        KeyState _state
    ) public {
        require(owner == msg.sender, "sender should be owner");
        require(
            keyMap[_keyAddress].keyAddress == address(0),
            "_key.keyAddress should not already exist"
        );

        keyMap[_keyAddress].keyAddress = _keyAddress;
        keyMap[_keyAddress].startTime = _startTime;
        keyMap[_keyAddress].expiryTime = _expiryTime;
        keyMap[_keyAddress].valueLevel = _valueLevel;
        keyMap[_keyAddress].state = _state;

        emit KeyAdded(_keyAddress);
    }

    function changeKeyState(address _keyAddress, KeyState _keyState)
        public
    {
        require(owner == msg.sender, "sender should be owner");
        require(
            keyMap[_keyAddress].keyAddress != address(0),
            "_keyAddress sholuld be a valid key"
        );

        KeyState beforeState = keyMap[_keyAddress].state;
        keyMap[_keyAddress].state = _keyState;

        emit KeyStateChanged(_keyAddress, beforeState, _keyState);
    }

    event KeyAdded(address agentKeyAddress);
    event KeyStateChanged(
        address keyAddress,
        KeyState beforeState,
        KeyState afterState
    );
    event InfoSet(uint256 nameHash, string value);
}
