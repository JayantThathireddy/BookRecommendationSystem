namespace ITCS_3112_Lab_2_Recommendation.services;

public class LibraryService: ILibraryService
{
    public LibraryService()
    {
    }

    public Member AddMember()
    {
        return new Member();
    }

    public Book AddBook()
    {
        return new Book();
    }
}