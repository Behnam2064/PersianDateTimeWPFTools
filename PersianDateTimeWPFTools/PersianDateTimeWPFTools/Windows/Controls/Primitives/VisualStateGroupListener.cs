
using System;
using System.Windows;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{
  public class VisualStateGroupListener : FrameworkElement
  {
    public static readonly DependencyProperty GroupProperty = DependencyProperty.Register(nameof (Group), typeof (VisualStateGroup), typeof (VisualStateGroupListener), new PropertyMetadata(new PropertyChangedCallback(VisualStateGroupListener.OnGroupChanged)));
    private static readonly DependencyPropertyKey CurrentStateNamePropertyKey = DependencyProperty.RegisterReadOnly(nameof (CurrentStateName), typeof (string), typeof (VisualStateGroupListener), (PropertyMetadata) null);
    public static readonly DependencyProperty CurrentStateNameProperty = VisualStateGroupListener.CurrentStateNamePropertyKey.DependencyProperty;
    public static readonly DependencyProperty ListenerProperty = DependencyProperty.RegisterAttached("Listener", typeof (VisualStateGroupListener), typeof (VisualStateGroupListener), new PropertyMetadata(new PropertyChangedCallback(VisualStateGroupListener.OnListenerChanged)));

    static VisualStateGroupListener()
    {
      UIElement.VisibilityProperty.OverrideMetadata(typeof (VisualStateGroupListener), (PropertyMetadata) new FrameworkPropertyMetadata((object) Visibility.Collapsed));
    }

    public VisualStateGroup Group
    {
      get => (VisualStateGroup) this.GetValue(VisualStateGroupListener.GroupProperty);
      set => this.SetValue(VisualStateGroupListener.GroupProperty, (object) value);
    }

    private static void OnGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((VisualStateGroupListener) d).OnGroupChanged((VisualStateGroup) e.OldValue, (VisualStateGroup) e.NewValue);
    }

    private void OnGroupChanged(VisualStateGroup oldGroup, VisualStateGroup newGroup)
    {
      if (oldGroup != null)
        oldGroup.CurrentStateChanged -= new EventHandler<VisualStateChangedEventArgs>(this.OnCurrentStateChanged);
      if (newGroup != null)
        newGroup.CurrentStateChanged += new EventHandler<VisualStateChangedEventArgs>(this.OnCurrentStateChanged);
      this.UpdateCurrentStateName(newGroup?.CurrentState);
    }

    private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
    {
      this.UpdateCurrentStateName(e.NewState);
    }

    public string CurrentStateName
    {
      get => (string) this.GetValue(VisualStateGroupListener.CurrentStateNameProperty);
      private set
      {
        this.SetValue(VisualStateGroupListener.CurrentStateNamePropertyKey, (object) value);
      }
    }

    private void UpdateCurrentStateName(VisualState currentState)
    {
      if (currentState != null)
        this.CurrentStateName = currentState.Name;
      else
        this.ClearValue(VisualStateGroupListener.CurrentStateNamePropertyKey);
    }

    public static VisualStateGroupListener GetListener(VisualStateGroup group)
    {
      return (VisualStateGroupListener) group.GetValue(VisualStateGroupListener.ListenerProperty);
    }

    public static void SetListener(VisualStateGroup group, VisualStateGroupListener value)
    {
      group.SetValue(VisualStateGroupListener.ListenerProperty, (object) value);
    }

    private static void OnListenerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (e.OldValue is VisualStateGroupListener oldValue)
        oldValue.ClearValue(VisualStateGroupListener.GroupProperty);
      if (!(e.NewValue is VisualStateGroupListener newValue))
        return;
      newValue.Group = (VisualStateGroup) d;
    }
  }
}
