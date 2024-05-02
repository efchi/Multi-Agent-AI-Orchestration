using Agents.Architecture.Domain;
using Agents.Architecture.Shared;

namespace Agents.Architecture.Logic
{
    public static class Threading
    {
        public static readonly Action<object> ThreadLambda = parameter =>
        {
            var agent = parameter as Agent ??
                throw new FatalException("Agent :: Fatal Error :: Agent was null.");

            Output.WriteInfoLine($"Agent :: Agent {agent.Name} of role {agent.Role.Name} started.");

            while (true)
            {
                // Wait for a message from the Role queue.
                var message = agent.Role.Queue.WaitDequeue();

                if (message == default)
                    throw new FatalException("Agent :: Fatal Error :: Dequeued Message was null.");

                var messageId = Common.NewMessageId(agent, message);
                Output.WriteEventLine($"Agent :: Agent {agent.Name} processing message {messageId}: {message.Content}");

                // Process the message.
                try
                {
                    var reaction = agent.Consume(message);
                    Output.WriteResponseLine($"Agent :: Agent {agent.Name} responded to ({messageId}) with reaction: {reaction}");

                    // Process the reaction.
                    try
                    {
                        agent.Process.Machine.Run(agent, message.ContextId, reaction);
                    }
                    catch (Exception ex)
                    {
                        Output.WriteErrorLine($"Agent :: Agent {agent.Name} failed to process reaction for {messageId}: {ex.Message}. Content was: {message.Content}");
                    }
                }
                catch (Exception ex)
                { 
                    Output.WriteErrorLine($"Agent :: Agent {agent.Name} failed to process message {messageId}: {ex.Message}.");
                }               
            }
        };
    }
}
