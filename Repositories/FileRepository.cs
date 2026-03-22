using ITCS_3112_Lab_2_Recommendation.Domain;
namespace ITCS_3112_Lab_2_Recommendation.Repositories;

public class FileRepository
{
    public List<Book> Books { get; private set; } = new();
    public List<Member> Members { get; private set; } = new();

    public void LoadData(string bookPath, string ratingPath)
    {
        var bookLines = File.ReadAllLines(bookPath);
        for (int i = 0; i < bookLines.Length; i++)
        {
            var parts = bookLines[i].Split(',');
            Books.Add(new Book((i + 1).ToString(), parts[1].Trim(), parts[0].Trim(), int.Parse(parts[2].Trim())));
        }

        var ratingLines = File.ReadAllLines(ratingPath);
        for (int i = 0; i < ratingLines.Length; i++)
        {
            var parts = ratingLines[i].Split(' ');
            var member = new Member((i + 1).ToString(), parts[0]);
            for (int j = 1; j < parts.Length; j++)
            {
                if (j - 1 < Books.Count)
                {
                    member.Ratings[Books[j - 1].ISBN] = int.Parse(parts[j]);
                }
            }

            Members.Add(member);
        }
    }

    public void AddBook(string author, string title, int year)
    {
        string nextIsbn = (Books.Count + 1).ToString();
        var newBook = new Book(nextIsbn, title, author, year);
        Books.Add(newBook);

        foreach (var member in Members)
        {
            member.Ratings[nextIsbn] = 0;
        }

    }

    public void AddMember(string name)
    {
        string nextAccount = (Members.Count + 1).ToString();
        var newMember = new Member(nextAccount, name);

        foreach (var book in Books)
        {
            newMember.Ratings[book.ISBN] = 0;
        }

        Members.Add(newMember);
    }
}