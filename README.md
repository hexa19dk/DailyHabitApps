## 🧠 Atomic Habits – Habit Tracking & Analytics Platform

A full-stack habit tracking application that helps users build consistency through daily habit tracking, calendar-based logging, and visual analytics.
Built with React (Material UI) on the frontend and ASP.NET Core Web API (.NET 8) on the backend.

## 🖥️ Tech Stack
### 🖼️ Frontend (Client UI)
| Category           | Technology                |
| ------------------ | ------------------------- |
| Language           | TypeScript / JavaScript   |
| Framework          | React                     |
| UI Library         | Material UI (MUI)         |
| Charting           | ApexCharts                |
| Routing            | React Router              |
| State Management   | React Hooks               |
| Styling            | CSS, MUI Theme            |
| Environment Config | `.env`, `.env.production` |
| Build Tool         | Vite / CRA                |

### 🧠 Backend (Server API)
| Category         | Technology                 |
| ---------------- | -------------------------- |
| Language         | C#                         |
| Framework        | ASP.NET Core Web API       |
| Runtime          | .NET 8                     |
| Architecture     | Controller–Service–UseCase |
| API Style        | REST                       |
| Validation       | Custom Validators          |
| Mapping          | DTO ↔ Domain Mappers       |
| Configuration    | `appsettings.json`         |
| Containerization | Docker                     |
| CI/CD            | Jenkins                    |

### 🖥 Client (Frontend)
	client-ui/
	├── public/
	├── src/
	│   ├── assets/           # Images, icons, static assets
	│   ├── components/       # Reusable UI components
	│   ├── layouts/          # App layout wrappers
	│   ├── routes/           # Public & protected routes
	│   ├── services/         # API service calls
	│   ├── utils/            # Helper functions
	│   ├── views/
	│   │   ├── dashboard/    # Dashboard pages & widgets
	│   │   ├── habit/        # Habit management & tracking
	│   │   ├── stats/        # Statistics & analytics
	│   │   └── settings/     # User & app settings
	│   ├── App.tsx
	│   └── main.tsx
	├── .env
	├── .env.production
	└── package.json
	
### 🧠 Server (Backend)
	Talenta-Receiver-API-Gateway/
	├── Config/              # Application configuration setup
	├── Mappers/             # DTO ↔ Domain model mappings
	├── Models/              # Domain / data models
	├── Repositories/        # Data access layer
	├── Services/            # Business logic services
	├── UseCases/            # Application use-case orchestrators
	├── Validators/          # Input & request validation
	├── Utils/               # Helpers & shared utilities
	├── Protos/              # Protobufs (gRPC / integration events)
	├── Dockerfile           # Container build definition
	├── Jenkinsfile          # CI/CD pipeline configuration
	├── Program.cs           # Application entry point
	└── Startup.cs           # Middleware, DI, routing


## ✨ Key Features
### 🔐 Authentication & Access Control
  - User registration and login
  - Secure authenticated routes
  - Public and protected page separation
  - Environment-based configuration for different deployments

### 📊 Dashboard
  - Overview dashboard with summary cards (KPIs)
  - Habit completion metrics
  - Visual analytics using interactive charts
  - Graceful error handling and fallback UI

### ✅ Habit Management
  - Clear visual representation of active habits
  - Habit action menus and dialogs
  - Configure habit schedules and preferred times
  - Create, update, and delete habits

### 📅 Habit Tracking
  - Calendar-based habit tracking
  - Daily habit completion logging
  - Interactive dialogs for marking habit progress
  - Visual feedback for completed and missed habits

### 📈 Statistics & Analytics
  - Data optimized for chart-based insights
  - Time-based statistics (daily/monthly)
  - Distribution and trend visualization
  - Habit completion rate analysis

### 🎨 UI & UX
  - Error pages (404, fallback states)
  - Reusable and modular UI components
  - Material UI design system
  - Responsive dashboard layout

## 🧩 System Architecture
### Frontend (Client UI)
- Framework: React
- UI Library: Material UI
- Charts: ApexCharts
- State Management: React hooks & component state
- Routing: Protected and public routes
- Environment Config: .env, .env.production
### Backend (Server API)
- Framework: ASP.NET Core Web API (.NET 8)
- Architecture: Controller–Service–DTO pattern
- Configuration: appsettings.json
- API Style: RESTful endpoints
- Responsibility:
	- Habit CRUD operations
	- Habit tracking persistence
	- Aggregated statistics for analytics
	- Secure API access
