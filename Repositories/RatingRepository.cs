using ITCS_3112_Lab_2_Recommendation.Contracts;

namespace ITCS_3112_Lab_2_Recommendation.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly List<Rating> _ratings = new();

    public void LoadRatingsFromFile(string filename, IMemberRepository memberRepository, IBookRepository bookRepository)
    {
        string[] lines = File.ReadAllLines(filename);
        for (int i = 0; i < lines.Length; i += 2)
        {
            if (i + 1 >= lines.Length) break;

            string memberName = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(memberName)) continue;

            Member member = new Member(memberName);
            memberRepository.AddMember(member);

            string ratingsLine = lines[i + 1].Trim();
            string[] ratingValues = ratingsLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int bookIndex = 0; bookIndex < ratingValues.Length; bookIndex++)
            {
                if (int.TryParse(ratingValues[bookIndex], out int score))
                {
                    Book? book = bookRepository.GetBookByIndex(bookIndex);
                    if (book != null)
                    {
                        _ratings.Add(new Rating(member.Account, book.ISBN, score));
                    }
                }
            }
        }
    }

    public void AddRating(Rating rating)
    {
        var existingRating = _ratings.FirstOrDefault(r =>
            r.MemberAccount == rating.MemberAccount && r.BookISBN == rating.BookISBN);

        if (existingRating != null)
        {
            existingRating.Score = rating.Score;
        }
        else
        {
            _ratings.Add(rating);
        }
    }

    public List<Rating> GetRatingsByMember(string memberAccount)
    {
        return _ratings.Where(r => r.MemberAccount == memberAccount).ToList();
    }

    public List<Rating> GetAllRatings()
    {
        return _ratings;
    }

    public int GetRating(string memberAccount, string bookISBN)
    {
        var rating = _ratings.FirstOrDefault(r =>
            r.MemberAccount == memberAccount && r.BookISBN == bookISBN);
        return rating?.Score ?? 0;
    }
}