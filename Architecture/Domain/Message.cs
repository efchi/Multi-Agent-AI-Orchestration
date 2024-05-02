namespace Agents.Architecture.Domain
{
    public class Message
    {
        public int? ContextId;
        public string Content = string.Empty;

        public Message(int? contextId, string content) =>
            (ContextId, Content) = (contextId, content);
    }
}
