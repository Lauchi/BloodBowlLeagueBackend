using System;

namespace Seasons.ReadHost.Matches
{
    public class MinimalMatchHto
    {
        public Guid MatchId { get; }
        public GameResult GameResult { get; }
        public string GuestTeamName { get; }
        public string HomeTeamName { get; }
        public bool IsStarted { get; }

        public MinimalMatchHto(Guid matchId, GameResult gameResult, bool isStarted, string guestTeamName,
            string homeTeamName)
        {
            MatchId = matchId;
            GameResult = gameResult;
            GuestTeamName = guestTeamName;
            HomeTeamName = homeTeamName;
            IsStarted = isStarted;
        }
    }
}