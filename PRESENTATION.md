# Serverless FP on Azure

## With a little Bit of Haskell
Monadic Party 2018

---

# Notice
Presented material (including this talk) is very much a *Work-in-Progress*.

There is little theory, fancy types, and (surprisingly) Haskell in this talk.

---

PLEASE ASK QUESTIONS DURING THE TALK

---

# About Me

- Programming Languages Theory in Haskell/Pen-and-Paper
- ML/Signal Processing in Python/.NET

---

# Motivation
> We wanted to see how difficult would it be to move to Haskell...
> -- Famous Last Words

---

# In My Case
> I had some serverless workflows running on top of Microsoft Azure.   I wanted to see if I could adopt any of them to running Haskell.


---

# WTF is Serverless?

![Commit Strip Episode About Serverless](https://www.commitstrip.com/wp-content/uploads/2017/04/Strip-Severless-650-finalenglishV2.jpg)

---

## Rough Idea
Spinning-up a container per each request, or other event.

---

## The Promise of Serverless Cloud

---

### Abstracting Away the Servers
There is neither physical nor virtual machine to provision or maintain.

---

### (Almost-)Automatic Scalability
Scaling should be at least as easy as adjusting the number of instances.  Ideally, you should be able to adjust automatically as the need arises.

---

### Pay Per Use
Pay only for the resources used by your workload.  Do not pay for idle time.

---

# Two Flavours of Serverless

---

## Bring Your Own Files (BYOF)
> Here is my script, may you run it every 5 minutes, please?

- Provide your application as a set of files,
- Deploy either as a git repo, or as a zip file.

---

## Bring Your Own Container (BYOC)
> Here is an image for my container, may you run it and keep it alive, please?

- Provide your application as a docker container,
- Deploy via some docker registry.


---

# .NET Core Framework
Cross-platform .NET flavour.

[https://www.microsoft.com/net/download](https://www.microsoft.com/net/download)

---

# Microsoft Azure

---

# Serverless on Azure

- Azure Functions
- Azure Container Instances
- Microsoft Flow
- Azure Logic Apps
- Azure WebJobs

Detailed Comparison: [https://bit.ly/2u3LcZG](https://bit.ly/2u3LcZG)

---

I will focus on:
- Azure Functions, and 
- Azure Container Instances.

---

# Azure Functions
Function as a Service (FaaS).

- Supports BYOF and BYOC
- Windows and Linux instances
- Supports:
    - .NET Languages (C#/**F#**)
    - JavaScript
    - Java
- Consumption Plan (Windows only)

---

## Azure Functions V2

I will be focusing on Azure Functions V2, wich is cross-platform.

- In Preview,
- Stable probably in late 2018,
- Windows and Linux instances,
- BYOF and BYOC (Linux only),
- Dev tools for Windows, Linux, and macOS.

---

# Azure Functions Script Host
Runtime available on the instance

- Provides a set of bindings and triggers,
- Basic logging and monitoring,
- Automatic retry for some failing triggers,
- Automatic back-off for some failing triggers,

Github: [Azure/azure-functions-host](https://github.com/Azure/azure-functions-host)

---

# Azure Functions Core Tools

Run Azure Functions Runtime locally

``` sh
npm i -g azure-functions-core-tools@core --unsafe-perm true
```

``` sh
brew tap azure/functions
brew install azure-functions-core-tools
```

Github: [Azure/azure-functions-core-tools](https://github.com/Azure/azure-functions-core-tools)

---

# Azure Storage

- Azure File Storage
- Azure Blob Storage
- Azure Queue Storage
- Azure Table Storage

---

# Azurite

Cross-platform Azure Storage Emulator

``` sh
npm install -g azurite
```

Github: [Azure/azurite](https://github.com/Azure/azurite)

---

# Exercise 1

- `dotnet-sdk` [https://www.microsoft.com/net/download](https://www.microsoft.com/net/download)
- `azure-functions-core-tools` [https://github.com/Azure/azure-functions-core-tools](https://github.com/Azure/azure-functions-core-tools)
- `azurite` [https://github.com/Azure/Azurite](https://github.com/Azure/Azurite)
- `azure storage explorer` [https://azure.microsoft.com/en-us/features/storage-explorer/](https://azure.microsoft.com/en-us/features/storage-explorer/)

---

# Triggers and Bindings
With some demos in F#.

---

## Timer Trigger
Execute a function every `* * * * * * *` (modified cron expression syntax)

---

## HTTP Trigger
Execute a function on HTTP request

--

## Queue Trigger
Execute a function whenever a message is available on a queue.

- Reply on error (by default 5 times),
- After that, move message to a poison queue.

---

## Queue Binding
Output the result of a function as a message to Azure Storage Queue.

---

## Blob Storage Binding
Output the result of a function as a blob to Azure Blob Storage.

---

# Haskell on Azure Functions

---

# Exercise 2
Let's try to run a simple Haskell program for a HTTP request. 

Code Samples: 
[https://github.com/jakzale/monadic-party-2018-samples](https://github.com/jakzale/monadic-party-2018-samples)

---

# .NET Wrapper + Unix Domain Sockets
Demo

---

# Azure Functions Language Worker

- Preview Support for Python
- Protobuf + gRPC

---

## Azure Container Instances (ACI).
Container as a Service (CaaS).

- Windows and Linux Containers
- Azure Container Registry for hosting images
- Azure File Storage for persisting storage (Linux only)

---



