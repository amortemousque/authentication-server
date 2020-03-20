namespace AuthorizationServer.Infrastructure.Context
{

    public class ApplicationDbSettings : IApplicationDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IApplicationDbSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }

}