using System;
using System.Windows;
using System.Windows.Controls;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls
{
  internal static class VisualStates
  {
    public const string StateCalendarButtonUnfocused = "CalendarButtonUnfocused";
    public const string StateCalendarButtonFocused = "CalendarButtonFocused";
    public const string GroupCalendarButtonFocus = "CalendarButtonFocusStates";
    public const string StateNormal = "Normal";
    public const string StateMouseOver = "MouseOver";
    public const string StatePressed = "Pressed";
    public const string StateDisabled = "Disabled";
    public const string GroupCommon = "CommonStates";
    public const string StateUnfocused = "Unfocused";
    public const string StateFocused = "Focused";
    public const string GroupFocus = "FocusStates";
    public const string StateSelected = "Selected";
    public const string StateUnselected = "Unselected";
    public const string GroupSelection = "SelectionStates";
    public const string StateActive = "Active";
    public const string StateInactive = "Inactive";
    public const string GroupActive = "ActiveStates";
    public const string StateValid = "Valid";
    public const string StateInvalidFocused = "InvalidFocused";
    public const string StateInvalidUnfocused = "InvalidUnfocused";
    public const string GroupValidation = "ValidationStates";
    public const string StateUnwatermarked = "Unwatermarked";
    public const string StateWatermarked = "Watermarked";
    public const string GroupWatermark = "WatermarkStates";

    public static void GoToState(Control control, bool useTransitions, params string[] stateNames)
    {
      if (control == null)
        throw new ArgumentNullException(nameof (control));
      if (stateNames == null)
        return;
      foreach (string stateName in stateNames)
      {
        if (VisualStateManager.GoToState((FrameworkElement) control, stateName, useTransitions))
          break;
      }
    }
  }
}
