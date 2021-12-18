// SPDX-License-Identifier: MIT
pragma solidity >=0.4.21 <0.8.11;

import "./Identity.sol";

contract System {

    constructor() {
        emit SystemCreated(address(this));
    }

    function createIdentity(Identity.ValueLevel _valueLevel) public returns(address identityAddress) {
        Identity newIdentity = new Identity(msg.sender, _valueLevel);
        identityAddress = address(newIdentity);
        identityMap[identityAddress] = true;

        emit IdentityCreated(identityAddress);

        return identityAddress;
    }

    mapping (address => bool) public identityMap;
    function isIdentity(address _identitytAddress) public view returns (bool) {
        return identityMap[_identitytAddress];
    }

    event IdentityCreated(address identityAddress);
    event SystemCreated(address systemAddress);
}
