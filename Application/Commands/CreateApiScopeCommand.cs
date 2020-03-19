using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AuthorizationServer.Domain.ApiAggregate;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class CreateApiScopeCommand : IRequest<ApiScope>
    {
        [Required]
        public Guid ApiResourceId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
