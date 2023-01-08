using Unity.Collections;
using Unity.Entities;

namespace MVPToolkit.StateSystem
{
    public static class StateUtility
    {
        public static void SetStateRequest(this EntityManager em, StateRequest request)
        {
            var e = em.CreateEntity();
            em.AddComponentData(e, request);
        }

        public static void SetStateRequest(this EntityManager em, StateType type, FixedString128Bytes uri)
        {
            var e = em.CreateEntity();
            em.AddComponentData(e, new StateRequest
            {
                type = type,
                uri = uri
            });
        }
    }
}