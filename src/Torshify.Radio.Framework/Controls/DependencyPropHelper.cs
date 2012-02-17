using System;
using System.Windows;

namespace Torshify.Radio.Framework.Controls
{
    public static class DependencyPropHelper
    {
        #region Methods

        public static DependencyProperty Register<TElement, TProperty>(string name, TProperty defaultValue, Action<TElement, TProperty, TProperty> changeHandler = null)
            where TElement : DependencyObject
        {
            PropertyMetadata metadata;
            if (changeHandler == null)
            {
                metadata = new PropertyMetadata(defaultValue);
            }
            else
            {
                metadata = new PropertyMetadata(defaultValue, GetPropertyChangedCallback(changeHandler));
            }

            return DependencyProperty.Register(name, typeof(TProperty), typeof(TElement), metadata);
        }

        public static DependencyProperty Register<TElement, TProperty>(string name, Action<TElement, TProperty, TProperty> changeHandler = null)
            where TElement : DependencyObject
        {
            PropertyMetadata metadata;
            if (changeHandler == null)
            {
                metadata = null;
            }
            else
            {
                metadata = new PropertyMetadata(GetPropertyChangedCallback(changeHandler));
            }

            return DependencyProperty.Register(name, typeof(TProperty), typeof(TElement), metadata);
        }

        public static DependencyProperty RegisterAttached<TOwner, TTarget, TProperty>(string name, Action<TTarget, TProperty, TProperty> changeHandler = null)
            where TTarget : DependencyObject
        {
            PropertyMetadata metadata;
            if (changeHandler == null)
            {
                metadata = null;
            }
            else
            {
                metadata = new PropertyMetadata(GetPropertyChangedCallback(changeHandler));
            }

            return DependencyProperty.RegisterAttached(name, typeof(TProperty), typeof(TOwner), metadata);
        }

        private static PropertyChangedCallback GetPropertyChangedCallback<TElement, TProperty>(Action<TElement, TProperty, TProperty> handler)
            where TElement : DependencyObject
        {
            return (element, args) => handler((TElement)element, (TProperty)args.NewValue, (TProperty)args.OldValue);
        }

        #endregion Methods
    }
}