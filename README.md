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

### SimpleUptime.Application
> Application layer

### Simple.Domain
> Domain layer

### SimpleUptime.FuncApp
> Azure function app that is a rest api and message processor

### SimpleUptime.Infrastructure
> Infrastructure layer

### SimpleUptime.ResourceGroup
> Azure arm templates, infrastructure as code

### SimpleUptime.WebApp
> Single page web app. Uses npm and webpack. Not part of visual studio solution.

### SimpleUptime.WebAppProxy
> Azure function app that proxies SimpleUptime.WebApp resources and proxies rest api calls to SimpleUptime.FuncApp
