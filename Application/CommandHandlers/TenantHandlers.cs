using System.Threading.Tasks;
using MediatR;
using System;
using System.Threading;
using AuthorizationServer.Application.Commands;
using System.Collections.Generic;
using Rebus.Bus;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.TenantAggregate;
using AuthorizationServer.Infrastructure.Context;
using AuthorizationServer.Infrastructure;
using AuthorizationServer.Exceptions;

namespace AuthorizationServer.Application.CommandHandlers
{
    public class TenantHandlers :
    IRequestHandler<CreateTenantCommand, Tenant>,
    IRequestHandler<UpdateTenantCommand, bool>,
    IRequestHandler<DeleteTenantCommand, bool>

    {
        private readonly ITenantRepository _repository;
        private readonly IBus _bus;
        private readonly IIdentityService _identityService;

        public TenantHandlers(ITenantRepository repository, IBus bus, IIdentityService identityService)
        {
            _repository = repository;
            _bus = bus;
            _identityService = identityService;
        }

        public async Task<Tenant> Handle(CreateTenantCommand message, CancellationToken cancellationToken)
        {
            var tenant = await Tenant.Factory.CreateNewEntry(_repository, message.Name, message.DisplayName, message.Description);

            tenant.CreatedAt = DateTime.Now;
            tenant.CreatedBy = _identityService.GetUserIdentity();
            await _repository.Add(tenant);

            return tenant;
        }

        public async Task<bool> Handle(UpdateTenantCommand message, CancellationToken cancellationToken)
        {
            var tenant = await _repository.GetById(message.Id) ?? throw new NotFoundException();

            tenant.UpdateInfos(message.Description, message.LogoUri, message.EmailSignature);
            if (message.Enabled) 
            {
                tenant.Enable();
            } 
            else 
            {
                tenant.Disable();
            }

            tenant.CreatedAt = DateTime.Now;
            tenant.UpdatedAt = DateTime.Now;
            tenant.CreatedBy = tenant.CreatedBy == Guid.Empty ? _identityService.GetUserIdentity() : tenant.CreatedBy;
            tenant.UpdatedBy = _identityService.GetUserIdentity();

            await _repository.SaveAsync(tenant);
            return true;
        }

        public async Task<bool> Handle(DeleteTenantCommand message, CancellationToken cancellationToken)
        {
            var tenant = await _repository.GetById(message.Id) ?? throw new NotFoundException();
            tenant.Archive();

            tenant.UpdatedAt = DateTime.Now;
            tenant.UpdatedBy = _identityService.GetUserIdentity();
            await _repository.SaveAsync(tenant);

            return true;

        }
    }
}        