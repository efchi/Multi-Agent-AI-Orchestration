namespace Agents.Architecture.Shared
{
    public class PlatformException : Exception
    {
        public PlatformException(string message) : base(message) { }
    }

    public class UsageException : PlatformException
    {
        public UsageException(string message) : base(message) { }
    }

    public class DomainException : PlatformException
    {
        public DomainException(string message) : base(message) { }
    }

    public class ContextException : PlatformException
    {
        public ContextException(string message) : base(message) { }
    }

    public class SyntaxException : PlatformException
    {
        public SyntaxException(string message) : base(message) { }
    }

    public class ConcurrencyException : PlatformException
    {
        public ConcurrencyException(string message) : base(message) { }
    }

    public class FatalException : PlatformException
    {
        public FatalException(string message) : base(message) { }
    }
}
