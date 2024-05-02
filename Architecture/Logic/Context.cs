using System.Collections.Concurrent;

namespace Agents.Architecture.Logic
{
    public class Context : ConcurrentDictionary<string, object>
    {
    }
}
