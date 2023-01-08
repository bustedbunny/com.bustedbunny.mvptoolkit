using System;
using Unity.Entities;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace MVPToolkit
{
    [Serializable]
    public class UIDataContainer : IComponentData
    {
        public VisualTreeAsset source;
        public string assemblyQualifiedType;
    }
}