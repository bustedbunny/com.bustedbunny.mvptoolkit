using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MVPToolkit.StateSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVPToolkit.Editor.StateUriWindow
{
    public class StateUriDisplay : EditorWindow
    {
        private static Type[] GetPresentations()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(BasePresentation).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        types.Add(type);
                    }
                }
            }

            return types.ToArray();
        }

        private class TypeOrUrl
        {
            public string typeName;
        }

        private void CreateGUI()
        {
            var types = GetPresentations();
            var source = new List<TreeViewItemData<TypeOrUrl>>(types.Length);

            var j = 0;
            foreach (var type in types)
            {
                {
                    var typeName = type.Name;
                    var urls = StateProcessor.GetTypeUris(type);
                    var group = new TypeOrUrl
                    {
                        typeName = typeName
                    };

                    var treeViewUris = new List<TreeViewItemData<TypeOrUrl>>();
                    foreach (var uri in urls)
                    {
                        treeViewUris.Add(new TreeViewItemData<TypeOrUrl>(j++, new TypeOrUrl
                        {
                            typeName = uri
                        }));
                    }

                    source.Add(new TreeViewItemData<TypeOrUrl>(j++, group, treeViewUris));
                }

                var methods = StateProcessor.GetAllMethods(type);

                foreach (var methodInfo in methods)
                {
                    var attributes = methodInfo.GetCustomAttributes<StateUriAttribute>().ToArray();

                    if (attributes.Length == 0) continue;
                    if (StateProcessor.CheckMethodInfo(methodInfo)) continue;

                    var urls = StateProcessor.GetUris(attributes);

                    var treeViewUrls = new List<TreeViewItemData<TypeOrUrl>>(urls.Length);
                    foreach (var url in urls)
                    {
                        treeViewUrls.Add(new TreeViewItemData<TypeOrUrl>(j++, new TypeOrUrl
                        {
                            typeName = url
                        }));
                    }

                    var methodRecord = new TypeOrUrl { typeName = $"{type.Name}.{methodInfo.Name}" };

                    source.Add(new TreeViewItemData<TypeOrUrl>(j++, methodRecord, treeViewUrls));
                }
            }

            source.Sort((x, y) => string.CompareOrdinal(x.data.typeName, y.data.typeName));

            var treeView = new TreeView();
            treeView.SetRootItems(source);

            treeView.makeItem = () => new Label();
            treeView.bindItem = (element, i) =>
            {
                ((Label)element).text = treeView.GetItemDataForIndex<TypeOrUrl>(i).typeName;
            };

            rootVisualElement.Add(treeView);
        }

        private const string BrowserTitle = "State URI Browser";

        [MenuItem("Tools/" + BrowserTitle)]
        public static void OpenWindow()
        {
            var window = GetWindow<StateUriDisplay>();
            window.titleContent = new GUIContent(BrowserTitle);
        }
    }
}