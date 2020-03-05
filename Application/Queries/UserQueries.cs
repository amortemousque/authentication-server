using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 
using MongoDB.Driver;
using System.Linq;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.UserAggregate;
using AuthorizationServer.Infrastructure.Context;
using AuthorizationServer.Application.Security;

namespace AuthorizationServer.Application.Queries
{
    public class UserQueries
    {
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly SecurityService _securityService;

        public UserQueries(IUserRepository userRepository, ApplicationDbContext context, SecurityService securityService)
        {
            _userRepository = userRepository;
            _context = context;
            _securityService = securityService;

        }

        public async Task<List<IdentityUser>> GetUsersAsync(string name, string email, List<string> ids, string fields)
        {

            var filterDef = new FilterDefinitionBuilder<IdentityUser>();

            var filter = FilterDefinition<IdentityUser>.Empty;


            if (ids != null && ids.Any())
            {
                filter = filterDef.In(x => x.Id, ids);
            }

            if (!await _securityService.CanReadRoleRoot())
            {
                filter = filter & filterDef.Where(f => !f.Roles.Contains("ROOT"));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {

                filter = filter & filterDef.Eq(x => x.NormalizedFullName, name.ToUpper());
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                filter = filter & filterDef.Eq(x => x.NormalizedEmail, email.ToUpper());
            }

            if (string.IsNullOrEmpty(fields)) 
            {
                fields = "GivenName,FamilyName,Culture,Picture,Email";
            }

            var fs = fields.Split(',');
            var includes = Builders<IdentityUser>.Projection.Include("Id");

            foreach(var f in fs) {
                includes = includes.Include(f.Trim());
            }

            var users = await _context.Users.Find(filter).ToListAsync();

            return users;
        }

    }

}
