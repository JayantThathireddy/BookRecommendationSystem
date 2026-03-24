namespace ITCS_3112_Lab_2_Recommendation;

public interface ILibraryService
{
    void LoadData(string booksFile, string ratingsFile);
    bool Login(string memberName);
    void Logout();
    bool IsLoggedIn();
    Member? GetCurrentMember();
    Member? AddMember(string name);
    Book? AddBook(string author, string title, string year);
    void RateBook(string bookISBN, int score);
    List<(Book, int)> GetMyRatings();
    List<Book> GetRecommendedBooks(int count = 5);
    List<Book> GetAllBooks();
}