using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Contracts;

public interface IBookRepository
{
    void LoadBooksFromFile(string filename);
    void AddBook(Book book);
    Book? GetBookByISBN(string isbn);
    List<Book> GetAllBooks();
    Book? GetBookByIndex(int index);
}