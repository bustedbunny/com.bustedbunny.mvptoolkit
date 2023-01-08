using System;
using MVPToolkit.Authoring;
using MVPToolkit.StateSystem;
using Unity.Entities;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Debug = Unity.Debug;
using Object = UnityEngine.Object;

namespace MVPToolkit
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UIInitializationSystem : SystemBase
    {
        private UISingleton _uiSingleton;

        protected override void OnCreate()
        {
            _uiSingleton = new UISingleton
            {
                Document = Object.FindObjectOfType<UIDocument>()
            };

            var e = EntityManager.CreateSingleton<UISingleton>();
            EntityManager.AddComponentObject(e, _uiSingleton);
        }

        private void EnsureDocumentIsLoaded()
        {
            if (_uiSingleton.Document is not null) return;

            var doc = Object.FindObjectOfType<UIDocument>();
            if (doc is null)
                ThrowDocNotFound();

            _uiSingleton.Document = doc;
        }

        private static void ThrowDocNotFound()
        {
            throw new NullReferenceException(
                "Couldn't load UIDocument. Ensure there is a valid UIDocument in a scene.");
        }

        protected override void OnStartRunning()
        {
            Enabled = false;

            EnsureDocumentIsLoaded();

            var array = Object.FindObjectsOfType(typeof(UIAuthoring));

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (UIAuthoring authoring in array)
            {
                try
                {
                    var presentation = (BasePresentation)World.GetExistingSystemManaged(authoring.Type);
                    presentation.Initialize(authoring.Asset, _uiSingleton);

                    _uiSingleton.presentations.Add(presentation);
                    _uiSingleton.callbacks.AddRange(StateProcessor.Parse(presentation));

                    presentation.OnInit();
                    presentation.PostInit(authoring.Locale);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        protected override void OnUpdate()
        {
        }
    }
}