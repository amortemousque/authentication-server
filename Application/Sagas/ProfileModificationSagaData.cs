using System;
using System.Collections.Generic;
using Rebus.Sagas;

namespace AuthorizationServer.Application.Sagas
{
	public class ProfileModificationSagaData : ISagaData
	{
		public Guid Id { get; set; }
		public int Revision { get; set; }
		public Guid PersonId { get; set; }
		public List<string> Roles { get; set; }
		public Guid TenantId { get; set; }

		public ProfileModificationSagaData()
		{
			Roles = new List<string>();
		}

		public void AddRole(string role)
		{
			if (!Roles.Contains(role)) Roles.Add(role);
		}

		public void EmptyRoles()
		{
			Roles.Clear();
		}

		public void RemoveRole(string role)
		{
			if (Roles.Contains(role)) Roles.Remove(role);
		}

		public bool HasRole(string role)
		{
			return Roles.Contains(role);
		}
	}
}