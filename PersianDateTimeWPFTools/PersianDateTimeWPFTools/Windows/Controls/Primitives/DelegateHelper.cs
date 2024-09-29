
using System;
using System.Reflection;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{
    internal static class DelegateHelper
    {
        private const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        public static T CreateDelegate<T>(MethodInfo method) where T : Delegate
        {
            return (T)Delegate.CreateDelegate(typeof(T), method);
        }

        public static T CreateDelegate<T>(object firstArgument, MethodInfo method) where T : Delegate
        {
            return (T)Delegate.CreateDelegate(typeof(T), firstArgument, method);
        }

        public static T CreateDelegate<T>(Type target, string method, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public) where T : Delegate
        {
            if (bindingAttr == (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
                return (T)Delegate.CreateDelegate(typeof(T), target, method);
            MethodInfo method1 = target.GetMethod(method, bindingAttr);
            return method1 != (MethodInfo)null ? DelegateHelper.CreateDelegate<T>(method1) : default(T);
        }

        public static T CreateDelegate<T>(object target, string method, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public) where T : Delegate
        {
            if (bindingAttr == (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
                return (T)Delegate.CreateDelegate(typeof(T), target, method);
            MethodInfo method1 = target.GetType().GetMethod(method, bindingAttr);
            return method1 != (MethodInfo)null ? DelegateHelper.CreateDelegate<T>(target, method1) : default(T);
        }

        public static Func<TType, TProperty> CreatePropertyGetter<TType, TProperty>(
          string name,
          BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public,
          bool nonPublic = false)
        {
            PropertyInfo property = typeof(TType).GetProperty(name, bindingAttr);
            if (property != (PropertyInfo)null)
            {
                MethodInfo getMethod = property.GetGetMethod(nonPublic);
                if (getMethod != (MethodInfo)null)
                    return DelegateHelper.CreateDelegate<Func<TType, TProperty>>(getMethod);
            }
            return (Func<TType, TProperty>)null;
        }

        public static Action<TType, TProperty> CreatePropertySetter<TType, TProperty>(
          string name,
          BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public,
          bool nonPublic = false)
        {
            PropertyInfo property = typeof(TType).GetProperty(name, bindingAttr);
            if (property != (PropertyInfo)null)
            {
                MethodInfo setMethod = property.GetSetMethod(nonPublic);
                if (setMethod != (MethodInfo)null)
                    return DelegateHelper.CreateDelegate<Action<TType, TProperty>>(setMethod);
            }
            return (Action<TType, TProperty>)null;
        }
    }
}
