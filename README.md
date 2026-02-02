# 🚀 Coder Nexus - Smart Bootcamp Management System

**Coder Nexus** is a comprehensive Learning Management System (LMS) designed specifically for technical bootcamps. It bridges the gap between students, trainers, and administration by providing a unified platform for tracking progress, managing tasks, and analyzing performance through data-driven insights.


---

## 🌟 Key Features

###  :dizzy: Genral CoderNexus
- Home Page:
 ![App Screenshot](https://raw.githubusercontent.com/janais9/CoderNexues/main/G.C.png)
- Sign Up / Sign in Page:
 ![App Screenshot](https://raw.githubusercontent.com/janais9/CoderNexues/main/G.C.png)
 ![App Screenshot](https://raw.githubusercontent.com/janais9/CoderNexues/main/G.C.png)


### 🎓 For Students
- **Interactive Dashboard:** View enrolled bootcamps, upcoming tasks, and progress.
- **Task Submission:** Upload assignments (GitHub links/files) directly.
- **Grades & Feedback:** Receive detailed feedback and scores from trainers.
- **Roadmap Timeline:** Visualize the bootcamp schedule and key milestones.
- **Certificates:** Submit exam results and track professional certifications.

### 👨‍🏫 For Trainers
- **Camp Management:** Manage students and bootcamp details.
- **Task Creation:** Create assignments with deadlines and grading criteria.
- **Grading System:** Review submissions, assign grades, and provide feedback.
- **Performance Tracking:** Monitor student progress and identify struggling students.

### 📊 For Administration (Admin)
- **Advanced Analytics Dashboard:** Real-time insights using **Chart.js** (Active students, completion rates, trainer performance).
- **User Management:** Full control over roles (Student, Trainer, Admin) with activation/ban capabilities.
- **Smart Alerts:** Automated alerts for low-performing students or delayed grading.
- **Camp Scheduling:** Define bootcamp timelines and major events.

 ![App Screenshot](https://raw.githubusercontent.com/janais9/CoderNexues/main/G.C.png)
 ![App Screenshot](https://raw.githubusercontent.com/janais9/CoderNexues/main/G.C.png)
 ![App Screenshot](https://raw.githubusercontent.com/janais9/CoderNexues/main/G.C.png)
 ![App Screenshot](https://raw.githubusercontent.com/janais9/CoderNexues/main/G.C.png)
 ![App Screenshot](https://raw.githubusercontent.com/janais9/CoderNexues/main/G.C.png)
 ![App Screenshot](https://raw.githubusercontent.com/janais9/CoderNexues/main/G.C.png)

---

## 🛠️ Tech Stack & Architecture

This project is built using **Clean MVC Architecture** and modern .NET technologies.

- **Framework:** ASP.NET Core 10.0 (MVC)
- **Database:** SQL Server
- **ORM:** Entity Framework Core (Code-First Approach)
- **Authentication:** Custom Cookie-Based Authentication & Claims
- **Frontend:** HTML5, CSS3, Bootstrap 5, Chart.js
- **Tools:** Visual Studio 2022, Git

---

## 🗂️ Database Schema (ERD Overview)

The system is built on a relational database with the following core entities:

- **Users:** Stores user credentials and roles (Student, Trainer, Admin).
- **Camps:** Represents bootcamps with start/end dates.
- **CampUsers:** Handles Many-to-Many relationships between users and camps.
- **Tasks & Submissions:** Manages assignments and student work.
- **Evaluations:** Stores grades and qualitative feedback.
- **Certificates:** Tracks professional certification requirements and results.

---

## 🚀 Getting Started

Follow these steps to run the project locally:

### 1. Prerequisites
- .NET 8.0 or 10 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022

### 2. Clone the Repository
```bash
git clone https://github.com/janais9/CoderNexues.git
