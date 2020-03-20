using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.UserAggregate;
using AuthorizationServer.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using IdentityUser = AuthorizationServer.Domain.UserAggregate.IdentityUser;

namespace AuthorizationServer.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository, IUserStore<IdentityUser>,
            IUserPasswordStore<IdentityUser>,
            IUserRoleStore<IdentityUser>,
            IUserLoginStore<IdentityUser>,
            IUserSecurityStampStore<IdentityUser>,
            IUserEmailStore<IdentityUser>,
            IUserClaimStore<IdentityUser>,
            IUserPhoneNumberStore<IdentityUser>,
            IUserTwoFactorStore<IdentityUser>,
            IUserLockoutStore<IdentityUser>,
            IQueryableUserStore<IdentityUser>,
            IUserAuthenticationTokenStore<IdentityUser>,
            IUserAuthenticatorKeyStore<IdentityUser>
    {
        private readonly ApplicationDbContext _context;
        private readonly IIdentityService _identityService;

        public UserRepository(ApplicationDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }

        public async Task<IQueryable<IdentityUser>> GetAll()
        {
            return _context.Users.AsQueryable();
        }

        public virtual void Dispose()
        {
            // no need to dispose of anything, mongodb handles connection pooling automatically
        }

        public virtual async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken token)
        {
            user.TenantId = user.TenantId == Guid.Empty ? _identityService.GetTenantIdentity() : user.TenantId;
            user.FullName = user.GivenName + " " + user.FamilyName;
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            user.NormalizedFullName = user.FullName.ToUpper();

            await _context.Users.InsertOneAsync(user, cancellationToken: token);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken token)
        {
            user.FullName = user.GivenName + " " + user.FamilyName;
            user.NormalizedFullName = user.FullName.ToUpper();
            user.UpdatedAt = DateTime.Now;
            // todo should add an optimistic concurrency check
            await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: token);
            // todo success based on replace result
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken token)
        {
            var result = await _context.Users.DeleteOneAsync(u => u.Id == user.Id, token);
            // todo success based on delete result
            return result.DeletedCount > 0 ? IdentityResult.Success : IdentityResult.Failed();
        }

        public virtual async Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
            => user.Id;

        public virtual async Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
            => user.UserName;

        public virtual async Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
            => user.UserName = userName;

        // note: again this isn't used by Identity framework so no way to integration test it
        public virtual async Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
            => user.NormalizedUserName;

        public virtual async Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedUserName, CancellationToken cancellationToken)
            => user.NormalizedUserName = normalizedUserName;

        public virtual Task<IdentityUser> FindByIdAsync(string userId, CancellationToken token)
            => _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync(token);

        public virtual Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken token)
            // todo low priority exception on duplicates? or better to enforce unique index to ensure this
            => _context.Users.Find(u => u.NormalizedUserName == normalizedUserName && u.TenantId == _identityService.GetTenantIdentity()).FirstOrDefaultAsync(token);

        public virtual async Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken token)
            => user.PasswordHash = passwordHash;

        public virtual async Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken token)
            => user.PasswordHash;

        public virtual async Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken token)
            => user.HasPassword();

        public virtual async Task AddToRoleAsync(IdentityUser user, string normalizedRoleName, CancellationToken token)
            => user.AddRole(normalizedRoleName);

        public virtual async Task RemoveFromRoleAsync(IdentityUser user, string normalizedRoleName, CancellationToken token)
            => user.RemoveRole(normalizedRoleName);

        // todo might have issue, I'm just storing Normalized only now, so I'm returning normalized here instead of not normalized.
        // EF provider returns not noramlized here
        // however, the rest of the API uses normalized (add/remove/isinrole) so maybe this approach is better anyways
        // note: could always map normalized to not if people complain
        public virtual async Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken token)
            => user.Roles;

        public virtual async Task<bool> IsInRoleAsync(IdentityUser user, string normalizedRoleName, CancellationToken token)
            => user.Roles.Contains(normalizedRoleName);

        public virtual async Task<IList<IdentityUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken token)
            => await _context.Users.Find(u => u.Roles.Contains(normalizedRoleName))
                .ToListAsync(token);

        public virtual async Task AddLoginAsync(IdentityUser user, UserLoginInfo login, CancellationToken token)
            => user.AddLogin(login);

        public virtual async Task RemoveLoginAsync(IdentityUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
            => user.RemoveLogin(loginProvider, providerKey);

        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user, CancellationToken token)
            => user.Logins
                .Select(l => l.ToUserLoginInfo())
                .ToList();

        public virtual Task<IdentityUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
            => _context.Users
                .Find(u => u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey))
                .FirstOrDefaultAsync(cancellationToken);

        public virtual async Task SetSecurityStampAsync(IdentityUser user, string stamp, CancellationToken token)
            => user.SecurityStamp = stamp;

        public virtual async Task<string> GetSecurityStampAsync(IdentityUser user, CancellationToken token)
            => user.SecurityStamp;

        public virtual async Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken token)
            => user.EmailConfirmed;

        public virtual async Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken token)
            => user.EmailConfirmed = confirmed;

        public virtual async Task SetEmailAsync(IdentityUser user, string email, CancellationToken token)
            => user.Email = email;

        public virtual async Task<string> GetEmailAsync(IdentityUser user, CancellationToken token)
            => user.Email;

        // note: no way to intergation test as this isn't used by Identity framework    
        public virtual async Task<string> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
            => user.NormalizedEmail;

        public virtual async Task SetNormalizedEmailAsync(IdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
            => user.NormalizedEmail = normalizedEmail;

        public virtual Task<IdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken token)
        {
            // note: I don't like that this now searches on normalized email :(... why not FindByNormalizedEmailAsync then?
            // todo low - what if a user can have multiple accounts with the same email?
            return _context.Users.Find(u => u.NormalizedEmail == normalizedEmail && u.TenantId == _identityService.GetTenantIdentity()).FirstOrDefaultAsync(token);
        }

        public virtual async Task<IList<Claim>> GetClaimsAsync(IdentityUser user, CancellationToken token)
            => user.Claims.Select(c => c.ToSecurityClaim()).ToList();

        public virtual Task AddClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken token)
        {
            foreach (var claim in claims)
            {
                user.AddClaim(claim);
            }
            return Task.FromResult(0);
        }

        public virtual Task RemoveClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken token)
        {
            foreach (var claim in claims)
            {
                user.RemoveClaim(claim);
            }
            return Task.FromResult(0);
        }

        public virtual async Task ReplaceClaimAsync(IdentityUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.ReplaceClaim(claim, newClaim);
        }

        public virtual Task SetPhoneNumberAsync(IdentityUser user, string phoneNumber, CancellationToken token)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public virtual Task<string> GetPhoneNumberAsync(IdentityUser user, CancellationToken token)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public virtual Task<bool> GetPhoneNumberConfirmedAsync(IdentityUser user, CancellationToken token)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public virtual Task SetPhoneNumberConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken token)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public virtual Task SetTwoFactorEnabledAsync(IdentityUser user, bool enabled, CancellationToken token)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public virtual Task<bool> GetTwoFactorEnabledAsync(IdentityUser user, CancellationToken token)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public virtual async Task<IList<IdentityUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _context.Users
                .Find(u => u.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                .ToListAsync(cancellationToken);
        }

        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(IdentityUser user, CancellationToken token)
        {
            DateTimeOffset? dateTimeOffset = user.LockoutEndDateUtc;
            return Task.FromResult(dateTimeOffset);
        }

        public virtual Task SetLockoutEndDateAsync(IdentityUser user, DateTimeOffset? lockoutEnd, CancellationToken token)
        {
            user.LockoutEndDateUtc = lockoutEnd?.UtcDateTime;
            return Task.FromResult(0);
        }

        public virtual Task<int> IncrementAccessFailedCountAsync(IdentityUser user, CancellationToken token)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public virtual Task ResetAccessFailedCountAsync(IdentityUser user, CancellationToken token)
        {
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public virtual async Task<int> GetAccessFailedCountAsync(IdentityUser user, CancellationToken token)
            => user.AccessFailedCount;

        public virtual async Task<bool> GetLockoutEnabledAsync(IdentityUser user, CancellationToken token)
            => user.LockoutEnabled;

        public virtual async Task SetLockoutEnabledAsync(IdentityUser user, bool enabled, CancellationToken token)
            => user.LockoutEnabled = enabled;

        public virtual IQueryable<IdentityUser> Users => _context.Users.AsQueryable();

        public virtual async Task SetTokenAsync(IdentityUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
            => user.SetToken(loginProvider, name, value);

        public virtual async Task RemoveTokenAsync(IdentityUser user, string loginProvider, string name, CancellationToken cancellationToken)
            => user.RemoveToken(loginProvider, name);

        public virtual async Task<string> GetTokenAsync(IdentityUser user, string loginProvider, string name, CancellationToken cancellationToken)
            => user.GetTokenValue(loginProvider, name);

        public Task SetAuthenticatorKeyAsync(IdentityUser user, string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAuthenticatorKeyAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasUniqEmail(string email)
        {
            return !await _context.Users.AsQueryable().AnyAsync(a =>  a.NormalizedEmail == email.ToUpper() && a.TenantId == _identityService.GetTenantIdentity());
        }
    }
}
