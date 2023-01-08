using System;
using System.Collections.Generic;
using MVPToolkit.Localization;
using MVPToolkit.StateSystem;
using Unity.Entities;
using UnityEngine.UIElements;
using Debug = Unity.Debug;
using Object = UnityEngine.Object;

namespace MVPToolkit
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UIInitializeSystem : SystemBase
    {
        private struct UIInstantiated : IComponentData
        {
        }

        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder().WithAll<UIDataContainer>().WithNone<UIInstantiated>().Build();
            RequireForUpdate(query);
        }


        private UISingleton _uiSingleton;
        private LocaleHolder _localizationHolder;

        private void EnsureUIDocument()
        {
            if (_uiSingleton is not null) return;
            _uiSingleton = new UISingleton
            {
                Document = Object.FindObjectOfType<UIDocument>()
            };

            _localizationHolder = Object.FindObjectOfType<LocaleHolder>();

            var e = EntityManager.CreateSingleton<UISingleton>();
            EntityManager.AddComponentObject(e, _uiSingleton);
        }

        protected override void OnUpdate()
        {
            EnsureUIDocument();

            var entities = new Dictionary<Entity, BasePresentation>();
            foreach (var (uiData, e) in SystemAPI.Query<UIDataContainer>()
                         .WithNone<UIInstantiated>()
                         .WithEntityAccess())
            {
                var type = Type.GetType(uiData.assemblyQualifiedType);
                try
                {
                    var presentation = (BasePresentation)World.GetExistingSystemManaged(type);
                    presentation.Initialize(uiData.source, _uiSingleton);

                    entities.Add(e, presentation);

                    _uiSingleton.presentations.Add(presentation);
                    _uiSingleton.callbacks.AddRange(StateProcessor.Parse(presentation));
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }


            foreach (var pair in entities)
            {
                try
                {
                    #if UNITY_EDITOR
                    EntityManager.SetName(pair.Key, pair.Value.GetType().Name);
                    #endif

                    pair.Value.OnInit();
                    pair.Value.PostInit(_localizationHolder);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                EntityManager.AddComponent<UIInstantiated>(pair.Key);
            }
        }
    }
}