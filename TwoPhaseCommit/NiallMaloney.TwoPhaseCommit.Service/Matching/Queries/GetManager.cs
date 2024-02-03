using MediatR;
using NiallMaloney.TwoPhaseCommit.Cassandra;
using NiallMaloney.TwoPhaseCommit.Cassandra.Matching;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Queries;

public record GetManager(string MatchingId) : IRequest<MatchingManagerRow?>;
