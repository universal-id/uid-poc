"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const hardhat_1 = require("hardhat");
describe("System", function () {
    it("Should ensure System is deployable", async function () {
        const System = await hardhat_1.ethers.getContractFactory("System");
        const Identity = await hardhat_1.ethers.getContractFactory("Identity");
        const system = await System.deploy();
        await system.deployed();
        //expect(await greeter.greet()).to.equal("Hello, world!");
        //const setGreetingTx = await greeter.setGreeting("Hola, mundo!");
        //// wait until the transaction is mined
        //await setGreetingTx.wait();
        //expect(await greeter.greet()).to.equal("Hola, mundo!");
    });
});
