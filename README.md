# Book Recommendation System - ITCS 3112 Lab 2

A console-based C# application that provides personalized book recommendations to users. This system uses a dot-product similarity algorithm to compare user rating histories and recommend unread books based on the preferences of similar users.

## Features
- **Data Loading:** Parses and loads books and ratings data from `.txt` files.
- **User Authentication:** Login/logout functionality using unique Account IDs.
- **Recommendation Engine:** Calculates similarity between users using dot-products to suggest books.
- **Rating System:** Allows users to rate books on a scale from -5 (Hated it) to 5 (Really liked it).
- **Management:** Add new books and register new members directly from the console.
- **View History:** See a list of all your previously rated books.

## Architecture
The application is built applying SOLID Object-Oriented Design principles and is organized into logical layers:
- **Domain:** Core data models (`Book`, `Member`, `Rating`).
- **Contracts:** Interfaces for loose coupling (`ILibraryService`, `IBookRepository`, etc.).
- **Repositories:** Data access and file parsing logic (`BookRepository`, `RatingRepository`, `MemberRepository`).
- **Services:** Core business logic and recommendation algorithms (`LibraryService`).

## How to Run
1. Open the project in your preferred IDE (e.g., JetBrains Rider, Visual Studio).
2. Ensure `books.txt` and `ratings.txt` are located in the `Data/` directory.
3. Build and run the project.
4. When prompted, press `Enter` to use the default file paths (`Data\books.txt` and `Data\ratings.txt`).
5. Follow the on-screen console prompts to log in, view ratings, or generate recommendations!

## Technology Stack
- **Language:** C# (.NET 10.0)
- **UI:** Console Application
