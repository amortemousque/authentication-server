using System;
using AuthorizationServer.Domain.RoleAggregate;

namespace AuthorizationServer.Domain.AccountInviteAggregate
{
	public class AccountInvite
	{
		public AccountInvite()
		{
			Id = Guid.NewGuid();
		}

		public Guid Id { get; set; }
		public Guid TenantId { get; set; }
		public Guid CreatedBy { get; set; }
		public Guid UpdatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		public Guid InviteeId { get; set; }
		public string Email { get; set; }
		public string Role { get; set; }
		public bool IsAccepted { get; set; }


		public void Accept()
		{
			IsAccepted = true;
		}

		public class Factory
		{
			public static AccountInvite CreateNewEntry(Guid candidateId, Guid tenantId, string email,
				Guid initiatorId, string role)
			{
				if (!IdentityRole.NormalizedNames.Contains(role)) throw new ArgumentException(nameof(role));

				return new AccountInvite { Id = Guid.NewGuid(), InviteeId = candidateId, TenantId = tenantId,
					Email = email, IsAccepted = false, Role = role, CreatedBy = initiatorId, UpdatedBy = initiatorId,
					CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
				};
			}
		}
	}
}