# UID Proof of Concept

This is the Universal Identity (UID) proof of concept (PoC) project source repository. All development activities on the current UID proof of concept occurs in this public repository.

The first iteration is a complex undertaking, that will result in the first working end-to-end identity system for online and offline scenarios. As a PoC the source is not expected to measure up to the highest engineering standards, it is only meant to prove feasibility. As such, we are also not selecting the most common, modern and proven technology platform to implement this, instead we are using .NET Core 6 as it makes our development process more cost effective, especially since one of the founders brings some expertise.

## Components

* **UID library** - General purpose libraries, containing key UID functionality, for all layers of the stack, to be used in all modalities including on-device app and back end services.
* **CLI client** - Offline identity for peer-to-peer (p2p) operation.
* **Identity client** - Online identity synced and backed up by identity operator service and used to interact with other apps like Contacts app.
* **Contacts app** - Online identity app showcasing login using identity app, as well as delegated permissions for managing contacts.
* **Identity operator service** - Backing service for identity app, used to backup and sync identity information as well as used to interchange connection information with another operator instance or offline identity.
* **Contacts app service** - Backing service for contacts app, which uses the same identity operator service in this specific prototype as its own backing identity service.
