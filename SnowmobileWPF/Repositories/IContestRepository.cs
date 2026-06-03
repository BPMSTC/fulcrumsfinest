using SnowmobileLibrary.Models;

namespace SnowmobileWPF.Repositories
{
    public interface IContestRepository
    {
        bool CurrentlyInContest { get; }
        bool IsLastContestAcknowledged();

        Contest? GetCurrentContest();

        void Create(DateTime endDate);
        void End();

        void ClearContestEntries();
        void ClearAdContestEntries();
    }
}