using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Contracts;

public interface IMemberRepository
{
    void AddMember(Member member);
    Member? GetMemberByAccount(string account);
    Member? GetMemberByName(string name);
    List<Member> GetAllMembers();
}