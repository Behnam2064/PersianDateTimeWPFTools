using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using PersianDateTimeWPFTools.Tools;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{
    public class PasswordBoxHelper : DependencyObject
    {
        private const string ButtonStatesGroup = "ButtonStates";
        private const string ButtonVisibleState = "ButtonVisible";
        private const string ButtonCollapsedState = "ButtonCollapsed";
        private static readonly CommandBinding TextBoxCutBinding;
        private static readonly CommandBinding TextBoxCopyBinding;
        private readonly PasswordBox _passwordBox;
        private bool _hideRevealButton;
        public static readonly DependencyProperty PasswordRevealModeProperty = DependencyProperty.RegisterAttached(nameof(PasswordRevealMode), typeof(PasswordRevealMode), typeof(PasswordBoxHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)PasswordRevealMode.Peek, new PropertyChangedCallback(PasswordBoxHelper.OnPasswordRevealModeChanged)));
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(PasswordBoxHelper), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(PasswordBoxHelper.OnIsEnabledChanged)));
        private static readonly DependencyPropertyKey PlaceholderTextVisibilityPropertyKey = DependencyProperty.RegisterAttachedReadOnly("PlaceholderTextVisibility", typeof(Visibility), typeof(PasswordBoxHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)Visibility.Visible));
        public static readonly DependencyProperty PlaceholderTextVisibilityProperty = PasswordBoxHelper.PlaceholderTextVisibilityPropertyKey.DependencyProperty;
        private static readonly DependencyProperty HelperInstanceProperty = DependencyProperty.RegisterAttached("HelperInstance", typeof(PasswordBoxHelper), typeof(PasswordBoxHelper), new PropertyMetadata(new PropertyChangedCallback(PasswordBoxHelper.OnHelperInstanceChanged)));

        static PasswordBoxHelper()
        {
            PasswordBoxHelper.TextBoxCutBinding = new CommandBinding((ICommand)ApplicationCommands.Cut);
            PasswordBoxHelper.TextBoxCutBinding.CanExecute += new CanExecuteRoutedEventHandler(PasswordBoxHelper.OnDisabledCommandCanExecute);
            PasswordBoxHelper.TextBoxCopyBinding = new CommandBinding((ICommand)ApplicationCommands.Copy);
            PasswordBoxHelper.TextBoxCopyBinding.CanExecute += new CanExecuteRoutedEventHandler(PasswordBoxHelper.OnDisabledCommandCanExecute);
        }

        public PasswordBoxHelper(PasswordBox passwordBox) => this._passwordBox = passwordBox;

        public static PasswordRevealMode GetPasswordRevealMode(PasswordBox passwordBox)
        {
            return (PasswordRevealMode)passwordBox.GetValue(PasswordBoxHelper.PasswordRevealModeProperty);
        }

        public static void SetPasswordRevealMode(PasswordBox passwordBox, PasswordRevealMode value)
        {
            passwordBox.SetValue(PasswordBoxHelper.PasswordRevealModeProperty, (object)value);
        }

        private static void OnPasswordRevealModeChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            PasswordBoxHelper.GetHelperInstance((PasswordBox)d)?.UpdateVisualState(true);
        }

        public static bool GetIsEnabled(PasswordBox passwordBox)
        {
            return (bool)passwordBox.GetValue(PasswordBoxHelper.IsEnabledProperty);
        }

        public static void SetIsEnabled(PasswordBox passwordBox, bool value)
        {
            passwordBox.SetValue(PasswordBoxHelper.IsEnabledProperty, (object)value);
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)d;
            if ((bool)e.NewValue)
                PasswordBoxHelper.SetHelperInstance(passwordBox, new PasswordBoxHelper(passwordBox));
            else
                passwordBox.ClearValue(PasswordBoxHelper.HelperInstanceProperty);
        }

        public static Visibility GetPlaceholderTextVisibility(Control control)
        {
            return (Visibility)control.GetValue(PasswordBoxHelper.PlaceholderTextVisibilityProperty);
        }

        private static void SetPlaceholderTextVisibility(Control control, Visibility value)
        {
            control.SetValue(PasswordBoxHelper.PlaceholderTextVisibilityPropertyKey, (object)value);
        }

        private static PasswordBoxHelper GetHelperInstance(PasswordBox passwordBox)
        {
            return (PasswordBoxHelper)passwordBox.GetValue(PasswordBoxHelper.HelperInstanceProperty);
        }

        private static void SetHelperInstance(PasswordBox passwordBox, PasswordBoxHelper value)
        {
            passwordBox.SetValue(PasswordBoxHelper.HelperInstanceProperty, (object)value);
        }

        private static void OnHelperInstanceChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is PasswordBoxHelper oldValue)
                oldValue.Detach();
            if (!(e.NewValue is PasswordBoxHelper newValue))
                return;
            newValue.Attach();
        }

        private TextBox TextBox { get; set; }

        private PasswordRevealMode PasswordRevealMode
        {
            get => PasswordBoxHelper.GetPasswordRevealMode(this._passwordBox);
        }

        private static void OnDisabledCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }

        private void Attach()
        {
            this._passwordBox.PasswordChanged += new RoutedEventHandler(this.OnPasswordChanged);
            this._passwordBox.GotFocus += new RoutedEventHandler(this.OnGotFocus);
            this._passwordBox.LostFocus += new RoutedEventHandler(this.OnLostFocus);
            if (this._passwordBox.IsLoaded)
                this.OnApplyTemplate();
            else
                this._passwordBox.Loaded += new RoutedEventHandler(this.OnLoaded);
        }

        private void Detach()
        {
            this._passwordBox.PasswordChanged -= new RoutedEventHandler(this.OnPasswordChanged);
            this._passwordBox.GotFocus -= new RoutedEventHandler(this.OnGotFocus);
            this._passwordBox.LostFocus -= new RoutedEventHandler(this.OnLostFocus);
            this._passwordBox.Loaded -= new RoutedEventHandler(this.OnLoaded);
            if (this.TextBox == null)
                return;
            this.TextBox.CommandBindings.Remove(PasswordBoxHelper.TextBoxCutBinding);
            this.TextBox.CommandBindings.Remove(PasswordBoxHelper.TextBoxCopyBinding);
            this.TextBox.TextChanged -= new TextChangedEventHandler(this.OnTextBoxTextChanged);
            this.TextBox = (TextBox)null;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this._passwordBox.Loaded -= new RoutedEventHandler(this.OnLoaded);
            this.OnApplyTemplate();
        }

        private void OnApplyTemplate()
        {
            this._passwordBox.ApplyTemplate();
            this.TextBox = this._passwordBox.GetTemplateChild<TextBox>("TextBox");
            if (this.TextBox != null)
            {
                this.TextBox.IsUndoEnabled = false;
                SpellCheck.SetIsEnabled((TextBoxBase)this.TextBox, false);
                this.TextBox.CommandBindings.Add(PasswordBoxHelper.TextBoxCutBinding);
                this.TextBox.CommandBindings.Add(PasswordBoxHelper.TextBoxCopyBinding);
                this.TextBox.TextChanged += new TextChangedEventHandler(this.OnTextBoxTextChanged);
                this.UpdateTextBox();
            }
            this.UpdateVisualState(false);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (this.PasswordRevealMode == PasswordRevealMode.Visible && this.TextBox != null && e.OriginalSource == this._passwordBox)
            {
                this.TextBox.Focus();
                e.Handled = true;
            }
            if (!string.IsNullOrEmpty(this._passwordBox.Password))
                this._hideRevealButton = true;
            this.UpdateVisualState(true);
        }

        private void OnLostFocus(object sender, RoutedEventArgs e) => this.UpdateVisualState(true);

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            bool flag = !string.IsNullOrEmpty(this._passwordBox.Password);
            if (!flag)
                this._hideRevealButton = false;
            PasswordBoxHelper.SetPlaceholderTextVisibility((Control)this._passwordBox, flag ? Visibility.Collapsed : Visibility.Visible);
            this.UpdateTextBox();
            this.UpdateVisualState(true);
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.PasswordRevealMode != PasswordRevealMode.Visible)
                return;
            this._passwordBox.Password = ((TextBox)sender).Text;
        }

        private void UpdateTextBox()
        {
            if (this.TextBox == null)
                return;
            this.TextBox.Text = this._passwordBox.Password;
        }

        private void UpdateVisualState(bool useTransitions)
        {
            bool flag = false;
            if (this._passwordBox.IsFocused)
            {
                switch (this.PasswordRevealMode)
                {
                    case PasswordRevealMode.Peek:
                        flag = !this._hideRevealButton && !string.IsNullOrEmpty(this._passwordBox.Password);
                        break;
                    case PasswordRevealMode.Hidden:
                    case PasswordRevealMode.Visible:
                        flag = false;
                        break;
                }
            }
            VisualStateManager.GoToState((FrameworkElement)this._passwordBox, flag ? "ButtonVisible" : "ButtonCollapsed", useTransitions);
        }
    }
}
