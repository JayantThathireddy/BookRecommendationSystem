using ITCS_3112_Lab_2_Recommendation.Domain;
namespace ITCS_3112_Lab_2_Recommendation.Services;

public class RecommendationService
{
    private int CalculateSimilarity(Member m1, Member m2, List<Book> books)
    {
        int score = 0;
        foreach (var book in books)
        {
            int r1 = m1.Ratings.GetValueOrDefault(book.ISBN, 0);
            int r2 = m2.Ratings.GetValueOrDefault(book.ISBN, 0);
            score += (r1 * r2);
        }

        return score;
    }

    public (Member Match, List<Book> ReallyLiked, List<Book> Liked) GetRecommendations(Member user,
        List<Member> allMembers, List<Book> allBooks)
    {
        Member? bestMatch = null;
        int topScore = int.MinValue;

        foreach (var other in allMembers)
        {
            if (other.AccountID == user.AccountID) continue;

            int currentScore = CalculateSimilarity(user, other, allBooks);
            if (currentScore > topScore)
            {
                topScore = currentScore;
                bestMatch = other;
            }
        }

        var reallyLiked = new List<Book>();
        var liked = new List<Book>();

        if (bestMatch != null)
        {
            foreach (var book in allBooks)
            {
                int userRating = user.Ratings.GetValueOrDefault(book.ISBN, 0);
                int matchRating = bestMatch.Ratings.GetValueOrDefault(book.ISBN, 0);
                if (userRating == 0)
                {
                    if (matchRating == 5) reallyLiked.Add(book);
                    else if (matchRating == 3) liked.Add(book);
                }
            }
        }

        return (bestMatch, reallyLiked, liked);
    }
}
