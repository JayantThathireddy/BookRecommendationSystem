using ITCS_3112_Lab_2_Recommendation.Contracts;
using ITCS_3112_Lab_2_Recommendation.Repositories;

namespace ITCS_3112_Lab_2_Recommendation.services;

public class LibraryService : ILibraryService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IRatingRepository _ratingRepository;
    private Member? _currentMember;

    public LibraryService()
    {
        _bookRepository = new BookRepository();
        _memberRepository = new MemberRepository();
        _ratingRepository = new RatingRepository();
    }

    public void LoadData(string booksFile, string ratingsFile)
    {
        _bookRepository.LoadBooksFromFile(booksFile);
        _ratingRepository.LoadRatingsFromFile(ratingsFile, _memberRepository, _bookRepository);
    }

    public bool Login(string memberName)
    {
        var member = _memberRepository.GetMemberByName(memberName);
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
        if (_memberRepository.GetMemberByName(name) != null)
            return null;

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

    public List<(Book, int)> GetMyRatings()
    {
        if (!IsLoggedIn()) return new List<(Book, int)>();

        var ratings = _ratingRepository.GetRatingsByMember(_currentMember!.Account);
        var result = new List<(Book, int)>();

        foreach (var rating in ratings)
        {
            if (rating.Score != 0)
            {
                var book = _bookRepository.GetBookByISBN(rating.BookISBN);
                if (book != null)
                {
                    result.Add((book, rating.Score));
                }
            }
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
}