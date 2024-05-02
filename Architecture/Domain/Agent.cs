using Agents.Architecture.Logic;
using Agents.Architecture.Shared;

namespace Agents.Architecture.Domain
{
    public abstract class Agent
    {
        public readonly string Name;
        public readonly Role Role;
        public readonly Process Process;
        readonly Thread? Thread;

        public abstract void Initialize();
        public abstract string Consume(Message message);
        
        public Agent(string name, Role role, Process process) =>
            (Name, Role, Process, Thread) = (name, role, process, new Thread(Threading.ThreadLambda.Invoke!) { IsBackground = false });
    
        public void Start()
        {
            if (Thread == default)
                throw new FatalException("Agent :: Start :: Thread was null.");

            Thread.Start(this);
        }
    }

    public sealed class LambdaAgent : Agent
    { 
        readonly Func<Agent, Message, string> Lambda;

        public LambdaAgent(string name, Role role, Process process, Func<Agent, Message, string> lambda) : base(name, role, process) =>
            Lambda = lambda;

        public override void Initialize()
        {
        }

        public override string Consume(Message message) =>
            Lambda(this, message);
    }
}
