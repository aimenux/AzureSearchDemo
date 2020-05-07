namespace Lib.Configuration
{
    public class Settings : ISettings
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string IndexName { get; set; }
    }
}