using System.Threading.Tasks;
using MediatR;
using System;
using System.Threading;
using AuthorizationServer.Application.Commands;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Rebus.Bus;
using IntegrationEvents.User.Account;
using AuthorizationServer.Application.Events;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.Services;
using AuthorizationServer.Application.Security;
using AuthorizationServer.Exceptions;
using IdentityUser = AuthorizationServer.Domain.UserAggregate.IdentityUser;
using AuthorizationServer.Infrastructure;

namespace AuthorizationServer.Application.CommandHandlers
{
    public class UserHandlers :
    IRequestHandler<CreateUserCommand, IdentityUser>,
    IRequestHandler<UpdateUserCommand, bool>,
    IRequestHandler<UpdateUserPasswordCommand, bool>,
    IRequestHandler<DeleteUserCommand, bool>,
	IRequestHandler<ResetUserPasswordCommand, IdentityResult>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly SecurityService _securityService;
        private readonly IBus _bus;
        private readonly IEmailSender _emailSender;
        private readonly IIdentityService _identityService;
		

        public UserHandlers(UserManager<IdentityUser> userManager, IUserRepository userRepository,
            SecurityService securityService, IEmailSender emailSender, IBus bus, IIdentityService identityService)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _bus = bus;
            _identityService = identityService;
            _emailSender = emailSender;
            _securityService = securityService;
        }

        public async Task<IdentityUser> Handle(CreateUserCommand message, CancellationToken cancellationToken)
        {
            if (!await _securityService.CanCreateUser()) throw new ForbbidenException();


            if (!await _userRepository.HasUniqEmail(message.Email))
	            throw new ConflictException("Another user has the same email.");

            var user = await IdentityUser.Factory.CreateNewEntry(
                message.TenantId, //root user can create user anywhere
                message.Email,
                message.GivenName,
                message.FamilyName,
                message.Culture,
                message.Picture,
                message.EmailConfirmed
            );

            // Edit user password
            IdentityResult result = string.IsNullOrWhiteSpace(message.Password) ? await _userManager.CreateAsync(user) : await _userManager.CreateAsync(user, message.Password);
            if (!result.Succeeded)
            {
                if (result.Errors.First().Code == "DuplicateUserName") 
                    throw new ConflictException(message.Email);
                else if(result.Errors.First().Code == "PasswordRequiresDigit") 
                    throw new ArgumentException(result.Errors.First().Description, nameof(message.Password));
                else 
                    throw new ArgumentException(result.Errors.First().Description);
            }

            // Send email confirmation
            if(!message.EmailConfirmed) 
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _emailSender.SendEmailForUserCreation(user, code);
            }

            await _bus.Publish(new UserCreated {PersonId = Guid.Parse(user.Id)});

            return user;
        }

        public async Task<bool> Handle(UpdateUserCommand message, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(message.Id.ToString());
            if (!await _securityService.CanUpdateUser(message.Id, user.TenantId)) throw new ForbbidenException();

            user.ChangeName(message.GivenName, message.FamilyName);

            if (message.EmailConfirmed)
            {
                user.ConfirmEmail();
            }

            if (message.Email.Trim().ToUpper() != user.NormalizedEmail)
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, message.Email);
                await _userManager.ChangeEmailAsync(user, message.Email, token);
            }

            if (!string.IsNullOrWhiteSpace(message.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, message.Password);
                if (!result.Succeeded)
                {
                    if(result.Errors.First().Code == "PasswordRequiresDigit") 
                        throw new ArgumentException(result.Errors.First().Description, nameof(message.Password));
                    throw new ArgumentException(result.Errors.First().Description);
                }
            }
            

            return true;
        }

		public async Task<bool> Handle(DeleteUserCommand message, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(message.Id.ToString());
			if (user == null) throw new NotFoundException(message.Id);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new ArgumentException(result.Errors.First().Description, nameof(user));

            return true;

        }

        public async Task<bool> Handle(UpdateUserPasswordCommand message, CancellationToken cancellationToken)
        {
            if (message.Mode == "reset")
            {
                var user = await _userManager.FindByIdAsync(message.Id.ToString());
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                     throw new DomainException("The user must have confirm mail");
                

                //For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                //Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _emailSender.SendEmailResetPassword(user, code);
            }
            return true;
        }

        public async Task<IdentityResult> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(request.Email) ?? throw new NotFoundException(request.Email);
			var result = await _userManager.ResetPasswordAsync(user, request.Code, request.Password);
			if(result.Succeeded) await _bus.Publish(new PasswordReset { InitiatorId = Guid.Parse(user.Id), TenantId = user.TenantId, TenantName = _identityService.GetTenantName() });
			return result;
		}
    }
}