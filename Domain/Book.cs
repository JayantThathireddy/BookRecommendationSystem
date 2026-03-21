namespace ITCS_3112_Lab_2_Recommendation.Domain;

public class Book
{
    public string ISBN { get; set; }
    public string Author { get; set; }
    public string Title { get; set; }
    public string Year { get; set; }

    public Book(string isbn, string author, string title, string year)
    {
        ISBN = isbn;
        Author = author;
        Title = title;
        Year = year;
    }
    public override string ToString() => $"{Title} by {Author} ({Year})";
}