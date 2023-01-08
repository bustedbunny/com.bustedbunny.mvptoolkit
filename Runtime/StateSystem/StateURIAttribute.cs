using System;

namespace MVPToolkit.StateSystem
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class StateUriAttribute : Attribute
    {
        public readonly string uri;

        public StateUriAttribute(string uri)
        {
            this.uri = uri;
        }
    }
}