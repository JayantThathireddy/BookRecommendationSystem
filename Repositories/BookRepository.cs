using ITCS_3112_Lab_2_Recommendation.Contracts;

namespace ITCS_3112_Lab_2_Recommendation.Repositories;

public class BookRepository : IBookRepository
{
    private readonly List<Book> _books = new();

    public void LoadBooksFromFile(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');
            if (parts.Length >= 3)
            {
                string author = parts[0].Trim();
                string title = parts[1].Trim();
                string year = parts[2].Trim();
                _books.Add(new Book(author, title, year));
            }
        }
    }

    public void AddBook(Book book)
    {
        _books.Add(book);
    }

    public Book? GetBookByISBN(string isbn)
    {
        return _books.FirstOrDefault(b => b.ISBN == isbn);
    }

    public List<Book> GetAllBooks()
    {
        return _books;
    }

    public Book? GetBookByIndex(int index)
    {
        if (index >= 0 && index < _books.Count)
            return _books[index];
        return null;
    }
}