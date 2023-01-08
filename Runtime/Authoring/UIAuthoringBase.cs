using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVPToolkit.Authoring
{
    public abstract class UIAuthoringBase<T> : MonoBehaviour, IUIAuthoring where T : BasePresentation
    {
        [SerializeField] private VisualTreeAsset asset;
        public VisualTreeAsset Asset => asset;
        public Type Type => typeof(T);
    }

    internal interface IUIAuthoring
    {
        public VisualTreeAsset Asset { get; }
        public Type Type { get; }
    }
}