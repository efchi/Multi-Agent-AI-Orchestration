using Agents.Architecture.Domain;

namespace Agents.Architecture.Shared
{
    public class Common
    {
        static readonly Random Random = new();

        const int minMessageId = 0;
        const int maxMessageId = 9999;
        
        public static string NewMessageId(Agent agent, Message message) =>
            $"{agent.Role.Name}:{Random.Next(minMessageId, maxMessageId)}@{message.ContextId}";

        const int minContextId = 0;
        const int maxContextId = 9999;

        public static int NewContextId() =>
            Random.Next(minContextId, maxContextId);
    }
}
