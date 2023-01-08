using System;
using System.Linq;
using Unity;
using Unity.Collections;
using Unity.Entities;

namespace MVPToolkit.StateSystem
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UIReactSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<UISingleton>();
            RequireForUpdate<StateRequest>();
        }

        private UISingleton _ui;

        protected override void OnUpdate()
        {
            if (_ui is null)
            {
                var q = SystemAPI.QueryBuilder().WithAll<UISingleton>().Build();
                _ui = EntityManager.GetComponentObject<UISingleton>(q.GetSingletonEntity());
            }

            using var requests = SystemAPI.QueryBuilder().WithAll<StateRequest>().Build()
                .ToComponentDataArray<StateRequest>(Allocator.Temp);

            foreach (var request in requests)
            {
                var uri = request.uri.ToString();

                if (request.type is StateType.Strong)
                {
                    foreach (var presentation in _ui.presentations)
                    {
                        presentation.Disable();
                    }
                }

                foreach (var callback in _ui.callbacks)
                {
                    try
                    {
                        if (callback.uris.Contains(uri))
                        {
                            callback.action.Invoke();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }


            EntityManager.DestroyEntity(SystemAPI.QueryBuilder().WithAll<StateRequest>().Build());
        }
    }
}