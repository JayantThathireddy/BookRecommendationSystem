using ITCS_3112_Lab_2_Recommendation.Contracts;
using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly List<Member> _members = new();

    public void AddMember(Member member)
    {
        _members.Add(member);
    }

    public Member? GetMemberByAccount(string account)
    {
        return _members.FirstOrDefault(m => m.Account == account);
    }

    public Member? GetMemberByName(string name)
    {
        return _members.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public List<Member> GetAllMembers()
    {
        return _members;
    }
}