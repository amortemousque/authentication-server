using System;

namespace IntegrationEvents.User.Account
{
	public class AccountCreated : IntegrationEvent
	{
		public Guid UserId { get; set; }

		public AccountCreated(Guid userId)
		{
			UserId = userId;
		}

		public override string ToString()
		{
			return $"{nameof(AccountCreated)} : {Id}";
		}
	}
}