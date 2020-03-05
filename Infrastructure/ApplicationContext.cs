using System;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.Shared;
using AuthorizationServer.Infrastructure.IdentityServer.Extensions;

namespace AuthorizationServer.Infrastructure.Context
{
    public class ApplicationContext : IApplicationContext
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly ITenantRepository _tenantRepository;


        private PartialUser _user;
        private PartialTenant _tenant;
        private string _clientId { get; set; }


        public ApplicationContext(IHttpContextAccessor accessor, ITenantRepository tenantRepository)
        {
            _accessor = accessor;
            _tenantRepository = tenantRepository;
        }


        public string ClientId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_clientId) && _accessor.HttpContext.User.HasClaim(c => c.Type == "client_id"))
                {
                    _clientId = _accessor.HttpContext.User.FindFirst(c => c.Type == "client_id").Value;
                }
                return _clientId;
            }
        }

        public PartialUser User
        {
            get
            {

                if (_user == null && _accessor.HttpContext.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                {
                    _user = new PartialUser
                    {
                        Id = Guid.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value),
                        GivenName = _accessor.HttpContext.User.FindFirst(ClaimTypes.GivenName).Value,
                        FamilyName = _accessor.HttpContext.User.FindFirst(ClaimTypes.Surname).Value
                    };
                    if (_accessor.HttpContext.User.HasClaim(c => c.Type == "employeeId"))
                    {
                        var employeeId = _accessor.HttpContext.User.FindFirst(c => c.Type == "employeeId").Value;
                        _user.EmployeeId = !string.IsNullOrEmpty(employeeId) ? Guid.Parse(employeeId) : (Guid?)null;
                    }
                }
                return _user;
            }
            set {
                _user = value;
            }
        }
        public PartialTenant Tenant
        {
            get
            {
                if (_tenant == null)
                {
                    // if tenant id is in body request (ex: user creation)
                    var tenantId = _accessor.HttpContext?.Request.GetTenantIdFromBody();

                    if (tenantId != null) 
                    {
                        var tenant = _tenantRepository.GetById(tenantId.Value).Result;
                        _tenant = new PartialTenant()
                        {
                            Id = tenantId.Value,
                            Name = tenant.Name
                        };
                    }
                    // if user connected then get tenant from token
                    else if ((_accessor.HttpContext?.User.Identity.IsAuthenticated??false) && _accessor.HttpContext.User.HasClaim(c => c.Type == "tenant_id"))
                    {
                        _tenant = new PartialTenant()
                        {
                            Id = Guid.Parse(_accessor.HttpContext.User.FindFirst(c => c.Type == "tenant_id").Value),
                            Name = _accessor.HttpContext.User.FindFirst(c => c.Type == "tenant_name").Value                        
                        };
                    }
                    // if user disconnected then get tenant from host subdomain
                    else
                    {
                        var tenantName = _accessor.HttpContext?.Request.GetTenantNameFromHost();
                        var tenant = _tenantRepository.GetByName(tenantName).GetAwaiter().GetResult();
                        _tenant = new PartialTenant()
                        {
                            Id = tenant?.Id ?? Guid.Empty,
                            Name = tenantName
                        };
                    }
                }
                return _tenant;
            }
            set
            {
                _tenant = value;
            }
        }
    }
}
