using System;
using Unity.Entities;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace MVPToolkit.Authoring
{
    [RequireComponent(typeof(IUIAuthoring))]
    public class PresentationAuthoring : MonoBehaviour
    {
    }

    public class PresentationBaker : Baker<PresentationAuthoring>
    {
        public override void Bake(PresentationAuthoring authoring)
        {
            AddComponentObject(MakeContainer(authoring));
        }

        public static UIDataContainer MakeContainer(MonoBehaviour obj)
        {
            var comp = (IUIAuthoring)obj.GetComponent(typeof(IUIAuthoring));
            Debug.Assert(comp != null, nameof(comp) + " != null");

            var asset = comp.Asset;
            var type = comp.Type;

            if (asset is null)
            {
                throw new NullReferenceException();
            }

            return new UIDataContainer
            {
                source = asset,
                assemblyQualifiedType = type.AssemblyQualifiedName
            };
        }
    }
}