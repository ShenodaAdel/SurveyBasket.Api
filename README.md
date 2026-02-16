# 📊 Survey Basket API

A robust and scalable **Survey Management System** built with **.NET 8** and **ASP.NET Core Web API**.  
This project allows administrators to create and manage surveys, while users can participate and view results.

---

## 🚀 Features

### 🔐 Authentication & Authorization
- JWT Authentication
- ASP.NET Core Identity
- Role-based Authorization (Admin / User)
- Account Registration & Login
- Email Confirmation (optional)
- Password Management

### 👥 User Management (Admin)
- Create / Update Users
- Lock / Unlock Accounts
- Manage Roles
- Reset Password

### 🗳 Poll Management
- Create, Update, Delete Polls
- Activate / Deactivate Polls
- Set Start & End Dates
- Send Notifications (optional extension)

### ❓ Questions & Answers
- Add Questions to Poll
- Add Multiple Answers per Question
- Update / Delete Questions & Answers

### ✅ Voting System
- Users can participate in active polls
- Prevent duplicate voting
- Store vote history
- Track voting timestamp

### 📊 Results
- View vote counts per answer
- Calculate percentage per answer
- Total participants count
