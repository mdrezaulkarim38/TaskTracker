# TaskTracker

TaskTracker is a robust task management web application built with ASP.NET Core MVC. It provides a secure, feature-rich platform for users to create, manage, and track their daily tasks efficiently with real-time updates and comprehensive filtering capabilities. 

---

## Features

### Authentication & Security

* User registration and login with ASP.NET Core Identity
* Password validation (minimum 6 characters, requires uppercase, lowercase, and digit)
* Account lockout after 5 failed attempts (5-minute duration)
* Email confirmation support
* Secure cookie authentication with sliding expiration (60 minutes)
* Role-based authorization ready

### Task Management

* Create, edit, and delete tasks
* Toggle task completion status with instant AJAX updates
* Mark tasks as complete/pending with real-time UI feedback
* Server-side pagination (5 tasks per page)
* Task details:

  * Title and description
  * Due date
  * Priority levels
  * Creation timestamp
  * Completion status

### Advanced Features

* **Search**: Real-time AJAX search
* **Filter**: Pending/Completed
* **Sort**: Due date or priority
* **Export**: CSV (UTF-8 BOM)
* Responsive table with partial updates
* Toast notifications for CRUD actions

### User Experience

* SweetAlert2 confirmations
* Bootstrap 5 UI
* Loading states for AJAX
* Form validation (client & server)
* Clean navigation

---

## Tech Stack

| Technology                 | Purpose       |
| -------------------------- | ------------- |
| ASP.NET Core MVC (.NET 10) | Web framework |
| Entity Framework Core      | ORM           |
| SQL Server                 | Database      |
| ASP.NET Core Identity      | Auth          |
| Bootstrap 5                | UI            |
| jQuery                     | AJAX          |
| SweetAlert2                | Alerts        |
| Font Awesome               | Icons         |

---

## Project Structure

```
TaskTracker/
├── Controllers/
│   ├── AccountController.cs
│   ├── TaskController.cs
│   └── HomeController.cs
├── Services/
├── Repositories/
├── Models/
├── Data/
├── Views/
├── wwwroot/
└── Migrations/
```

---

## Setup Instructions

### Prerequisites

* .NET 10 SDK
* SQL Server
* Git

### Clone

```bash
git clone https://github.com/mdrezaulkarim38/TaskTracker.git
cd TaskTracker
```

### Configure Database

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskTrackerDb;Trusted_Connection=True;"
  }
}
```

### Run Migrations

```bash
dotnet ef database update
```

### Run App

```bash
dotnet run
```

Visit:

```
https://localhost:5001
```

---

## API Endpoints

| Endpoint                | Method   | Description |
| ----------------------- | -------- | ----------- |
| /Account/Register       | GET/POST | Register    |
| /Account/Login          | GET/POST | Login       |
| /Account/Logout         | POST     | Logout      |
| /Task                   | GET      | Task list   |
| /Task/Create            | GET/POST | Create      |
| /Task/Edit/{id}         | GET/POST | Edit        |
| /Task/Delete/{id}       | POST     | Delete      |
| /Task/ToggleStatus/{id} | POST     | Toggle      |
| /Task/Search            | GET      | Search      |
| /Task/ExportCsv         | GET      | Export      |

---

## Security Features

* CSRF protection
* Password complexity rules
* Account lockout
* Cookie authentication
* HTTPS enforcement
* Secure hashing

---

## Future Improvements

* SignalR real-time updates
* Dashboard & analytics
* Calendar view
* Email reminders
* Tags & subtasks
* Drag-and-drop tasks
* Dark mode
* Mobile PWA

---

## License

MIT License

---

## Author

**Md Rezaul Karim**
GitHub: @mdrezaulkarim38
