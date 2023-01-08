using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVPToolkit.Localization
{
    public interface IPropertyProvider
    {
        BindableProperty this[string binding] { get; }
    }

    public class PropertyProvider : IPropertyProvider
    {
        private readonly Dictionary<string, BindableProperty> _properties = new();

        public BindableProperty this[string binding]
        {
            get
            {
                var value = _properties[binding];
                if (value is null) throw new Exception();
                return value;
            }
            set => _properties.Add(binding, value);
        }
    }
}