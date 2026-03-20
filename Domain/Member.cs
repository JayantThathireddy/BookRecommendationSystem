namespace ITCS_3112_Lab_2_Recommendation;

public class Member
{
    public string AccountID { get; set; }
    public string Name { get; set; }

    public Member(string accountID, string name)
    {
        AccountID = accountID;
        Name = name;
    }
}