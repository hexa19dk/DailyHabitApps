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
	
### 🧠 Backend (Daily Habit Tracker API)
	server/
	├── Controllers/              # HTTP API controllers
	│   ├── AuthController.cs     # Authentication endpoints
	│   ├── HabitController.cs    # Habit CRUD & tracking endpoints
	│   └── StatsController.cs    # Statistics & analytics endpoints
	│
	├── DTOs/                     # Request / response data contracts
	│   ├── Auth/
	│   ├── Habit/
	│   └── Stats/
	│
	├── Models/                   # Domain & data models
	│   ├── User.cs
	│   ├── Habit.cs
	│   ├── HabitLog.cs
	│   └── HabitSchedule.cs
	│
	├── Services/                 # Business logic layer
	│   ├── AuthService.cs
	│   ├── HabitService.cs
	│   └── StatsService.cs
	│
	├── Repositories/             # Data access layer
	│   ├── Interfaces/
	│   └── Implementations/
	│
	├── Validators/               # Input & request validation
	│   ├── HabitValidator.cs
	│   └── AuthValidator.cs
	│
	├── Mappers/                  # DTO ↔ Domain model mapping
	│   └── HabitMapper.cs
	│
	├── Utils/                    # Helper & utility classes
	│   ├── DateTimeHelper.cs
	│   └── ResponseHelper.cs
	│
	├── Data/                     # Database context & migrations
	│   ├── ApplicationDbContext.cs
	│   └── Migrations/
	│
	├── Config/                   # Application & environment configuration
	│   ├── JwtOptions.cs
	│   └── AppSettings.cs
	│
	├── Program.cs                # Application entry point
	├── Startup.cs                # Middleware, DI, routing
	├── appsettings.json          # Base configuration
	├── appsettings.Development.json
	├── Dockerfile                # Container build definition
	└── README.md


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
