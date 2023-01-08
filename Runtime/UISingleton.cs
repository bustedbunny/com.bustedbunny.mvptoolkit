using System.Collections.Generic;
using MVPToolkit.StateSystem;
using Unity.Entities;
using UnityEngine.UIElements;

namespace MVPToolkit
{
    public class UISingleton : IComponentData
    {
        private UIDocument _document;

        public UIDocument Document
        {
            get => _document;
            set
            {
                if (_document == value) return;

                // If old document is not null we clear it from our presentations
                if (_document is not null)
                {
                    foreach (var activePresentation in activePresentations)
                    {
                        activePresentation.RemoveFromPanel(_document.rootVisualElement);
                    }
                }

                if (value is not null)
                {
                    // If new document is not null we add all our active presentations
                    foreach (var activePresentation in activePresentations)
                    {
                        activePresentation.AddToPanel(value.rootVisualElement);
                    }
                }

                _document = value;
            }
        }

        public List<BasePresentation> presentations = new();
        public List<StateCallback> callbacks = new();


        public HashSet<BasePresentation> activePresentations = new();
        public HashSet<ModalPresentation> activeModals = new();
    }
}