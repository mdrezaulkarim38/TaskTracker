# TaskTracker

TaskTracker is a simple task management web application built with ASP.NET Core MVC. It allows users to create, manage, and track their tasks efficiently.

## Features

- User authentication (Register / Login)
- Create, edit, and delete tasks
- Search tasks using AJAX
- Filter tasks by status (Pending / Completed)
- Sort tasks by due date or priority
- Toggle task completion status
- Server-side pagination
- Export tasks to CSV
- SweetAlert notifications for better user experience
- Responsive UI using Bootstrap

## Tech Stack

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core
- SQL Server
- Bootstrap 5
- jQuery (AJAX)
- SweetAlert2

## Setup Instructions

### 1. Clone the repository

```bash
git clone https://github.com/mdrezaulkarim38/TaskTracker.git
cd TaskTracker
```

### 2. Configure the database

Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=TaskTrackerDb;Trusted_Connection=True;"
}
```

### 3. Apply migrations

```bash
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

Then open:

```
https://localhost:xxxx
```

## Database

- Uses Entity Framework Core (Code First approach)
- Includes migrations in the project
- Main tables:
  - Users (ASP.NET Identity)
  - Tasks

## Project Structure

```
Controllers/
Services/
Repositories/
Models/
Views/
wwwroot/
```

- Controllers: Handle HTTP requests  
- Services: Contain business logic  
- Repositories: Handle data access  
- ViewModels: Shape data for the UI  

## Design Decisions

- Repository pattern for clean data access  
- Service layer for separation of concerns  
- AJAX used for search, delete, and status toggle  
- SweetAlert used for notifications  
- Server-side pagination for performance  

## Time Spent

- Backend (CRUD, authentication, service/repository): 6–7 hours  
- Frontend (UI, AJAX, alerts): 4–5 hours  
- Pagination and CSV export: 2–3 hours  
- Debugging and refinement: 2 hours  

**Total time: approximately 14–17 hours**

## Future Improvements

- Real-time updates (SignalR)  
- Dashboard and analytics  
- Calendar view  
- Enhanced mobile experience  
- Export to Excel (XLSX)  

## Author

Md Rezaul Karim  
https://github.com/mdrezaulkarim38