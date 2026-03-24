namespace ITCS_3112_Lab_2_Recommendation.Domain;

public class Member
{
    private static int _nextAccount = 1000;

    public string Account { get; }
    public string Name { get; set; }

    public Member(string name)
    {
        Account = _nextAccount++.ToString();
        Name = name;
    }

    public Member(string account, string name)
    {
        Account = account;
        Name = name;
        if (int.TryParse(account, out int num) && num >= _nextAccount)
        {
            _nextAccount = num + 1;
        }
    }

    public override string ToString()
    {
        return $"{Name} (Account: {Account})";
    }
}