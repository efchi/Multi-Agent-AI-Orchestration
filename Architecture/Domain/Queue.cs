using System.Collections.Concurrent;

namespace Agents.Architecture.Domain
{
    public class Queue<T>
    {
        public readonly Role Role;
        readonly BlockingCollection<T> Items = new();

        public Queue(Role role) =>
            Role = role;

        public void Enqueue(T message) =>
            Items.Add(message);

        public T WaitDequeue() =>
            Items.Take();
    }
}
