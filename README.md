TEAM MEMBERS:
David Chukwudi Igberi
Akuegbo Iheanyi Ejeagba
Micah Brown
Daniel Ayvazyan


tester@test.com

TestPassword!23


seeding:

if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();

    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<AppDbContext>();

    var userManager =
        services.GetRequiredService<UserManager<ApplicationUser>>();

    // Apply migrations first.
    await db.Database.MigrateAsync();

    // Seed the database.
    await DatabaseSeeder.SeedAsync(
        db,
        userManager);

    return;
}



AI Generated Project Ideas:

1. Campus Event Management Portal ⭐⭐⭐⭐⭐

A web app where students can create, browse, and register for campus events.

Features

User registration and login
Event creation and editing
Event categories
RSVP system
Attendance tracking
Event search and filtering
Admin dashboard

Technical skills

ASP.NET Identity
Entity Framework Core
Authorization roles
File uploads for event images
Email notifications (optional)

2. Personal Finance Tracker ⭐⭐⭐⭐⭐

Users manage income, expenses, and budgets.

Features

Income/expense tracking
Categories
Monthly budgets
Charts and analytics
Recurring transactions
Savings goals
CSV import/export

Stretch goals

Receipt image uploads
AI spending insights

3. Study Group & Course Collaboration Platform

Students create study groups for classes.

Features

Course listings
Group creation
Join requests
Meeting scheduler
Shared notes
Discussion board
Notifications

4. Equipment Reservation System

Reserve shared resources such as laptops, cameras, or lab equipment.

Features

Equipment inventory
Reservation calendar
Availability checking
Admin approvals
Damage reports
Check-in/check-out logs

5. Task & Project Management System

A simplified project management tool similar to Trello.

Features

Kanban board
Tasks
Due dates
Teams
Comments
Labels
File attachments

Good for demonstrating

Drag-and-drop UI
Component architecture

6. Library Management System

For schools or small organizations.

Features

Book catalog
Search
Borrow/return
Reservations
Fine calculation
Librarian dashboard

7. Online Learning Portal

Students can enroll in courses.

Features

Course catalog
Video links
Assignments
Progress tracking
Quiz system
Instructor dashboard

8. Restaurant Ordering System

Customers order food online.

Features

Menu
Shopping cart
Checkout
Order tracking
Kitchen dashboard
Admin menu management

9. Medical Appointment Scheduler

Patients schedule appointments.

Features

Doctor schedules
Appointment booking
Cancellation
Availability calendar
Admin dashboard
Email reminders

10. Gym Membership Portal

Manage memberships and classes.

Features

Membership plans
Workout classes
Booking
Trainer profiles
Attendance
Payment history

11. Property Rental Management

A landlord dashboard.

Features

Property listings
Tenants
Maintenance requests
Lease tracking
Rent payments
Reports

12. Volunteer Management Platform

For charities.

Features

Volunteer registration
Events
Hours tracking
Certificates
Admin approvals
Messaging

13. Online Marketplace

Students buy and sell items.

Features

Listings
Categories
Search
Favorites
Messaging
Seller ratings

14. Movie & TV Collection Tracker

Track watched media.

Features

Personal library
Ratings
Reviews
Watchlists
Recommendations
Statistics

15. Recipe Sharing Community

Users upload and discover recipes.

Features

Recipes
Images
Ingredients
Categories
Comments
Ratings
Favorites

16. Hotel Reservation System

A hotel booking website.

Features

Room inventory
Booking calendar
Availability
Customer management
Admin dashboard
Booking history

17. Bug Tracking System

Perfect for software engineering students.

Features

Create issues
Assign developers
Priorities
Status workflow
Comments
Dashboard
Reports

18. Internship & Job Board

A college career center application.

Features

Employer accounts
Student accounts
Job postings
Resume uploads
Applications
Saved jobs

19. Fitness Challenge Platform

Users compete with friends.

Features

Goals
Daily logs
Leaderboards
Teams
Badges
Statistics
20. Smart Inventory Management

Designed for a small business.

Features

Products
Stock levels
Suppliers
Purchase orders
Sales
Alerts
Reports