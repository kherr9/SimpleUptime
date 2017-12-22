# Simple Uptime
> Website uptime monitoring application


## Values
- Simple and lean
- Use Domain Driven Design
- Low hosting cost
- Automated tests, deployments, and alerting

## How to run SimpleUptime.WebApp for local development

- run Azure Storage Emulator
- run Azure CosmosDb Emulator
- run SimpleUptime.FuncApp

```powershell
cd src\SimpleUptime.WebApp

# restore packages 
npm install

# run webapp for development
npm run start

# build webapp
npm run build 
```

## Projects

### Simple.Domain
> Domain layer. Domain model, repository interfaces, and domain service interfaces.

### SimpleUptime.Infrastructure
> Infrastructure layer. Implementation of domain repositories, and domain services.

### SimpleUptime.Application
> Application layer. Application services.

### SimpleUptime.FuncApp
> Azure function app that is a rest api and message processor.

### SimpleUptime.WebAppProxy
> Azure function app that proxies SimpleUptime.WebApp resources and proxies rest api calls to SimpleUptime.FuncApp. The proxy logic could be moved into SimpleUptime.FuncApp, but there's an open bug that requires us to keep it seperate for now.

### SimpleUptime.WebApp
> Single page web app. Uses npm and webpack. Not part of visual studio solution.

### SimpleUptime.ResourceGroup
> Azure arm templates, infrastructure as code.

## Test Projects

### SimpleUptime.UnitTests
> Unit tests for visual studio projects.

### SimpleUptime.IntegrationTests
> Integration tests for visual studio project. Requires azure storage emulator and azure cosmosdb emulator to be running.