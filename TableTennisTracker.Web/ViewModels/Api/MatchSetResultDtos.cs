using System;

namespace TableTennisTracker.Web.ViewModels.Api
{
    public record MatchSetResultDto(Guid Id, Guid MatchId, int SetNumber, int PlayerOnePoints, int PlayerTwoPoints);

    public record MatchSetResultCreateDto(Guid MatchId, int SetNumber, int PlayerOnePoints, int PlayerTwoPoints);

    public record MatchSetResultUpdateDto(Guid MatchId, int SetNumber, int PlayerOnePoints, int PlayerTwoPoints);
}
