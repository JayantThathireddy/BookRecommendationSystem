using ITCS_3112_Lab_2_Recommendation.Services;
using ITCS_3112_Lab_2_Recommendation.Contracts;
using ITCS_3112_Lab_2_Recommendation.Domain;
using ITCS_3112_Lab_2_Recommendation.Repositories;

namespace ITCS_3112_Lab_2_Recommendation;

class Program
{
    static void Main(string[] args)
    {
        IBookRepository bookRepo = new BookRepository();
        IMemberRepository memberRepo = new MemberRepository();
        IRatingRepository ratingRepo = new RatingRepository();
        ILibraryService libraryService = new LibraryService(bookRepo, memberRepo, ratingRepo);

        Console.WriteLine("Welcome to the Book Recommendation Program.\n");

        Console.Write("Enter books file (default: Data\\books.txt): ");
        string booksFile = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(booksFile))
        {
            booksFile = "Data\\books.txt";
        }

        Console.Write("Enter rating file (default: Data\\ratings.txt): ");
        string ratingsFile = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(ratingsFile))
        {
            ratingsFile = "Data\\ratings.txt";
        }

        try
        {
            libraryService.LoadData(booksFile, ratingsFile);
            Console.WriteLine($"\n# of books: {libraryService.GetAllBooks().Count}");
            Console.WriteLine($"# of memberList: {libraryService.GetMemberCount()}\n");
        }
        catch (Exception ex)
        {
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

            Console.Write("\nEnter a menu option: ");
            string choice = Console.ReadLine()?.Trim() ?? "";
            Console.WriteLine();

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
        }

        Console.WriteLine("\nThank you for using the Book Recommendation Program!");
    }

    static void ShowLoggedOutMenu(ILibraryService service)
    {
        Console.WriteLine("\n************** MENU **************");
        Console.WriteLine("* 1. Add a new member            *");
        Console.WriteLine("* 2. Add a new book              *");
        Console.WriteLine("* 3. Login                       *");
        Console.WriteLine("* 4. Quit                        *");
        Console.WriteLine("**********************************");
    }

    static void ShowLoggedInMenu(ILibraryService service)
    {
        var currentMember = service.GetCurrentMember();
        Console.WriteLine("\n************** MENU **************");
        Console.WriteLine("* 1. Add a new member            *");
        Console.WriteLine("* 2. Add a new book              *");
        Console.WriteLine("* 3. Rate book                   *");
        Console.WriteLine("* 4. View ratings                *");
        Console.WriteLine("* 5. See recommendations         *");
        Console.WriteLine("* 6. Logout                      *");
        Console.WriteLine("**********************************");
    }

    static bool HandleLoggedOutChoice(string choice, ILibraryService service)
    {
        switch (choice)
        {
            case "1":
                AddNewMember(service);
                break;
            case "2":
                AddNewBook(service);
                break;
            case "3":
                LoginMember(service);
                break;
            case "4":
                return false;
        }
        return true;
    }

    static bool HandleLoggedInChoice(string choice, ILibraryService service)
    {
        switch (choice)
        {
            case "1":
                AddNewMember(service);
                break;
            case "2":
                AddNewBook(service);
                break;
            case "3":
                RateBook(service);
                break;
            case "4":
                ViewMyRatings(service);
                break;
            case "5":
                SeeRecommendations(service);
                break;
            case "6":
                service.Logout();
                break;
        }
        return true;
    }

    static void LoginMember(ILibraryService service)
    {
        Console.Write("Enter member account: ");
        string account = Console.ReadLine()?.Trim() ?? "";

        if (service.Login(account))
        {
            Console.WriteLine($"{service.GetCurrentMember()?.Name}, you are logged in!");
        }
    }

    static void AddNewMember(ILibraryService service)
    {
        Console.Write("Enter the name of the new member: ");
        string name = Console.ReadLine()?.Trim() ?? "";

        var member = service.AddMember(name);
        if (member != null)
        {
            Console.WriteLine($"{member.Name} (account #: {member.Account}) was added.");
        }
    }

    static void AddNewBook(ILibraryService service)
    {
        Console.Write("Enter the author of the new book: ");
        string author = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Enter the title of the new book: ");
        string title = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Enter the year (or range of years) of the new book: ");
        string year = Console.ReadLine()?.Trim() ?? "";

        var book = service.AddBook(author, title, year);
        if (book != null)
        {
            Console.WriteLine($"{book} was added.");
        }
    }

    static void RateBook(ILibraryService service)
    {
        Console.Write("Enter the ISBN for the book you'd like to rate: ");
        string input = Console.ReadLine()?.Trim() ?? "";

        int currentRating = service.GetRating(input);
        
        var allBooks = service.GetAllBooks();
        var selectedBook = allBooks.FirstOrDefault(b => b.ISBN == input);

        if (selectedBook == null) {
            return;
        }

        if (currentRating != 0)
        {
            Console.WriteLine($"Your current rating for {selectedBook} => rating: {currentRating}");
            Console.Write("Would you like to re-rate this book (y/n)? ");
            string rerate = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (rerate != "y" && rerate != "yes")
            {
                return;
            }
        }

        Console.Write("Enter your rating: ");
        string ratingInput = Console.ReadLine()?.Trim() ?? "";

        if (int.TryParse(ratingInput, out int rating))
        {
            service.RateBook(input, rating);
            Console.WriteLine($"Your new rating for {selectedBook} => rating: {rating}");
        }
    }

    static void ViewMyRatings(ILibraryService service)
    {
        var currentMember = service.GetCurrentMember();
        Console.WriteLine($"{currentMember?.Name}'s ratings...");

        var ratings = service.GetMyRatings();
        foreach (var (book, score) in ratings)
        {
            Console.WriteLine($"{book} => rating: {score}");
        }
    }

    static void SeeRecommendations(ILibraryService service)
    {
        var (similarMember, reallyLiked, liked) = service.GetDetailedRecommendations();
        
        if (similarMember == null) {
            Console.WriteLine("No similar members found.");
            return;
        }

        Console.WriteLine($"\nYou have similar taste in books as {similarMember.Name}!\n");
        
        if (reallyLiked.Count > 0)
        {
            Console.WriteLine("Here are the books they really liked:");
            foreach (var b in reallyLiked)
            {
                Console.WriteLine(b);
            }
        }
        
        if (liked.Count > 0)
        {
            Console.WriteLine("\nAnd here are the books they liked:");
            foreach (var b in liked)
            {
                Console.WriteLine(b);
            }
        }
    }
}