using System;
namespace AuthorizationServer.Domain.Shared
{
    public class PartialUser
    {
        public Guid Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
    }
}
