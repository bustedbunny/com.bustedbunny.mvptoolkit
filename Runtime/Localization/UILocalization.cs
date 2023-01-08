using System;
using System.Collections.Generic;
using Unity;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace MVPToolkit.Localization
{
    public class UILocalization : IDisposable
    {
        private readonly VisualElement _rootVisualElement;
        private readonly LocalizedStringTable _stringTable;
        private StringTable _currentTable;

        private readonly IPropertyProvider _propertyProvider;
        private readonly Dictionary<BindableTextElement, string> _boundMap = new();

        public UILocalization(LocalizedStringTable stringTable, VisualElement rootVisualElement,
            IPropertyProvider propertyProvider)
        {
            _propertyProvider = propertyProvider;
            _rootVisualElement = rootVisualElement;

            UpdateBindings();

            _stringTable = stringTable;
            _stringTable.TableChanged += OnStringTableChanged;
            OnStringTableChanged();
        }

        public void Dispose()
        {
            if (_stringTable != null)
            {
                _stringTable.TableChanged -= OnStringTableChanged;
            }
        }

        private void UpdateBindings()
        {
            foreach (var (binding, _) in _boundMap)
            {
                binding.Dispose();
            }

            UpdateBindings(_rootVisualElement);
        }

        private void UpdateBindings(VisualElement element)
        {
            var elementHierarchy = element.hierarchy;
            var childCount = elementHierarchy.childCount;
            for (var i = 0; i < childCount; i++)
            {
                if (elementHierarchy[i] is TextElement textElement)
                {
                    if (!GetKey(textElement, out var key))
                    {
                        continue;
                    }

                    var bindableElement = BindTextElement(textElement, _propertyProvider);
                    _boundMap.Add(bindableElement, key);
                }
            }

            for (var i = 0; i < childCount; i++)
            {
                UpdateBindings(elementHierarchy[i]);
            }
        }

        private static bool GetKey(TextElement element, out string key)
        {
            key = element.text;
            if (string.IsNullOrEmpty(key) || key[0] != '#')
            {
                return false;
            }

            key = key.Substring(1, key.Length - 1);
            return true;
        }

        private static BindableTextElement BindTextElement(TextElement element, IPropertyProvider propertyProvider)
        {
            if (element.bindingPath is not null && propertyProvider is not null)
            {
                // Binding properties are separated by dot
                var bindings = element.bindingPath.Split('.');

                var boundProperties = new BindableProperty[bindings.Length];
                for (int j = 0; j < bindings.Length; j++)
                {
                    boundProperties[j] = propertyProvider[bindings[j]];
                }

                var boundText = new BindableTextElement(element, boundProperties);
                return boundText;
            }

            return new BindableTextElement(element);
        }

        private void OnStringTableChanged(StringTable value)
        {
            OnStringTableChanged();
        }

        private void OnStringTableChanged()
        {
            var op = _stringTable.GetTableAsync();
            op.Completed -= OnTableLoaded;
            op.Completed += OnTableLoaded;
        }

        private void OnTableLoaded(AsyncOperationHandle<StringTable> op)
        {
            _currentTable = op.Result;
            LocalizeAll();
        }

        public void Rebind(TextElement textElement, string newKey)
        {
            foreach (var (bind, _) in _boundMap)
            {
                if (textElement == bind.TextElement)
                {
                    _boundMap[bind] = newKey;
                    Localize(bind, newKey);
                    _rootVisualElement.MarkDirtyRepaint();
                    return;
                }
            }
        }

        private void Localize(BindableTextElement bind, string key)
        {
            var entry = _currentTable[key];
            if (entry == null)
            {
                Debug.LogWarning($"No translation in {_currentTable.LocaleIdentifier} for key: {key}");
                bind.Format = $"#{key}";
            }
            else
            {
                var str = entry.LocalizedValue;
                bind.Format = str;
            }
        }

        private void LocalizeAll()
        {
            foreach (var (bind, key) in _boundMap)
            {
                Localize(bind, key);
            }

            _rootVisualElement.MarkDirtyRepaint();
        }
    }
}