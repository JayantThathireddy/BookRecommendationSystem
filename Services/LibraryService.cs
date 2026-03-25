using ITCS_3112_Lab_2_Recommendation.Contracts;
using ITCS_3112_Lab_2_Recommendation.Domain;
using ITCS_3112_Lab_2_Recommendation.Repositories;

namespace ITCS_3112_Lab_2_Recommendation.Services;

public class LibraryService : ILibraryService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IRatingRepository _ratingRepository;
    private Member? _currentMember;

    public LibraryService(IBookRepository bookRepository, IMemberRepository memberRepository, IRatingRepository ratingRepository)
    {
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _ratingRepository = ratingRepository;
    }

    public void LoadData(string booksFile, string ratingsFile)
    {
        _bookRepository.LoadBooksFromFile(booksFile);
        _ratingRepository.LoadRatingsFromFile(ratingsFile, _memberRepository, _bookRepository);
    }

    public bool Login(string accountId)
    {
        var member = _memberRepository.GetMemberByAccount(accountId);
        if (member != null)
        {
            _currentMember = member;
            return true;
        }
        return false;
    }

    public void Logout()
    {
        _currentMember = null;
    }

    public bool IsLoggedIn()
    {
        return _currentMember != null;
    }

    public Member? GetCurrentMember()
    {
        return _currentMember;
    }

    public Member? AddMember(string name)
    {
        var member = new Member(name);
        _memberRepository.AddMember(member);
        return member;
    }

    public Book? AddBook(string author, string title, string year)
    {
        var book = new Book(author, title, year);
        _bookRepository.AddBook(book);
        return book;
    }

    public void RateBook(string bookISBN, int score)
    {
        if (!IsLoggedIn()) return;

        var rating = new Rating(_currentMember!.Account, bookISBN, score);
        _ratingRepository.AddRating(rating);
    }

    public int GetRating(string bookISBN)
    {
        if (!IsLoggedIn()) return 0;
        return _ratingRepository.GetRating(_currentMember!.Account, bookISBN);
    }

    public List<(Book, int)> GetMyRatings()
    {
        if (!IsLoggedIn()) return new List<(Book, int)>();

        var allBooks = _bookRepository.GetAllBooks();
        var result = new List<(Book, int)>();

        foreach (var book in allBooks)
        {
            int score = _ratingRepository.GetRating(_currentMember!.Account, book.ISBN);
            result.Add((book, score));
        }

        return result;
    }

    public List<Book> GetRecommendedBooks(int count = 5)
    {
        if (!IsLoggedIn()) return new List<Book>();

        var allMembers = _memberRepository.GetAllMembers();
        var allBooks = _bookRepository.GetAllBooks();
        var currentMemberAccount = _currentMember!.Account;

        var similarities = new Dictionary<string, int>();

        // Calculate finding the most similar member by checking their dot product
        foreach (var otherMember in allMembers)
        {
            if (otherMember.Account == currentMemberAccount) continue;

            int dotProduct = 0;
            // The dot-product similarity algorithm takes the products of corresponding ratings 
            // for two members. Highest overall value indicates maximum similarly.
            foreach (var book in allBooks)
            {
                int myRating = _ratingRepository.GetRating(currentMemberAccount, book.ISBN);
                int theirRating = _ratingRepository.GetRating(otherMember.Account, book.ISBN);
                dotProduct += myRating * theirRating;
            }

            similarities[otherMember.Account] = dotProduct;
        }

        // Get the single most similar member to base our recommendations on
        var mostSimilarMember = similarities.OrderByDescending(kvp => kvp.Value).FirstOrDefault();

        if (mostSimilarMember.Key == null) return new List<Book>();

        var theirRatings = _ratingRepository.GetRatingsByMember(mostSimilarMember.Key)
            .Where(r => r.Score > 0)
            .OrderByDescending(r => r.Score)
            .ToList();

        var recommendations = new List<Book>();
        foreach (var rating in theirRatings)
        {
            int myRating = _ratingRepository.GetRating(currentMemberAccount, rating.BookISBN);
            // Ensure we do not recommend a book the user has already rated (read)
            if (myRating == 0)
            {
                var book = _bookRepository.GetBookByISBN(rating.BookISBN);
                if (book != null)
                {
                    recommendations.Add(book);
                    if (recommendations.Count >= count) break;
                }
            }
        }

        return recommendations;
    }

    public List<Book> GetAllBooks()
    {
        return _bookRepository.GetAllBooks();
    }

    public int GetMemberCount()
    {
        return _memberRepository.GetAllMembers().Count;
    }

    public (Member? similarMember, List<Book> reallyLiked, List<Book> liked) GetDetailedRecommendations()
    {
        if (!IsLoggedIn()) return (null, new List<Book>(), new List<Book>());

        var allMembers = _memberRepository.GetAllMembers();
        var allBooks = _bookRepository.GetAllBooks();
        var currentMemberAccount = _currentMember!.Account;

        var similarities = new Dictionary<string, int>();

        foreach (var otherMember in allMembers)
        {
            if (otherMember.Account == currentMemberAccount) continue;

            int dotProduct = 0;
            foreach (var book in allBooks)
            {
                int myRating = _ratingRepository.GetRating(currentMemberAccount, book.ISBN);
                int theirRating = _ratingRepository.GetRating(otherMember.Account, book.ISBN);
                dotProduct += myRating * theirRating;
            }

            similarities[otherMember.Account] = dotProduct;
        }

        var mostSimilarMemberKvp = similarities.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
        if (mostSimilarMemberKvp.Key == null) return (null, new List<Book>(), new List<Book>());

        Member? similarMember = _memberRepository.GetMemberByAccount(mostSimilarMemberKvp.Key);
        
        var theirRatings = _ratingRepository.GetRatingsByMember(mostSimilarMemberKvp.Key);
        
        var reallyLiked = new List<Book>();
        var liked = new List<Book>();

        foreach (var rating in theirRatings.Where(r => r.Score >= 3).OrderByDescending(r => r.Score))
        {
            int myRating = _ratingRepository.GetRating(currentMemberAccount, rating.BookISBN);
            if (myRating == 0) // I haven't read it
            {
                var book = _bookRepository.GetBookByISBN(rating.BookISBN);
                if (book != null)
                {
                    if (rating.Score == 5)
                        reallyLiked.Add(book);
                    else if (rating.Score == 3)
                        liked.Add(book);
                }
            }
        }

        return (similarMember, reallyLiked, liked);
    }
}