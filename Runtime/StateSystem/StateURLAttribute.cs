using System;

namespace MVPToolkit.StateSystem
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class StateURLAttribute : Attribute
    {
        public readonly string uri;

        public StateURLAttribute(string uri)
        {
            this.uri = uri;
        }
    }
}