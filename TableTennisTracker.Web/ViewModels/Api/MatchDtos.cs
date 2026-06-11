using System;
using System.Collections.Generic;

namespace TableTennisTracker.Web.ViewModels.Api
{
    public record MatchSetResultBriefDto(Guid Id, int SetNumber, int PlayerOnePoints, int PlayerTwoPoints);

    public record MatchParticipantBriefDto(Guid Id, Guid PlayerId, int Slot, int ScoreSetsWon, Guid? PlayerReference = null);

    public record MatchDto(Guid Id, Guid TournamentId, int RoundNumber, int TableNumber, DateTime ScheduledStartUtc, DateTime? ActualStartUtc, DateTime? CompletedUtc, int BestOfSets, Guid? WinnerPlayerId, string Status, IEnumerable<MatchParticipantDto> Participants, IEnumerable<MatchSetResultDto> SetResults);

    public record MatchCreateDto(Guid TournamentId, int RoundNumber, int TableNumber, DateTime ScheduledStartUtc, DateTime? ActualStartUtc, DateTime? CompletedUtc, int BestOfSets, Guid? WinnerPlayerId, string Status);

    public record MatchUpdateDto(Guid TournamentId, int RoundNumber, int TableNumber, DateTime ScheduledStartUtc, DateTime? ActualStartUtc, DateTime? CompletedUtc, int BestOfSets, Guid? WinnerPlayerId, string Status);
}
