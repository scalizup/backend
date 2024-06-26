﻿using Application.Common.Exceptions;
using Application.Repositories;
using Domain.Constants;

namespace Application.UseCases.Tenants.Commands;

public static class DeleteTenant
{
    [Authorize(Role = UserRoles.Admin)]
    public record Command( 
        int Id) : IRequest<bool>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }

    public class Handler(ITenantRepository tenantRepository) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingTenant = await tenantRepository.GetTenantById(request.Id, cancellationToken);
            if (existingTenant is null)
            {
                throw new NotFoundException($"Tenant with id {request.Id} was not found.");
            }
            
            var wasDeleted = await tenantRepository.DeleteTenant(request.Id, cancellationToken);
            
            return wasDeleted;
        }
    }
}