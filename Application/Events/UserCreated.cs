using System;

namespace AuthorizationServer.Application.Events
{
	public class UserCreated
	{
		public Guid PersonId { get; set; }
		public Guid Id { get; set; }

		public UserCreated()
		{
			Id = Guid.NewGuid();
		}
	}
}