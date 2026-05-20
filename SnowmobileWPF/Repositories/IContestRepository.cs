using SnowmobileLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SnowmobileWPF.Repositories
{
    public interface IContestRepository
    {
        bool CurrentlyInContest { get; }
        void Create(DateTime endDate);
        void End();
        void ClearContestEntries();
        Contest? GetCurrentContest();
        bool IsLastContestAcknowledged();
    }
}
