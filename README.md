<p align="center">
  <a href="https://github.com/maurogioberti" target="_blank">
    <img alt="Mauro Gioberti" src="https://www.maurogioberti.com/assets/profile/maurogioberti-avatar.png" width="200" />
  </a>
</p>

<h1 align="center">
  Appointments Microservice 🚀 - 🚧 Development In Progress
</h1>
<p align="center">
  A fun-to-use C# microservice built with .NET 8.  
  <br />
  It’s all about <strong>testing</strong>, <strong>automation</strong>, and making developers’ lives easier.  
  <br />
  <br />
  <a href="https://github.com/maurogioberti/microservice-appointments/stargazers">⭐ Leave a star if you like it!</a>
  <a href="https://github.com/maurogioberti/microservice-appointments/issues">💰 Found a bug? Report it here!</a>
</p>

<p align="center">
  <a href="https://github.com/maurogioberti/microservice-appointments" title="C# Microservice Testing and Automation" target="_blank">
    <img src="https://img.shields.io/badge/Built_with-.NET | Made_for_Testing-blue?style=for-the-badge" alt="Built with .NET and Testing Focus" />
  </a>
</p>

---

## 🚀 What’s This About?

The Appointments Microservice is a **testing playground** disguised as a backend app. It’s built to show off **Unit Testing**, **Functional Testing**, and **Automation**. Sure, it uses Clean Architecture, but the star of the show is making sure everything is fully tested and automated for reliability.

### Why Testing and Automation?
- **Unit Testing**: Keeps your core logic solid, even when the code changes.  
- **Functional Testing**: Makes sure the app behaves how it should in the real world.  
- **Automation**: 🚀 This is where the fun begins! Verifies the integrity of the microservice implementation by hitting endpoints or interacting with bus services.

---

## 🚦 Get Started Quickly

1. **Clone this repo**:

   ```bash
   git clone https://github.com/maurogioberti/microservice-appointments.git
   cd microservice-appointments
   ```

2. **Restore dependencies**:

   ```bash
   dotnet restore
   ```

3. **Set up and run the solutions**:

#### **Main Solution**: 
  1. Open `Microservice.Appointments.sln` in Visual Studio or your preferred IDE.
  2. Set `Microservice.Appointments.Api` as the startup project.
  3. Ensure Docker is running to support the EventBus (RabbitMQ) and Database (SQL Server) containers.
  4. Run the project to host the API locally (by default on `http://localhost:[port]`).

#### **IntegrityAssurance Solution**: 
  1. Open `Microservice.Appointments.IntegrityAssurance.sln`.
  2. Navigate to the `Microservice.Appointments.IntegrationTests` project.
  3. Ensure the `Microservice.Appointments.Api` is running in **IntegrityAssurance mode**.
  4. Execute the tests and validate the workflows.

---

## 🐳 Docker Containers

This project uses **Docker** to simplify the infrastructure setup. Make sure Docker is running, and the app will handle everything for you automatically.  

Take a coffee ☕ and relax while the app spins up the containers. Here's what's included:

1. **EventBus (RabbitMQ):** Handles asynchronous messaging for domain events like `AppointmentCreatedEvent`.  
2. **Database (SQL Server):** Stores appointment data and ensures persistence for the service.  

Everything is pre-configured and ready to go. Just hit "Run" and start testing! 😎

---

## 📂 How It’s Organized

This app keeps things clean, testable, and easy to automate:  

```
/Microservice.Appointments.sln # Main solution
/Microservice.Appointments.IntegrityAssurance.sln # Testing solution
/src
├── Api               # Your API controllers, middlewares
├── Application       # Use cases, DTOs, and services
├── Domain            # Core business logic like entities and events
├── Infrastructure    # Database, repositories, and external integrations
/tests
├── FunctionalTests   # End-to-end tests for APIs
├── UnitTests         # Isolated tests for logic and services
└── IntegrationTests  # Tests that touch the database or external services
```

---

## 🧪 Testing Is Everything

Consistency is key, and this project follows the **Given_When_Then** naming convention for tests to ensure maintainability.  

### Test Naming Pattern:  
`Given_[Condition]_When_[Action]_Then_[ExpectedOutcome]`  
- Example: `Given_Valid_Parameters_When_ExecuteAsync_Called_Then_Returns_Expected_Result`.

### 🎩 Unit Testing
Unit tests keep things predictable:
- Test the logic in your services, entities, domains, and use cases. 

### 🔀 Functional Testing
Functional tests check the big picture:
- Validate that API endpoints respond the way they’re supposed to.  
- Mock those infrastructure dependencies so tests don’t need a database.  

### 🧬 Automation Is Your Best Friend
Automation makes life easier. Here’s what this microservice automates:
- **API Testing**: Every endpoint gets tested, so you don’t miss a thing. 

---

## 🖓 Why Testing and Automation Matter

1. **Saves Time**: Automating tests means less debugging and more time for coding cool features. 😎
2. **Confidence in Changes**: Know your updates won’t break the app. 😁
3. **Smooth Deployments**: Automating checks makes the app stable and easy to scale. 🪜

---

## 🤖 Continuous Integration: Built for Reliability  

This project features a lightweight **CI pipeline** to keep everything stable and running smoothly.  

### 🛠️ Steps in the Pipeline:  
1. **🧪 Run Unit Tests:** Ensures all core logic behaves as expected.  
2. **🔀 Run Functional Tests:** Validates that endpoints and workflows are working correctly.  

💡 With these automated steps, you can push with confidence knowing the app won’t break! 🤓  

---

## 📓 License

This project is under the [MIT License](https://github.com/maurogioberti/microservice-appointments/blob/master/LICENSE), so feel free to use it, share it, or break it—just don’t forget to give credit!

---

If you're a dev 👨‍💻 and you've never done something like this... 🤔 What are you waiting for? 💥 Improve your quality and get excited to dive in! 🚀