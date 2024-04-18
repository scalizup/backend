namespace Application.UseCases.Auth.Users.Queries;

public static class GetMe
{
    public record Query : BasePermissionRequest, IRequest<MeDto>;

    public class Handler : IRequestHandler<Query, MeDto>
    {
        public Task<MeDto> Handle(Query request, CancellationToken cancellationToken)
        {
            return Task.FromResult<MeDto>(new(
                request.UserAccessor.User.Username,
                new(
                    request.UserAccessor.RefreshToken.Tenant.Id,
                    request.UserAccessor.RefreshToken.Tenant.Name)));
        }
    }

    public record MeDto(
        string? Username,
        TenantDto Tenant);

    public record TenantDto(
        int Id,
        string Name);
}