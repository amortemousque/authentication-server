namespace IntegrationEvents.User.Account
{
	public class ResetPasswordRequested : IntegrationEvent
	{
		public string ResetPasswordLink { get; set; }
	}
}
