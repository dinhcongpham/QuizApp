# Link github Front-end: [Link](https://github.com/dinhcongpham/quiz-app-vite)

# QuizApp

QuizApp is a web-based application designed to create, manage, and participate in online quizzes. Built with .NET Core, it leverages modern technologies like SignalR, JWT Authentication, and Entity Framework Core to deliver a seamless and secure user experience.

## Key Features

- **User Management**:
  - User registration, login, and account management.
  - Password reset and profile updates.
  - Email notifications for forgotten passwords.

- **Quiz Management**:
  - Create, edit, and delete quizzes.
  - Add questions with multiple-choice options and correct answers.
  - View a list of created quizzes.

- **Real-Time Game Rooms**:
  - Create game rooms for quizzes.
  - Join game rooms using room codes.
  - Real-time synchronization using SignalR.

- **Statistics and Leaderboards**:
  - View personal statistics such as quizzes created, participated, and accuracy rate.
  - Display leaderboards for players in game rooms.

- **Security**:
  - JWT-based authentication and authorization.
  - Password encryption using BCrypt.

## Technologies Used

- **Backend**:
  - .NET Core 8.0
  - Entity Framework Core
  - SignalR
  - Microsoft SQL Server

- **Frontend**:
  - API-ready for integration with any frontend framework.

- **Others**:
  - Docker for containerized deployment.
  - Swagger for API documentation.
  - MemoryCache for real-time data storage.

## Project Structure

- `QuizApp.API`: Contains API controllers and SignalR hubs.
- `QuizApp.Core`: Contains core interfaces and entities.
- `QuizApp.Infrastructure`: Contains services, repositories, and data access logic.
- `QuizApp.Shared`: Contains shared DTOs and helpers.
- `QuizApp.Tests`: Contains unit tests for the application.

## How to Run the Project

### Prerequisites

- .NET SDK 8.0 or higher
- Docker (optional for containerized deployment)
- SQL Server

### Steps

1. **Clone the Repository**:
   ```bash
   git clone <repository-url>
   cd QuizApp
   ```
2. **Configure the Database**:
- Update the connection string in appsettings.json and appsettings.Development.json
3. **Run the Application**:
- Using .NET CLI:
```bash
dotnet run --project QuizApp/QuizApp.csproj
```
- Using Docker:
```bash
docker-compose up
```
4. **Access The Application**:
- Open your browser and navigate to `http://localhost:5079/swagger` to view the API documentation.

## User Interface Overview

#### **1. Login**
- Allows users to authenticate using their credentials.  
  ![Login](images/login_screen.png)

#### **2. Register**
- New users can create an account by providing required registration information.  
  ![Register](images/register_screen.png)

#### **3. Forgot Password**
- Users can request a password reset. A new password will be sent via email.  
  ![Forgot Password](images/forgot_pass_screen.png)

#### **4. Home**
- Displays all available quizzes. Users can search quizzes by name.  
  ![Home](images/home_screen.png)

#### **5. Manage Quizzes**
- Provides CRUD (Create, Read, Update, Delete) operations for quizzes.  
  ![Manage Quizzes](images/manage_quiz_screen.png)

#### **6. Manage Questions**
- Enables management of questions associated with each quiz. Includes CRUD functionality.  
  ![Manage Questions](images/manag_questions_quiz_screen.png)

#### **7. How to Play**
- Users can select a quiz and create a room to start a session.  
  ![How to Play](images/play_in_home_screen.png)

#### **8. Waiting Room**
- Displays waiting area for the game. Users can join later or start the game once ready.  
  ![Waiting Room](images/waiting_room_screen.png)

#### **9. In-Game**
- Each question has a countdown timer. The leaderboard updates after each question.  
  ![In-Game](images/in_game_screen.png)

#### **10. Game Results**
- Shows final results, including accuracy, rank, total score, and detailed question outcomes.  
  ![Finish Game](images/finish_game_screen.png)

#### **11. Profile**
- Displays user statistics such as total quizzes created, total participants, and accuracy rate.  
  ![Profile](images/profile_screen.png)

#### **12. Game History**
- Lists all previously participated quizzes and their details.  
  ![Game History](images/history_quiz_result.png)  
  ![Detailed History](images/details_history_quiz_result.png)

#### **13. Settings**
- Allows users to update their display name and change password.  
  ![Settings](images/profile_change_name_password.png)




