namespace ITCS_3112_Lab_2_Recommendation.Domain;

public class Rating
{
    public string MemberID { get; set; }
    public string ISBN { get; set; }
    public int Score { get; set; }
    
    public Rating(string memberId, string isbn, int score)
    {
        MemberID = memberId;
        ISBN = isbn;
        Score = score;
    }
}