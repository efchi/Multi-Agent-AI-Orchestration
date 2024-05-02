using Agents.Architecture.Shared;

namespace Agents.Architecture.Domain
{
    public class Role
    {
        public readonly string Name;
        public string RoleScript { get; private set; } = string.Empty;
        
        public readonly Queue<Message> Queue;
        public readonly Process Process;

        public Role(string name, Process process) =>
            (Name, Queue, Process) = (name, new(this), process);

        public void SetScript(string script)
        {
            if (RoleScript != string.Empty)
                // We generally don't want to overwrite the script once it's set.
                throw new DomainException("Role :: SetScript :: Role script was already set");

            RoleScript = script;
        }
    }
}
