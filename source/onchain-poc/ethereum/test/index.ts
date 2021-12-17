import { expect } from "chai";
import { ethers } from "hardhat";

describe("Network", function () {
  it("Should ensure Network is deployable", async function () {
    const Network = await ethers.getContractFactory("Network");
    const Identity = await ethers.getContractFactory("Identity");
    
    const network = await Network.deploy();
    await network.deployed();

    //expect(await greeter.greet()).to.equal("Hello, world!");

    //const setGreetingTx = await greeter.setGreeting("Hola, mundo!");

    //// wait until the transaction is mined
    //await setGreetingTx.wait();

    //expect(await greeter.greet()).to.equal("Hola, mundo!");
  });
});
