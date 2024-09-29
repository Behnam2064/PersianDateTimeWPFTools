
using System;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls
{
  public class DatePickerDateValidationErrorEventArgs : EventArgs
  {
    private bool _throwException;

    public DatePickerDateValidationErrorEventArgs(Exception exception, string text)
    {
      this.Text = text;
      this.Exception = exception;
    }

    public Exception Exception { get; private set; }

    public string Text { get; private set; }

    public bool ThrowException
    {
      get => this._throwException;
      set => this._throwException = value;
    }
  }
}
