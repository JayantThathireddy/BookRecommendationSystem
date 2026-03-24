namespace ITCS_3112_Lab_2_Recommendation.Contracts;

public interface IRatingRepository
{
    void LoadRatingsFromFile(string filename, IMemberRepository memberRepository, IBookRepository bookRepository);
    void AddRating(Rating rating);
    List<Rating> GetRatingsByMember(string memberAccount);
    List<Rating> GetAllRatings();
    int GetRating(string memberAccount, string bookISBN);
}