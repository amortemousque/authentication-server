using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationServer.Application.Commands
{
	public class ResetUserPasswordCommand : IRequest<IdentityResult>
	{
		public string Email { get; set; }
		public string Code { get; set; }
		public string Password { get; set; }
	}
}