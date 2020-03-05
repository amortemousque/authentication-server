using System;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Domain.UserAggregate;

namespace AuthorizationServer.Domain.Contracts
{
    public interface IUserRepository
    {
        Task<IQueryable<IdentityUser>> GetAll();

        Task<bool> HasUniqEmail(string email, Guid tenantId);

        Task<bool> HasUniqPersonId(Guid personId, Guid tenantId);
    }
}
