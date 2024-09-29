
using PersianDateTimeWPFTools.Tools;
using PersianDateTimeWPFTools.Windows.Controls.Primitives;
using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Controls
{
  public class TextContextMenu : ContextMenu
  {
    private static readonly ResourceAccessor ResourceAccessor = new ResourceAccessor(typeof (TextContextMenu));
    private static readonly CommandBinding _selectAllBinding;
    private static readonly CommandBinding _undoBinding;
    private static readonly CommandBinding _redoBinding;
    private readonly MenuItem _proofingMenuItem;
    public static readonly DependencyProperty UsingTextContextMenuProperty = DependencyProperty.RegisterAttached("UsingTextContextMenu", typeof (bool), typeof (TextContextMenu), new PropertyMetadata((object) false, new PropertyChangedCallback(TextContextMenu.OnUsingTextContextMenuChanged)));

    static TextContextMenu()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (TextContextMenu), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (TextContextMenu)));
      TextContextMenu._selectAllBinding = new CommandBinding((ICommand) ApplicationCommands.SelectAll);
      TextContextMenu._selectAllBinding.PreviewCanExecute += new CanExecuteRoutedEventHandler(TextContextMenu.OnSelectAllPreviewCanExecute);
      TextContextMenu._undoBinding = new CommandBinding((ICommand) ApplicationCommands.Undo);
      TextContextMenu._undoBinding.PreviewCanExecute += new CanExecuteRoutedEventHandler(TextContextMenu.OnUndoRedoPreviewCanExecute);
      TextContextMenu._redoBinding = new CommandBinding((ICommand) ApplicationCommands.Redo);
      TextContextMenu._redoBinding.PreviewCanExecute += new CanExecuteRoutedEventHandler(TextContextMenu.OnUndoRedoPreviewCanExecute);
    }

    public TextContextMenu()
    {
      this._proofingMenuItem = new MenuItem();
      this.Items.Add((object) this._proofingMenuItem);
      this.Items.Add((object) new MenuItem()
      {
        Command = (ICommand) ApplicationCommands.Cut,
        Icon = (object) new SymbolIcon(Symbol.Cut)
      });
      this.Items.Add((object) new MenuItem()
      {
        Command = (ICommand) ApplicationCommands.Copy,
        Icon = (object) new SymbolIcon(Symbol.Copy)
      });
      this.Items.Add((object) new MenuItem()
      {
        Command = (ICommand) ApplicationCommands.Paste,
        Icon = (object) new SymbolIcon(Symbol.Paste)
      });
      this.Items.Add((object) new MenuItem()
      {
        Command = (ICommand) ApplicationCommands.Undo,
        Icon = (object) new SymbolIcon(Symbol.Undo)
      });
      this.Items.Add((object) new MenuItem()
      {
        Command = (ICommand) ApplicationCommands.Redo,
        Icon = (object) new SymbolIcon(Symbol.Redo)
      });
      this.Items.Add((object) new MenuItem()
      {
        Command = (ICommand) ApplicationCommands.SelectAll
      });
    }

    public static bool GetUsingTextContextMenu(Control textControl)
    {
      return (bool) textControl.GetValue(TextContextMenu.UsingTextContextMenuProperty);
    }

    public static void SetUsingTextContextMenu(Control textControl, bool value)
    {
      textControl.SetValue(TextContextMenu.UsingTextContextMenuProperty, (object) value);
    }

    private static void OnUsingTextContextMenuChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      Control control = (Control) d;
      if ((bool) e.NewValue)
      {
        control.CommandBindings.Add(TextContextMenu._selectAllBinding);
        control.CommandBindings.Add(TextContextMenu._undoBinding);
        control.CommandBindings.Add(TextContextMenu._redoBinding);
        control.ContextMenuOpening += new ContextMenuEventHandler(TextContextMenu.OnContextMenuOpening);
      }
      else
      {
        control.CommandBindings.Remove(TextContextMenu._selectAllBinding);
        control.CommandBindings.Remove(TextContextMenu._undoBinding);
        control.CommandBindings.Remove(TextContextMenu._redoBinding);
        control.ContextMenuOpening -= new ContextMenuEventHandler(TextContextMenu.OnContextMenuOpening);
      }
    }

    protected override void OnOpened(RoutedEventArgs e)
    {
      base.OnOpened(e);
      if (!this._proofingMenuItem.IsVisible)
        return;
      this._proofingMenuItem.IsSubmenuOpen = true;
    }

    protected override void OnClosed(RoutedEventArgs e)
    {
      base.OnClosed(e);
      if (this.IsOpen)
        return;
      this._proofingMenuItem.Items.Clear();
      foreach (DependencyObject dependencyObject in (IEnumerable) this.Items)
        dependencyObject.ClearValue(MenuItem.CommandTargetProperty);
    }

    private static void OnSelectAllPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      switch (sender)
      {
        case TextBox textBox when string.IsNullOrEmpty(textBox.Text):
          e.CanExecute = false;
          e.Handled = true;
          break;
        case PasswordBox passwordBox when string.IsNullOrEmpty(passwordBox.Password):
          e.CanExecute = false;
          e.Handled = true;
          break;
      }
    }

    private static void OnUndoRedoPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (!(sender is TextBoxBase textBoxBase) || !textBoxBase.IsReadOnly)
        return;
      e.CanExecute = false;
      e.Handled = true;
    }

    private static void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
      Control control = (Control) sender;
      if (!(control.ContextMenu is TextContextMenu contextMenu))
        return;
      Control target = !(control is PasswordBox passwordBox) || PasswordBoxHelper.GetPasswordRevealMode(passwordBox) != PasswordRevealMode.Visible || !(e.Source is TextBox) ? control : (Control) e.Source;
      contextMenu.UpdateItems(target);
      if (contextMenu.Items.OfType<MenuItem>().Any<MenuItem>((Func<MenuItem, bool>) (mi => mi.Visibility == Visibility.Visible)))
        return;
      e.Handled = true;
    }

    private void UpdateProofingMenuItem(Control target)
    {
      this._proofingMenuItem.Header = (object) TextContextMenu.ResourceAccessor.GetLocalizedStringResource("ProofingMenuItemLabel");
      this._proofingMenuItem.Items.Clear();
      SpellingError spellingError = (SpellingError) null;
      if (target is TextBox textBox)
        spellingError = textBox.GetSpellingError(textBox.CaretIndex);
      else if (target is RichTextBox richTextBox)
        spellingError = richTextBox.GetSpellingError(richTextBox.CaretPosition);
      if (spellingError != null)
      {
        foreach (string suggestion in spellingError.Suggestions)
        {
          MenuItem newItem = new MenuItem();
          newItem.Header = (object) suggestion;
          newItem.Command = (ICommand) EditingCommands.CorrectSpellingError;
          newItem.CommandParameter = (object) suggestion;
          newItem.CommandTarget = (IInputElement) target;
          this._proofingMenuItem.Items.Add((object) newItem);
        }
        if (this._proofingMenuItem.HasItems)
          this._proofingMenuItem.Items.Add((object) new Separator());
        ItemCollection items = this._proofingMenuItem.Items;
        MenuItem newItem1 = new MenuItem();
        newItem1.Header = (object) Strings.IgnoreMenuItemLabel;
        newItem1.Command = (ICommand) EditingCommands.IgnoreSpellingError;
        newItem1.CommandTarget = (IInputElement) target;
        items.Add((object) newItem1);
        this._proofingMenuItem.Visibility = Visibility.Visible;
      }
      else
        this._proofingMenuItem.Visibility = Visibility.Collapsed;
    }

    private void UpdateItems(Control target)
    {
      this.UpdateProofingMenuItem(target);
      foreach (MenuItem menuItem in (IEnumerable) this.Items)
      {
        if (menuItem.Command is RoutedUICommand command)
        {
          if (command == ApplicationCommands.Cut)
            menuItem.Header = (object) TextContextMenu.ResourceAccessor.GetLocalizedStringResource("TextCommandLabelCut");
          else if (command == ApplicationCommands.Copy)
            menuItem.Header = (object) TextContextMenu.ResourceAccessor.GetLocalizedStringResource("TextCommandLabelCopy");
          else if (command == ApplicationCommands.Paste)
            menuItem.Header = (object) TextContextMenu.ResourceAccessor.GetLocalizedStringResource("TextCommandLabelPaste");
          else if (command == ApplicationCommands.Undo)
            menuItem.Header = (object) TextContextMenu.ResourceAccessor.GetLocalizedStringResource("TextCommandLabelUndo");
          else if (command == ApplicationCommands.Redo)
            menuItem.Header = (object) TextContextMenu.ResourceAccessor.GetLocalizedStringResource("TextCommandLabelRedo");
          else if (command == ApplicationCommands.SelectAll)
            menuItem.Header = (object) TextContextMenu.ResourceAccessor.GetLocalizedStringResource("TextCommandLabelSelectAll");
          menuItem.CommandTarget = (IInputElement) target;
          menuItem.Visibility = command.CanExecute((object) null, (IInputElement) target) ? Visibility.Visible : Visibility.Collapsed;
        }
      }
    }
  }
}
