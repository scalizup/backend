using Application.Repositories;
using MediatR;

namespace Application.UseCases.Auth.Users.Queries;

public static class GetAllUsers
{
    [Authorize(Role = Domain.Constants.UserRoles.Admin)]
    public record Query : IRequest<IEnumerable<UserDto>>;

    public class Handler(
        IUserRepository userRepository) : IRequestHandler<Query, IEnumerable<UserDto>>
    {
        public async Task<IEnumerable<UserDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var allUsers = await userRepository.GetAllUsers();

            return allUsers.Select(u => new UserDto(
                u.Id,
                u.Username,
                u.Roles.Select(r => new RoleDto(r.Id, r.Name)).ToList(),
                u.AvailableTenants.Select(t => new TenantDto(t.Id, t.Name)).ToList())).ToList();
        }
    }

    public record UserDto(
        int Id,
        string Username,
        IReadOnlyList<RoleDto> Roles,
        IReadOnlyList<TenantDto> Tenants);

    public record RoleDto(
        int Id,
        string Name);
    
    public record TenantDto(
        int Id,
        string Name);
}