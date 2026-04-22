# Reenbit Chat App

A **real-time chat application** built with **ASP.NET Core + Angular**, following **Clean Architecture** principles and enhanced with **AI-powered sentiment analysis**.

---

## Features

* User authentication (JWT)
* Real-time messaging via SignalR
* Sentiment analysis for messages (Positive / Neutral / Negative)
* Chat rooms with multiple users
* Clean Architecture (Domain → Application → Infrastructure → Presentation)
* Angular SPA frontend integrated into ASP.NET Core backend

---

## Architecture

This project follows **Clean Architecture**:

```
Presentation (API + SignalR + Angular)
        ↓
Infrastructure (EF Core, Services)
        ↓
Application (Use Cases, DTOs, Interfaces)
        ↓
Domain (Entities, Business Logic)
```

### Key Principles

* Dependencies point **inward only**
* Domain & Application layers are **framework-independent**
* Separation of concerns across layers
* Fully testable business logic

---

## Project Structure

```
ChatApp/
│
├── ChatApp.Domain/            # Core business logic
│   ├── Entities/              # User, Message, ChatRoom
│   ├── Enums/                 # SentimentType
│   └── Exceptions/
│
├── ChatApp.Application/       # Use cases & interfaces
│   ├── DTOs/
│   ├── Interfaces/
│   ├── UseCases/
│   ├── Validators/
│   └── Mappings/
│
├── ChatApp.Infrastructure/    # External implementations
│   ├── Data/                  # DbContext
│   ├── Repositories/
│   ├── Services/              # JWT, hashing, sentiment
│   └── Migrations/
│
├── ChatApp.Presentation/      # API + frontend host
│   ├── Controllers/
│   ├── Hubs/                  # SignalR
│   ├── Middleware/
│   ├── Services/
│   └── ClientApp/             # Angular app
│
├── ChatApp.sln
```

---

## Tech Stack

### Backend

* ASP.NET Core
* Entity Framework Core
* SQL Server
* SignalR
* MediatR (CQRS)
* FluentValidation
* AutoMapper
* JWT Authentication

### Frontend

* Angular
* RxJS
* SignalR client
* HTTP Interceptors

### Other

* Azure Cognitive Services (Sentiment Analysis)
* Serilog (logging)

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/your-username/reenbit-chat-app.git
cd reenbit-chat-app
```

---

### 2. Run Backend

```bash
dotnet restore
dotnet ef database update
dotnet run --project ChatApp.Presentation
```

Backend runs on:

```
https://localhost:5001
```

---

### 3. Run Frontend

```bash
cd ChatApp.Presentation/ClientApp
npm install
ng serve
```

Frontend runs on:

```
http://localhost:4200
```

---

### Dev Proxy

Angular uses `proxy.conf.json` to forward API calls to backend, so no CORS issues during development.

---

## How Sentiment Analysis Works

1. User sends a message
2. Message is saved & broadcast via SignalR
3. Sentiment analysis runs asynchronously
4. Result is sent back and displayed in UI

---

## Authentication

* JWT-based authentication
* Login / Register endpoints
* Token stored and attached via Angular interceptor

---

## Real-Time Communication

* SignalR Hub: `ChatHub`
* Events:

  * Receive messages
  * Receive sentiment updates
* Users are grouped by chat rooms

---

## Validation & Error Handling

* FluentValidation for request validation
* Custom exceptions in Application & Domain layers
* Global exception middleware

---

## Build & Deployment

### Production Build

```bash
cd ClientApp
ng build --configuration production
```

Then:

```bash
dotnet publish -c Release
```

Angular app is served from:

```
wwwroot/
```

---

## Author

**Iryna Luchyn**


This project is for educational purposes.
