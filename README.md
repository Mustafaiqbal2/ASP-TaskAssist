# ASP-TaskAssist

A comprehensive task management web application built with ASP.NET Core, designed to help users efficiently organize, track, and complete their tasks.


## Overview

ASP-TaskAssist is a modern web-based task management solution that provides a seamless experience for creating, organizing, and tracking tasks. Built using ASP.NET Core and MS SQL Server, it offers a responsive interface and robust functionality to enhance productivity.

## Features

- **User Authentication**: Secure user registration and login system
- **Task Management**: Create, edit, delete, and prioritize tasks
- **Task Categories**: Organize tasks by categories or projects
- **Due Date Tracking**: Set and monitor task deadlines
- **Status Tracking**: Monitor task progress (Not Started, In Progress, Completed)
- **Search & Filter**: Find tasks quickly using advanced search options
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Notifications**: Get reminders for upcoming and overdue tasks
- **Data Persistence**: All data is stored securely in a SQL database

## Technologies Used

- **Backend**: ASP.NET Core MVC (.NET 6+)
- **Frontend**: HTML5, CSS3, JavaScript
- **Styling**: Bootstrap framework
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity

## Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022 or Visual Studio Code
- SQL Server (Local or Express)
- Git

## Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/Mustafaiqbal2/ASP-TaskAssist.git
   cd ASP-TaskAssist
   ```

2. **Set up the database**
   - Update the connection string in `appsettings.json` to point to your SQL Server instance
   - Run Entity Framework migrations to create the database:
     ```bash
     dotnet ef database update
     ```

3. **Build and run the application**
   ```bash
   dotnet build
   dotnet run
   ```

4. **Access the application**
   - Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`

## Usage

### User Registration and Login
1. Navigate to the Register page and create a new account
2. Log in with your credentials

### Managing Tasks
1. Create new tasks by clicking the "Add Task" button
2. View all your tasks on the dashboard
3. Edit tasks by clicking on them or using the edit button
4. Mark tasks as complete using the checkbox
5. Delete tasks you no longer need

### Organizing Tasks
1. Create categories for your tasks
2. Assign priority levels to differentiate urgent tasks
3. Set due dates to manage deadlines
4. Filter and sort tasks using the provided controls

## Screenshots

*(Add screenshots of your application here)*

## Project Structure

```
ASP-TaskAssist/
├── Controllers/         # MVC Controllers
├── Models/              # Data models and ViewModels
├── Views/               # Razor views
├── Data/                # Database context and migrations
├── Services/            # Business logic services
├── wwwroot/             # Static files (CSS, JS, images)
├── appsettings.json     # Application configuration
└── Program.cs           # Application entry point
```

## Contributing

Contributions are welcome! If you'd like to contribute to ASP-TaskAssist:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

Mustafa Iqbal - [@Mustafaiqbal2](https://github.com/Mustafaiqbal2)

Project Link: [https://github.com/Mustafaiqbal2/ASP-TaskAssist](https://github.com/Mustafaiqbal2/ASP-TaskAssist)

## Acknowledgments

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Bootstrap](https://getbootstrap.com/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

---

Last updated: 2025-03-14
