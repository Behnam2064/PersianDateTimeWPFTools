﻿using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Tools
{
  public class ThemeResouceExtensionConverter : TypeConverter
  {
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof (InstanceDescriptor) || base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      if (!(destinationType == typeof (InstanceDescriptor)))
        return base.ConvertTo(context, culture, value, destinationType);
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return value is ThemeResourceExtension resourceExtension ? (object) new InstanceDescriptor((MemberInfo) typeof (ThemeResourceExtension).GetConstructor(new Type[1]
      {
        typeof (object)
      }), (ICollection) new object[1]
      {
        resourceExtension.ResourceKey
      }) : throw new ArgumentException(string.Format("{0} must be of type {1}", value, (object) "ThemeResourceExtension"), nameof (value));
    }
  }
}
