using System;
using System.Threading.Tasks;
using IntegrationEvents.Directory.Entries;
using IntegrationEvents.Directory.Profile;
using AuthorizationServer.Application.Events;
using AuthorizationServer.Domain.RoleAggregate;
using AuthorizationServer.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using Rebus.Handlers;
using Rebus.Sagas;
using IdentityRole = AuthorizationServer.Domain.RoleAggregate.IdentityRole;
using IdentityUser = AuthorizationServer.Domain.UserAggregate.IdentityUser;

namespace AuthorizationServer.Application.Sagas
{
	public class ProfileSaga : Saga<ProfileModificationSagaData>,
		IAmInitiatedBy<CandidateHired>, IAmInitiatedBy<EmployeeOnboarded>, 
		IAmInitiatedBy<HRManagerOnboarded>, IAmInitiatedBy<DirectorOnboarded>,
		IAmInitiatedBy<EmployeePromotedToManager>, IAmInitiatedBy<EmployeePromotedToHRManager>,
		IAmInitiatedBy<EmployeePromotedToDirector>, IAmInitiatedBy<EmployeeDemotedFromManager>, 
		IAmInitiatedBy<EmployeeDemotedFromHRManager>, IAmInitiatedBy<EmployeeDemotedFromDirector>,
		IAmInitiatedBy<SupportCollaboratorCreated>, IAmInitiatedBy<CandidateCreated>,
		IHandleMessages<UserCreated>
	{
		readonly UserManager<IdentityUser> _userManager;

		public ProfileSaga(UserManager<IdentityUser> userManager)
		{
			_userManager = userManager;
		}

		protected override void CorrelateMessages(ICorrelationConfig<ProfileModificationSagaData> config)
		{
			config.Correlate<CandidateHired>(m => m.CandidateId, d => d.PersonId);
			config.Correlate<EmployeeOnboarded>(m => m.PersonId, d => d.PersonId);
			config.Correlate<HRManagerOnboarded>(m => m.PersonId, d => d.PersonId);
			config.Correlate<DirectorOnboarded>(m => m.PersonId, d => d.PersonId);
			config.Correlate<EmployeePromotedToManager>(m => m.EmployeeId, d => d.PersonId);
			config.Correlate<EmployeePromotedToHRManager>(m => m.EmployeeId, d => d.PersonId);
			config.Correlate<EmployeePromotedToDirector>(m => m.EmployeeId, d => d.PersonId);
			config.Correlate<EmployeeDemotedFromManager>(m => m.EmployeeId, d => d.PersonId);
			config.Correlate<EmployeeDemotedFromHRManager>(m => m.EmployeeId, d => d.PersonId);
			config.Correlate<EmployeeDemotedFromDirector>(m => m.EmployeeId, d => d.PersonId);
			config.Correlate<SupportCollaboratorCreated>(m => m.PersonId, d => d.PersonId);
			config.Correlate<CandidateCreated>(m => m.PersonId, d => d.PersonId);
			config.Correlate<UserCreated>(m => m.PersonId, d => d.PersonId);
		}

		public async Task Handle(CandidateHired message)
		{
			await Handle(message.CandidateId, message.TenantId, IdentityRole.NormalizedNames.Recruit);
		}

		public async Task Handle(EmployeeOnboarded message)
		{
			await Handle(message.PersonId, message.TenantId, IdentityRole.NormalizedNames.Employee);
		}

		public async Task Handle(HRManagerOnboarded message)
		{
			await Handle(message.PersonId, message.TenantId, IdentityRole.NormalizedNames.HRManager);
		}

		public async Task Handle(DirectorOnboarded message)
		{
			await Handle(message.PersonId, message.TenantId, IdentityRole.NormalizedNames.Director);
		}

		public async Task Handle(EmployeePromotedToManager message)
		{
			await Handle(message.EmployeeId, message.TenantId, IdentityRole.NormalizedNames.Manager);
		}

		public async Task Handle(EmployeePromotedToHRManager message)
		{
			await Handle(message.EmployeeId, message.TenantId, IdentityRole.NormalizedNames.HRManager);
		}

		public async Task Handle(EmployeePromotedToDirector message)
		{
			await Handle(message.EmployeeId, message.TenantId, IdentityRole.NormalizedNames.Director);
		}

		public async Task Handle(EmployeeDemotedFromManager message)
		{
			await HandleDemotion(message.EmployeeId, message.TenantId, IdentityRole.NormalizedNames.Manager);
		}

		public async Task Handle(EmployeeDemotedFromHRManager message)
		{
			await HandleDemotion(message.EmployeeId, message.TenantId, IdentityRole.NormalizedNames.HRManager);
		}

		public async Task Handle(EmployeeDemotedFromDirector message)
		{
			await HandleDemotion(message.EmployeeId, message.TenantId, IdentityRole.NormalizedNames.Director);
		}

		public async Task Handle(CandidateCreated message)
		{
			await HandleExclusiveRole(message.PersonId, message.TenantId, IdentityRole.NormalizedNames.Candidate);
		}

		public async Task Handle(SupportCollaboratorCreated message)
		{
			await HandleExclusiveRole(message.PersonId, message.TenantId, IdentityRole.NormalizedNames.SupportCollaborator);
		}

		public async Task Handle(UserCreated message)
		{
			var user = await _userManager.FindByIdAsync(message.PersonId.ToString());
			if (user == null) throw new ArgumentException($"The user for person '{message.PersonId}' was not found");

			await _userManager.AddToRolesAsync(user, Data.Roles);
			MarkAsComplete();
		}

		private async Task HandleDemotion(Guid personId, Guid tenantId, string role)
		{
			Data.PersonId = personId;
			Data.TenantId = tenantId;
			Data.RemoveRole(role);

			var user = await _userManager.FindByIdAsync(personId.ToString());
			if (user == null) return;

			user.RemoveRole(role);
			await _userManager.UpdateAsync(user);
			MarkAsComplete();
		}

		private async Task HandleExclusiveRole(Guid personId, Guid tenantId, string role)
		{
			Data.PersonId = personId;
			Data.TenantId = tenantId;
			Data.EmptyRoles();
			Data.AddRole(role);

			await TryUpdateUser(personId);
		}

		private async Task Handle(Guid personId, Guid tenantId, string role)
		{
			Data.PersonId = personId;
			Data.TenantId = tenantId;
			Data.AddRole(role);
			if (role != IdentityRole.NormalizedNames.Recruit) Data.RemoveRole(IdentityRole.NormalizedNames.Recruit);

			await TryUpdateUser(personId);
		}

		private async Task TryUpdateUser(Guid personId)
		{
			var user = await _userManager.FindByIdAsync(personId.ToString());
			if (user == null) return;

			await _userManager.AddToRolesAsync(user, Data.Roles);
			MarkAsComplete();
		}
	}
}