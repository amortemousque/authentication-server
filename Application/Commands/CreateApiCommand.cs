using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AuthorizationServer.Domain.ApiAggregate;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class CreateApiCommand : IRequest<ApiResource>
    {
        [Required]
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
    