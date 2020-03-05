using System.ComponentModel.DataAnnotations;
using AuthorizationServer.Domain.TenantAggregate;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class CreateTenantCommand : IRequest<Tenant>
    {
        [Required]
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}