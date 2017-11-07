# Serverless

The next-generation of application development

---

### Evolution of application development

- Physical Hardware: oldskool
- Virtual Machine: abstraction of hardware layer
- Container: abstraction of the OS layer
- Serverless: abstraction of the middleware stack

Every evolution adds increased flexibility, scalability & ease-of-use!

---

### Is it realy serverless?

No, but the main concept is that you only need to worry about your code, and don't need to bother with configuring servers or middleware

---

### Why go serverless?

- Focus on business code = faster delivery of fixes and features
- Micro-billing: only pay when your app is being consumed
- Auto-scaling: your code is scaled automatically depending on demand

---

### What is the serverless offering in Azure?

- Azure Functions: write custom code in multiple languages (C#, Powershell, PHP, Node.JS,...)
- LogicApps: Visual Business Flow Modelling (think System Center Orchestrator meets 2017)
- EventGrid: cloud-based event platform based on publisher/listener mechanism
- CosmosDB/Azure Storage: store records and/or state that is used by your code

- Various tools can be glued together by using input & output bindings

---

### I want serverless! How do I get it implemented for my apps?

- Existing apps need to be refactored to work as a micro-service
- Middle ground = using containers as intermediate solution
- Greenfield? Go for it!

---

### The new application delivery scenario

- Expose HTTP endpoints with Azure Functions
- Use multiple clients (Bot, WebSite, PowerShell module, Mobile) to access the endpoints
- One backend, multiple frontends = optimal flexibility!

---

### DEMO TIME

---

### Thanks for your attention!

Questions?