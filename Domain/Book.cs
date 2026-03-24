namespace ITCS_3112_Lab_2_Recommendation.Domain;

public class Book
{
    private static int _nextIsbn = 1;

    public string ISBN { get; }
    public string Author { get; set; }
    public string Title { get; set; }
    public string Year { get; set; }

    public Book(string author, string title, string year)
    {
        ISBN = _nextIsbn++.ToString();
        Author = author;
        Title = title;
        Year = year;
    }

    public Book(string isbn, string author, string title, string year)
    {
        ISBN = isbn;
        Author = author;
        Title = title;
        Year = year;
        if (int.TryParse(isbn, out int num) && num >= _nextIsbn)
        {
            _nextIsbn = num + 1;
        }
    }

    public override string ToString()
    {
        return $"{Author}, {Title} ({Year})";
    }
}