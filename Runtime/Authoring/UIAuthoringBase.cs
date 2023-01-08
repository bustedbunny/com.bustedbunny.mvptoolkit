using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace MVPToolkit.Authoring
{
    public abstract class UIAuthoringBase<T> : UIAuthoring where T : BasePresentation
    {
        [SerializeField] private VisualTreeAsset asset;
        public override VisualTreeAsset Asset => asset;

        [SerializeField] private LocalizedStringTable locale;
        public override LocalizedStringTable Locale => locale;
        public override Type Type => typeof(T);
    }

    public abstract class UIAuthoring : MonoBehaviour
    {
        public abstract VisualTreeAsset Asset { get; }
        public abstract Type Type { get; }
        public abstract LocalizedStringTable Locale { get; }
    }
}