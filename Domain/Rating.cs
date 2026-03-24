namespace ITCS_3112_Lab_2_Recommendation;

public class Rating
{
    public string MemberAccount { get; set; }
    public string BookISBN { get; set; }
    public int Score { get; set; }

    public Rating(string memberAccount, string bookISBN, int score)
    {
        MemberAccount = memberAccount;
        BookISBN = bookISBN;
        Score = score;
    }

    public static string GetRatingMeaning(int score)
    {
        return score switch
        {
            -5 => "😡 Hated it!",
            -3 => "🙁 Didn't like it",
            0 => "🤷 Haven't read it",
            1 => "😐 Ok – neither hot nor cold",
            3 => "🙂 Liked it!",
            5 => "🤩 Really liked it!",
            _ => "Unknown"
        };
    }
}