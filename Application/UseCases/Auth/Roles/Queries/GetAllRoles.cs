using Application.Repositories;
using MediatR;

namespace Application.UseCases.Auth.Roles.Queries;

public static class GetAllRoles
{
    [Authorize(Roles = [Domain.Constants.UserRoles.Admin, Domain.Constants.UserRoles.Moderator])]
    public record Query : IRequest<IEnumerable<Dto>>;

    public class Handler(
        IRoleRepository roleRepository) : IRequestHandler<Query, IEnumerable<Dto>>
    {
        public async Task<IEnumerable<Dto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var allRoles = await roleRepository.GetAllRoles(cancellationToken);

            return allRoles.Select(r => new Dto(r.Id, r.Name));
        }
    }
    
    public record Dto(
        int Id,
        string Name);
}