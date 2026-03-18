using ITCS_3112_Lab_2_Recommendation.services;

namespace ITCS_3112_Lab_2_Recommendation;

class Program
{
    static void Main(string[] args)
    {
        ILibraryService client = new LibraryService();
    }
}