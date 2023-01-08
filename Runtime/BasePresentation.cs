using MVPToolkit.Localization;
using Unity.Assertions;
using Unity.Entities;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

namespace MVPToolkit
{
    public abstract partial class BasePresentation : SystemBase
    {
        // ReSharper disable once MemberCanBePrivate.Global
        protected VisualElement RootVisualElement { get; private set; }

        protected UISingleton UISingleton { get; private set; }

        protected override void OnCreate()
        {
            Enabled = false;
        }

        protected override void OnUpdate()
        {
        }


        public void Initialize(VisualTreeAsset sourceAsset, UISingleton uiSingleton)
        {
            UISingleton = uiSingleton;

            RootVisualElement = sourceAsset.Instantiate();


            #if UNITY_EDITOR
            RootVisualElement.name = GetType().Name;
            #endif

            // This is done in order to avoid clipping
            RootVisualElement.pickingMode = PickingMode.Ignore;
            RootVisualElement.style.position = new StyleEnum<Position>(Position.Absolute);
            var fullScreen = new StyleLength()
            {
                value = new Length
                {
                    value = 100f,
                    unit = LengthUnit.Percent
                },
            };
            RootVisualElement.style.width = fullScreen;
            RootVisualElement.style.height = fullScreen;
        }

        public virtual void Enable()
        {
            Assert.IsFalse(UISingleton.activePresentations.Contains(this));

            UISingleton.activePresentations.Add(this);

            if (UISingleton.Document is not null)
            {
                AddToPanel(UISingleton.Document.rootVisualElement);
            }
        }

        public virtual void Disable()
        {
            UISingleton.activePresentations.Remove(this);

            if (UISingleton.Document is not null)
            {
                RemoveFromPanel(UISingleton.Document.rootVisualElement);
            }
        }

        public virtual void AddToPanel(VisualElement root)
        {
            if (!root.Contains(RootVisualElement))
                root.Add(RootVisualElement);
        }

        public virtual void RemoveFromPanel(VisualElement root)
        {
            if (root.Contains(RootVisualElement))
                root.Remove(RootVisualElement);
        }


        public virtual void OnInit()
        {
        }

        private LocaleHolder _localeHolder;


        protected IPropertyProvider propertyProvider;
        protected UILocalization UILocalization { get; private set; }

        internal void PostInit(LocaleHolder localization)
        {
            _localeHolder = localization;
            UILocalization =
                new UILocalization(_localeHolder.localizedStringTable, RootVisualElement, propertyProvider);
        }
    }
}