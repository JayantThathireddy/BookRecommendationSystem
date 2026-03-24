using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Contracts;

public interface ILibraryService
{
    void LoadData(string booksFile, string ratingsFile);
    bool Login(string accountId);
    void Logout();
    bool IsLoggedIn();
    Member? GetCurrentMember();
    Member? AddMember(string name);
    Book? AddBook(string author, string title, string year);
    void RateBook(string bookISBN, int score);
    int GetRating(string bookISBN);
    List<(Book, int)> GetMyRatings();
    List<Book> GetRecommendedBooks(int count = 5);
    List<Book> GetAllBooks();
    int GetMemberCount();
    (Member? similarMember, List<Book> reallyLiked, List<Book> liked) GetDetailedRecommendations();
}