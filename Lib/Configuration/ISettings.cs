using System.Collections.Generic;

namespace Lib.Configuration
{
    public interface ISettings
    {
        string Name { get; }
        string ApiKey { get; }
        string IndexName { get; }
    }
}
