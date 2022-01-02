import { expect } from "chai";
import { ethers } from "hardhat";

describe("System", function () {
  it("Should ensure System is deployable", async function () {
    const System = await ethers.getContractFactory("System");
    const Identity = await ethers.getContractFactory("Identity");
    
    const system = await System.deploy();
    await system.deployed();

    //expect(await greeter.greet()).to.equal("Hello, world!");

    //const setGreetingTx = await greeter.setGreeting("Hola, mundo!");

    //// wait until the transaction is mined
    //await setGreetingTx.wait();

    //expect(await greeter.greet()).to.equal("Hola, mundo!");
  });
});
