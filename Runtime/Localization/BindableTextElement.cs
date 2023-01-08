using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.UIElements;

namespace MVPToolkit.Localization
{
    public class BindableTextElement : IDisposable
    {
        private readonly TextElement _text;
        private readonly BindableProperty[] _binds;
        private readonly Action<object, object>[] _boundActions;

        private readonly object[] _return;

        private string _format;

        public TextElement TextElement => _text;

        public BindableTextElement([NotNull] TextElement text, params BindableProperty[] binds)
        {
            _text = text;
            _binds = binds;
            _return = new object[binds.Length];
            _boundActions = new Action<object, object>[binds.Length];

            for (int i = 0; i < binds.Length; i++)
            {
                var property = binds[i];
                // Assign value if it was assigned before
                _return[i] = property.Value;

                var index = i;

                var action = new Action<object, object>((oldValue, newValue) =>
                {
                    if (oldValue != newValue)
                    {
                        _return[index] = newValue;
                        UpdateText();
                    }
                });
                _boundActions[i] = action;

                property.OnPropertyChange += action;
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < _binds.Length; i++)
            {
                var property = _binds[i];
                var action = _boundActions[i];
                property.OnPropertyChange -= action;
            }
        }

        public string Format
        {
            set
            {
                _format = value;
                UpdateText();
            }
        }

        private string FormattedString => string.Format(_format, _return);

        private void UpdateText()
        {
            _text.text = FormattedString;
        }
    }
}