using Unity.Collections;
using Unity.Entities;

namespace MVPToolkit.StateSystem
{
    public struct StateRequest : IComponentData
    {
        public StateType type;
        public FixedString128Bytes uri;
    }

    public enum StateType
    {
        /// <summary>
        /// Closes all UI before processing callbacks
        /// </summary>
        Strong,

        /// <summary>
        /// Simply processing callbacks
        /// </summary>
        Soft
    }
}