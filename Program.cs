using ITCS_3112_Lab_2_Recommendation.Services;
using ITCS_3112_Lab_2_Recommendation.Contracts;
using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation;

class Program
{
    static void Main(string[] args)
    {
        ILibraryService libraryService = new LibraryService();

        Console.WriteLine("==============================================");
        Console.WriteLine("   Welcome to Book Recommendation System");
        Console.WriteLine("==============================================\n");

        Console.Write("Enter books file path (or press Enter for default 'Data\\books.txt'): ");
        string booksFile = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(booksFile))
        {
            booksFile = "Data\\books.txt";
        }

        Console.Write("Enter ratings file path (or press Enter for default 'Data\\ratings.txt'): ");
        string ratingsFile = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(ratingsFile))
        {
            ratingsFile = "Data\\ratings.txt";
        }

        try
        {
            libraryService.LoadData(booksFile, ratingsFile);
            Console.WriteLine("\n✓ Data loaded successfully!\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error loading data: {ex.Message}");
            return;
        }

        bool running = true;
        while (running)
        {
            if (libraryService.IsLoggedIn())
            {
                ShowLoggedInMenu(libraryService);
            }
            else
            {
                ShowLoggedOutMenu(libraryService);
            }

            Console.Write("\nEnter your choice: ");
            string choice = Console.ReadLine()?.Trim() ?? "";

            try
            {
                if (libraryService.IsLoggedIn())
                {
                    running = HandleLoggedInChoice(choice, libraryService);
                }
                else
                {
                    running = HandleLoggedOutChoice(choice, libraryService);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}");
            }

            if (running)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        Console.WriteLine("\nThank you for using the Book Recommendation System!");
    }

    static void ShowLoggedOutMenu(ILibraryService service)
    {
        Console.WriteLine("\n==============================================");
        Console.WriteLine("   Menu (Not Logged In)");
        Console.WriteLine("==============================================");
        Console.WriteLine("1. Login");
        Console.WriteLine("2. Add New Member");
        Console.WriteLine("3. Add New Book");
        Console.WriteLine("4. Exit");
        Console.WriteLine("==============================================");
    }

    static void ShowLoggedInMenu(ILibraryService service)
    {
        var currentMember = service.GetCurrentMember();
        Console.WriteLine("\n==============================================");
        Console.WriteLine($"   Menu (Logged in as: {currentMember?.Name})");
        Console.WriteLine("==============================================");
        Console.WriteLine("1. Logout");
        Console.WriteLine("2. Add New Member");
        Console.WriteLine("3. Add New Book");
        Console.WriteLine("4. Rate a Book");
        Console.WriteLine("5. View My Ratings");
        Console.WriteLine("6. See Recommended Books");
        Console.WriteLine("7. Exit");
        Console.WriteLine("==============================================");
    }

    static bool HandleLoggedOutChoice(string choice, ILibraryService service)
    {
        switch (choice)
        {
            case "1":
                LoginMember(service);
                break;
            case "2":
                AddNewMember(service);
                break;
            case "3":
                AddNewBook(service);
                break;
            case "4":
                return false;
            default:
                Console.WriteLine("\n✗ Invalid choice. Please try again.");
                break;
        }
        return true;
    }

    static bool HandleLoggedInChoice(string choice, ILibraryService service)
    {
        switch (choice)
        {
            case "1":
                service.Logout();
                Console.WriteLine("\n✓ Logged out successfully!");
                break;
            case "2":
                AddNewMember(service);
                break;
            case "3":
                AddNewBook(service);
                break;
            case "4":
                RateBook(service);
                break;
            case "5":
                ViewMyRatings(service);
                break;
            case "6":
                SeeRecommendations(service);
                break;
            case "7":
                return false;
            default:
                Console.WriteLine("\n✗ Invalid choice. Please try again.");
                break;
        }
        return true;
    }

    static void LoginMember(ILibraryService service)
    {
        Console.Write("\nEnter your Account ID: ");
        string account = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrEmpty(account))
        {
            Console.WriteLine("✗ Account ID cannot be empty.");
            return;
        }

        if (service.Login(account))
        {
            Console.WriteLine($"\n✓ Welcome back, {service.GetCurrentMember()?.Name}!");
        }
        else
        {
            Console.WriteLine("\n✗ Member not found. Please check your Account ID or add yourself as a new member.");
        }
    }

    static void AddNewMember(ILibraryService service)
    {
        Console.Write("\nEnter member name: ");
        string name = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("✗ Name cannot be empty.");
            return;
        }

        var member = service.AddMember(name);
        if (member != null)
        {
            Console.WriteLine($"\n✓ Member added successfully!");
            Console.WriteLine($"   Name: {member.Name}");
            Console.WriteLine($"   Account: {member.Account}");
        }
        else
        {
            Console.WriteLine("\n✗ Member with this name already exists.");
        }
    }

    static void AddNewBook(ILibraryService service)
    {
        Console.Write("\nEnter book author: ");
        string author = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Enter book title: ");
        string title = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Enter publication year: ");
        string year = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrEmpty(author) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(year))
        {
            Console.WriteLine("\n✗ All fields are required.");
            return;
        }

        var book = service.AddBook(author, title, year);
        if (book != null)
        {
            Console.WriteLine($"\n✓ Book added successfully!");
            Console.WriteLine($"   ISBN: {book.ISBN}");
            Console.WriteLine($"   {book}");
        }
    }

    static void RateBook(ILibraryService service)
    {
        var books = service.GetAllBooks();

        Console.WriteLine("\n--- Available Books ---");
        for (int i = 0; i < books.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {books[i]}");
        }

        Console.Write("\nEnter book number to rate: ");
        string input = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(input, out int bookNumber) || bookNumber < 1 || bookNumber > books.Count)
        {
            Console.WriteLine("\n✗ Invalid book number.");
            return;
        }

        var selectedBook = books[bookNumber - 1];

        Console.WriteLine("\nRating Scale:");
        Console.WriteLine("-5 :  Hated it!");
        Console.WriteLine("-3 :  Didn't like it");
        Console.WriteLine(" 0 :  Haven't read it");
        Console.WriteLine(" 1 :  Ok – neither hot nor cold");
        Console.WriteLine(" 3 :  Liked it!");
        Console.WriteLine(" 5 :  Really liked it!");

        Console.Write("\nEnter your rating (-5, -3, 0, 1, 3, 5): ");
        string ratingInput = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(ratingInput, out int rating) ||
            (rating != -5 && rating != -3 && rating != 0 && rating != 1 && rating != 3 && rating != 5))
        {
            Console.WriteLine("\n✗ Invalid rating. Must be -5, -3, 0, 1, 3, or 5.");
            return;
        }

        service.RateBook(selectedBook.ISBN, rating);
        Console.WriteLine($"\n✓ Your rating for '{selectedBook.Title}' has been saved!");
    }

    static void ViewMyRatings(ILibraryService service)
    {
        var ratings = service.GetMyRatings();

        if (ratings.Count == 0)
        {
            Console.WriteLine("\nYou haven't rated any books yet.");
            return;
        }

        Console.WriteLine("\n--- Your Ratings ---");
        foreach (var (book, score) in ratings)
        {
            Console.WriteLine($"{book} - Rating: {score} ({Rating.GetRatingMeaning(score)})");
        }
        Console.WriteLine($"\nTotal books rated: {ratings.Count}");
    }

    static void SeeRecommendations(ILibraryService service)
    {
        var recommendations = service.GetRecommendedBooks(5);

        if (recommendations.Count == 0)
        {
            Console.WriteLine("\nNo recommendations available at this time.");
            Console.WriteLine("Try rating more books to get better recommendations!");
            return;
        }

        Console.WriteLine("\n--- Recommended Books for You ---");
        for (int i = 0; i < recommendations.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {recommendations[i]}");
        }
    }
}