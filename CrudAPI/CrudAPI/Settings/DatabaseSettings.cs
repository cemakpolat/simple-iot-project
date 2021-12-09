namespace CrudAPI.Settings
{
    public interface IDatabaseSettings
    {
        public string ConnectionString { get; set; }

        public string Database { get; set; }
    }

    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }

        public string Database { get; set; }
    }
}