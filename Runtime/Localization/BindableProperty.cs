using System;

namespace MVPToolkit.Localization
{
    public class BindableProperty
    {
        private object _value;

        public object Value
        {
            get => _value;
            set
            {
                var oldValue = _value;
                _value = value;
                OnPropertyChange?.Invoke(oldValue, value);
            }
        }

        public void SetWithoutNotify(object value)
        {
            _value = value;
        }

        public event Action<object, object> OnPropertyChange;
    }
}