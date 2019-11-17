# Secure Service Toolbox (SST)
Secure Service Toolbox, or SST, is an easy to use toolbox for defining services and their clients. It makes security its top priority along with convenience of use.

To achieve security the toolbox uses end-to-end strong encryption. Also, communication with the SST servers is done though capabilities. A client requests a capability and the provider may grant it. The client, once in possession of a capability can invoke it.

To start using the toolbox you need to:

- create a capability definition
- implement the capability
- register the capability on a server (and optionally define rules for granting the capability)
- get a capability on a client (using the capability definition)
- invoke the capability

The toolbox also provides the Capability Registry Service which is used by the servers to register the capabilities they provide and by the clients to discover the provided capabilities.

There can be multiple Capability Registry Services configured to form a tree-like hierarchy. looking up a service can be done through the whole or only a part of this tree, depending on configuration.

The client gets a capability from the Capability Registry Server (CRS). If such a capability is registered, the CRS sends the endpoint (port number and IP address) information to the client which the client then uses to get the capability from the provider. The connection between the client and the server is one-to-one ideally, but if the client cannot reach the server there is also a Public Relay Server (PRS) which can be used as an intermediary between the client and the server.

The provider can grant the client the requested capability. If so, the client can invoke the capability with defined argument. The server evaluates the argument according to the user-defined business logic and returns a defined return value.

There are several implementations of the toolbox in following languages:

- C
- C++
- C# (.NET Core)
- Go
- Haskell
- Perl

Underlying communication is supported by ZeroMQ (NetMQ for C# implementation).

Other languages may be supported at a later time.

Contributions and suggestions are welcome.