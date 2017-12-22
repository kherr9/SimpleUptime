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