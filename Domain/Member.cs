namespace ITCS_3112_Lab_2_Recommendation.Domain;

public class Member
{
    public string AccountID { get; set; }
    public string Name { get; set; }
    public Dictionary<string, int> Ratings { get; set; } = new Dictionary<string, int>();
    public Member(string accountID, string name)
    {
        AccountID = accountID;
        Name = name;
    }
}