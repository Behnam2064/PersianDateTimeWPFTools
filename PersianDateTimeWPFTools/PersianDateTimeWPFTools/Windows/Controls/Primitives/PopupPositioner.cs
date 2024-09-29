using MS.Internal;
using MS.Win32;
using PersianDateTimeWPFTools.Windows.Controls.Primitives;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{
    internal class PopupPositioner : DependencyObject, IDisposable
    {
        internal const double Tolerance = 0.01;
        private PopupPositioner.PositionInfo _positionInfo;
        private FrameworkElement _popupRoot;
        private PopupPositioner.PopupSecurityHelper _secHelper;
        private static readonly DependencyProperty PositionerProperty = DependencyProperty.RegisterAttached("Positioner", typeof(PopupPositioner), typeof(PopupPositioner), new PropertyMetadata(new PropertyChangedCallback(PopupPositioner.OnPositionerChanged)));
        private readonly Popup _popup;
        private bool _isDisposed;

        static PopupPositioner()
        {
            PopupPositioner.IsSupported = PopupPositioner.Delegates.GetPlacementInternal != null && PopupPositioner.Delegates.GetDropOpposite != null && PopupPositioner.Delegates.GetPlacementTargetInterestPoints != null && PopupPositioner.Delegates.GetChildInterestPoints != null && PopupPositioner.Delegates.GetScreenBounds != null;
        }

        public PopupPositioner(Popup popup)
        {
            if (!PopupPositioner.IsSupported)
                throw new NotSupportedException();
            this._popup = popup;
            this._secHelper = new PopupPositioner.PopupSecurityHelper();
            PopupPositioner.SetPositioner(popup, this);
            popup.Opened += new EventHandler(this.OnPopupOpened);
            popup.Closed += new EventHandler(this.OnPopupClosed);
            if (!popup.IsOpen)
                return;
            this.OnPopupOpened((object)null, (EventArgs)null);
        }

        public void Dispose()
        {
            if (this._isDisposed)
                return;
            this._isDisposed = true;
            if (this._popup != null)
            {
                this._popup.Opened -= new EventHandler(this.OnPopupOpened);
                this._popup.Closed -= new EventHandler(this.OnPopupClosed);
                this._popup.ClearValue(PopupPositioner.PositionerProperty);
            }
            this.OnPopupClosed((object)null, (EventArgs)null);
        }

        public static bool IsSupported { get; }

        public bool IsOpen => this._popup.IsOpen;

        public PlacementMode Placement => this._popup.Placement;

        internal PlacementMode PlacementInternal
        {
            get => PopupPositioner.Delegates.GetPlacementInternal(this._popup);
        }

        public CustomPopupPlacementCallback CustomPopupPlacementCallback
        {
            get => this._popup.CustomPopupPlacementCallback;
        }

        public double HorizontalOffset => this._popup.HorizontalOffset;

        public double VerticalOffset => this._popup.VerticalOffset;

        internal bool DropOpposite => PopupPositioner.Delegates.GetDropOpposite(this._popup);

        private void OnWindowResize(object sender, AutoResizedEventArgs e)
        {
            if (this._positionInfo == null || !(e.Size != this._positionInfo.ChildSize))
                return;
            this._positionInfo.ChildSize = e.Size;
            this.Reposition();
        }

        internal void Reposition()
        {
            if (!this.IsOpen || !this._secHelper.IsWindowAlive())
                return;
            if (this.CheckAccess())
                this.UpdatePosition();
            else
            {

                this.Dispatcher.Invoke(() =>
                {
                    this.Reposition();
                });
                /*this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate)( =>
                {
                    this.Reposition();
                    return (object)null;
                }), (object)null);*/
            }
            
        }

        private void UpdatePosition()
        {
            if (this._popupRoot == null)
                return;
            PlacementMode placementInternal = this.PlacementInternal;
            Point[] targetInterestPoints = this.GetPlacementTargetInterestPoints(placementInternal);
            Point[] childInterestPoints = this.GetChildInterestPoints(placementInternal);
            Rect bounds = this.GetBounds(targetInterestPoints);
            Rect rect1 = this.GetBounds(childInterestPoints);
            double num1 = rect1.Width * rect1.Height;
            Rect windowRect = this._secHelper.GetWindowRect();
            if (this._positionInfo == null)
                this._positionInfo = new PopupPositioner.PositionInfo();
            this._positionInfo.X = (int)windowRect.X;
            this._positionInfo.Y = (int)windowRect.Y;
            this._positionInfo.ChildSize = windowRect.Size;
            Vector offsetVector1 = new Vector((double)this._positionInfo.X, (double)this._positionInfo.Y);
            double num2 = -1.0;
            CustomPopupPlacement[] customPopupPlacementArray = (CustomPopupPlacement[])null;
            int num3;
            if (placementInternal == PlacementMode.Custom)
            {
                CustomPopupPlacementCallback placementCallback = this.CustomPopupPlacementCallback;
                if (placementCallback != null)
                    customPopupPlacementArray = placementCallback(rect1.Size, bounds.Size, new Point(this.HorizontalOffset, this.VerticalOffset));
                num3 = customPopupPlacementArray == null ? 0 : customPopupPlacementArray.Length;
                if (!this.IsOpen)
                    return;
            }
            else
                num3 = PopupPositioner.GetNumberOfCombinations(placementInternal);
            for (int i = 0; i < num3; ++i)
            {
                Vector offsetVector2;
                PopupPrimaryAxis axis;
                if (placementInternal == PlacementMode.Custom)
                {
                    offsetVector2 = (Vector)targetInterestPoints[0] + (Vector)customPopupPlacementArray[i].Point;
                    axis = customPopupPlacementArray[i].PrimaryAxis;
                }
                else
                {
                    PopupPositioner.PointCombination pointCombination = this.GetPointCombination(placementInternal, i, out axis);
                    InterestPoint targetInterestPoint = pointCombination.TargetInterestPoint;
                    InterestPoint childInterestPoint = pointCombination.ChildInterestPoint;
                    offsetVector2 = targetInterestPoints[(int)targetInterestPoint] - childInterestPoints[(int)childInterestPoint];
                }
                Rect rect2 = Rect.Offset(rect1, offsetVector2);
                Rect rect3 = Rect.Intersect(this.GetScreenBounds(bounds, targetInterestPoints[0]), rect2);
                double num4 = rect3 != Rect.Empty ? rect3.Width * rect3.Height : 0.0;
                if (num4 - num2 > 0.01)
                {
                    offsetVector1 = offsetVector2;
                    num2 = num4;
                    if (Math.Abs(num4 - num1) < 0.01)
                        break;
                }
            }
            Matrix transformToDevice = this._secHelper.GetTransformToDevice();
            rect1 = new Rect((Size)transformToDevice.Transform((Point)GetChildSize()));
            rect1.Offset(offsetVector1);
            Vector offsetVector3 = (Vector)transformToDevice.Transform(GetChildTranslation());
            rect1.Offset(offsetVector3);
            Rect screenBounds = this.GetScreenBounds(bounds, targetInterestPoints[0]);
            Rect rect4 = Rect.Intersect(screenBounds, rect1);
            if (Math.Abs(rect4.Width - rect1.Width) > 0.01 || Math.Abs(rect4.Height - rect1.Height) > 0.01)
            {
                Point point1 = targetInterestPoints[0];
                Vector vector1 = targetInterestPoints[1] - point1;
                vector1.Normalize();
                if (!this.IsTransparent || double.IsNaN(vector1.Y) || Math.Abs(vector1.Y) < 0.01)
                {
                    if (rect1.Right > screenBounds.Right)
                    {
                        offsetVector1.X = screenBounds.Right - rect1.Width;
                        offsetVector1.X -= offsetVector3.X;
                    }
                    else if (rect1.Left < screenBounds.Left)
                    {
                        offsetVector1.X = screenBounds.Left;
                        offsetVector1.X -= offsetVector3.X;
                    }
                }
                else if (this.IsTransparent && Math.Abs(vector1.X) < 0.01)
                {
                    if (rect1.Bottom > screenBounds.Bottom)
                    {
                        offsetVector1.Y = screenBounds.Bottom - rect1.Height;
                        offsetVector1.Y -= offsetVector3.Y;
                    }
                    else if (rect1.Top < screenBounds.Top)
                    {
                        offsetVector1.Y = screenBounds.Top;
                        offsetVector1.Y -= offsetVector3.Y;
                    }
                }
                Point point2 = targetInterestPoints[2];
                Vector vector2 = point1 - point2;
                vector2.Normalize();
                if (!this.IsTransparent || double.IsNaN(vector2.X) || Math.Abs(vector2.X) < 0.01)
                {
                    if (rect1.Bottom > screenBounds.Bottom)
                    {
                        offsetVector1.Y = screenBounds.Bottom - rect1.Height;
                        offsetVector1.Y -= offsetVector3.Y;
                    }
                    else if (rect1.Top < screenBounds.Top)
                    {
                        offsetVector1.Y = screenBounds.Top;
                        offsetVector1.Y -= offsetVector3.Y;
                    }
                }
                else if (this.IsTransparent && Math.Abs(vector2.Y) < 0.01)
                {
                    if (rect1.Right > screenBounds.Right)
                    {
                        offsetVector1.X = screenBounds.Right - rect1.Width;
                        offsetVector1.X -= offsetVector3.X;
                    }
                    else if (rect1.Left < screenBounds.Left)
                    {
                        offsetVector1.X = screenBounds.Left;
                        offsetVector1.X -= offsetVector3.X;
                    }
                }
            }
            int x = DoubleUtil.DoubleToInt(offsetVector1.X);
            int y = DoubleUtil.DoubleToInt(offsetVector1.Y);
            if (x == this._positionInfo.X && y == this._positionInfo.Y)
                return;
            this._positionInfo.X = x;
            this._positionInfo.Y = y;
            this._secHelper.SetPopupPos(true, x, y, false, 0, 0);

            Size GetChildSize()
            {
                UIElement child = this._popup.Child;
                return child != null ? child.RenderSize : this._popupRoot.RenderSize;
            }

            Point GetChildTranslation()
            {
                UIElement child = this._popup.Child;
                return child != null ? child.TranslatePoint(new Point(), (UIElement)this._popupRoot) : new Point();
            }
        }

        private Point[] GetPlacementTargetInterestPoints(PlacementMode placement)
        {
            return PopupPositioner.Delegates.GetPlacementTargetInterestPoints(this._popup, placement);
        }

        private PopupPositioner.PointCombination GetPointCombination(
          PlacementMode placement,
          int i,
          out PopupPrimaryAxis axis)
        {
            bool menuDropAlignment = SystemParameters.MenuDropAlignment;
            switch (placement)
            {
                case PlacementMode.Relative:
                case PlacementMode.AbsolutePoint:
                case PlacementMode.RelativePoint:
                case PlacementMode.MousePoint:
                    axis = PopupPrimaryAxis.Horizontal;
                    if (menuDropAlignment)
                    {
                        switch (i)
                        {
                            case 0:
                                return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
                            case 1:
                                return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.TopLeft);
                            case 2:
                                return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.BottomRight);
                            case 3:
                                return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.BottomLeft);
                        }
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.TopLeft);
                            case 1:
                                return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
                            case 2:
                                return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.BottomLeft);
                            case 3:
                                return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.BottomRight);
                        }
                    }
                    break;
                case PlacementMode.Bottom:
                case PlacementMode.Mouse:
                    axis = PopupPrimaryAxis.Horizontal;
                    if (menuDropAlignment)
                    {
                        if (i == 0)
                            return new PopupPositioner.PointCombination(InterestPoint.BottomRight, InterestPoint.TopRight);
                        if (i == 1)
                            return new PopupPositioner.PointCombination(InterestPoint.TopRight, InterestPoint.BottomRight);
                        break;
                    }
                    if (i == 0)
                        return new PopupPositioner.PointCombination(InterestPoint.BottomLeft, InterestPoint.TopLeft);
                    if (i == 1)
                        return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.BottomLeft);
                    break;
                case PlacementMode.Center:
                    axis = PopupPrimaryAxis.None;
                    return new PopupPositioner.PointCombination(InterestPoint.Center, InterestPoint.Center);
                case PlacementMode.Right:
                case PlacementMode.Left:
                    axis = PopupPrimaryAxis.Vertical;
                    bool flag = menuDropAlignment | this.DropOpposite;
                    if (flag && placement == PlacementMode.Right || !flag && placement == PlacementMode.Left)
                    {
                        switch (i)
                        {
                            case 0:
                                return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
                            case 1:
                                return new PopupPositioner.PointCombination(InterestPoint.BottomLeft, InterestPoint.BottomRight);
                            case 2:
                                return new PopupPositioner.PointCombination(InterestPoint.TopRight, InterestPoint.TopLeft);
                            case 3:
                                return new PopupPositioner.PointCombination(InterestPoint.BottomRight, InterestPoint.BottomLeft);
                        }
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                return new PopupPositioner.PointCombination(InterestPoint.TopRight, InterestPoint.TopLeft);
                            case 1:
                                return new PopupPositioner.PointCombination(InterestPoint.BottomRight, InterestPoint.BottomLeft);
                            case 2:
                                return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
                            case 3:
                                return new PopupPositioner.PointCombination(InterestPoint.BottomLeft, InterestPoint.BottomRight);
                        }
                    }
                    break;
                case PlacementMode.Top:
                    axis = PopupPrimaryAxis.Horizontal;
                    if (menuDropAlignment)
                    {
                        if (i == 0)
                            return new PopupPositioner.PointCombination(InterestPoint.TopRight, InterestPoint.BottomRight);
                        if (i == 1)
                            return new PopupPositioner.PointCombination(InterestPoint.BottomRight, InterestPoint.TopRight);
                        break;
                    }
                    if (i == 0)
                        return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.BottomLeft);
                    if (i == 1)
                        return new PopupPositioner.PointCombination(InterestPoint.BottomLeft, InterestPoint.TopLeft);
                    break;
                default:
                    axis = PopupPrimaryAxis.None;
                    return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.TopLeft);
            }
            return new PopupPositioner.PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
        }

        private Point[] GetChildInterestPoints(PlacementMode placement)
        {
            return PopupPositioner.Delegates.GetChildInterestPoints(this._popup, placement);
        }

        private Rect GetBounds(Point[] interestPoints)
        {
            double num1;
            double x1 = num1 = interestPoints[0].X;
            double num2;
            double y1 = num2 = interestPoints[0].Y;
            for (int index = 1; index < interestPoints.Length; ++index)
            {
                double x2 = interestPoints[index].X;
                double y2 = interestPoints[index].Y;
                if (x2 < x1)
                    x1 = x2;
                if (x2 > num1)
                    num1 = x2;
                if (y2 < y1)
                    y1 = y2;
                if (y2 > num2)
                    num2 = y2;
            }
            return new Rect(x1, y1, num1 - x1, num2 - y1);
        }

        private static int GetNumberOfCombinations(PlacementMode placement)
        {
            switch (placement)
            {
                case PlacementMode.Bottom:
                case PlacementMode.Mouse:
                case PlacementMode.Top:
                    return 2;
                case PlacementMode.Right:
                case PlacementMode.AbsolutePoint:
                case PlacementMode.RelativePoint:
                case PlacementMode.MousePoint:
                case PlacementMode.Left:
                    return 4;
                case PlacementMode.Custom:
                    return 0;
                default:
                    return 1;
            }
        }

        private Rect GetScreenBounds(Rect boundingBox, Point p)
        {
            return PopupPositioner.Delegates.GetScreenBounds(this._popup, boundingBox, p);
        }

        private bool IsTransparent => this._popup.AllowsTransparency;

        internal static PopupPositioner GetPositioner(Popup popup)
        {
            return (PopupPositioner)popup.GetValue(PopupPositioner.PositionerProperty);
        }

        private static void SetPositioner(Popup popup, PopupPositioner value)
        {
            popup.SetValue(PopupPositioner.PositionerProperty, (object)value);
        }

        private static void OnPositionerChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            if (!(e.OldValue is PopupPositioner oldValue))
                return;
            oldValue.Dispose();
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            if (this._secHelper.AttachedToWindow)
                return;
            UIElement child = this._popup.Child;
            if (child == null || !(PresentationSource.FromVisual((Visual)child) is HwndSource window))
                return;
            this._secHelper.AttachToWindow(window, new AutoResizedEventHandler(this.OnWindowResize));
            this._popupRoot = window.RootVisual as FrameworkElement;
            DependencyPropertyDescriptor.FromProperty(Popup.ChildProperty, typeof(Popup)).AddValueChanged((object)this._popup, new EventHandler(this.OnPopupPropertyChanged));
            DependencyPropertyDescriptor.FromProperty(Popup.PlacementProperty, typeof(Popup)).AddValueChanged((object)this._popup, new EventHandler(this.OnPopupPropertyChanged));
            DependencyPropertyDescriptor.FromProperty(Popup.HorizontalOffsetProperty, typeof(Popup)).AddValueChanged((object)this._popup, new EventHandler(this.OnPopupPropertyChanged));
            DependencyPropertyDescriptor.FromProperty(Popup.VerticalOffsetProperty, typeof(Popup)).AddValueChanged((object)this._popup, new EventHandler(this.OnPopupPropertyChanged));
            DependencyPropertyDescriptor.FromProperty(Popup.PlacementRectangleProperty, typeof(Popup)).AddValueChanged((object)this._popup, new EventHandler(this.OnPopupPropertyChanged));
            this.Reposition();
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            if (!this._secHelper.AttachedToWindow)
                return;
            DependencyPropertyDescriptor.FromProperty(Popup.ChildProperty, typeof(Popup)).RemoveValueChanged((object)this._popup, new EventHandler(this.OnPopupPropertyChanged));
            DependencyPropertyDescriptor.FromProperty(Popup.PlacementProperty, typeof(Popup)).RemoveValueChanged((object)this._popup, new EventHandler(this.OnPopupPropertyChanged));
            DependencyPropertyDescriptor.FromProperty(Popup.HorizontalOffsetProperty, typeof(Popup)).RemoveValueChanged((object)this._popup, new EventHandler(this.OnPopupPropertyChanged));
            DependencyPropertyDescriptor.FromProperty(Popup.VerticalOffsetProperty, typeof(Popup)).RemoveValueChanged((object)this._popup, new EventHandler(this.OnPopupPropertyChanged));
            DependencyPropertyDescriptor.FromProperty(Popup.PlacementRectangleProperty, typeof(Popup)).RemoveValueChanged((object)this._popup, new EventHandler(this.OnPopupPropertyChanged));
            this._secHelper.DetachFromWindow(new AutoResizedEventHandler(this.OnWindowResize));
            this._popupRoot = (FrameworkElement)null;
            this._positionInfo = (PopupPositioner.PositionInfo)null;
        }

        private void OnPopupPropertyChanged(object sender, EventArgs e) => this.Reposition();

        private struct PointCombination
        {
            public InterestPoint TargetInterestPoint;
            public InterestPoint ChildInterestPoint;

            public PointCombination(InterestPoint targetInterestPoint, InterestPoint childInterestPoint)
            {
                this.TargetInterestPoint = targetInterestPoint;
                this.ChildInterestPoint = childInterestPoint;
            }
        }

        private class PositionInfo
        {
            public int X;
            public int Y;
            public Size ChildSize;
        }

        private class PopupSecurityHelper
        {
            private HwndSource _window;

            internal PopupSecurityHelper()
            {
            }

            internal bool AttachedToWindow => this._window != null;

            internal void AttachToWindow(HwndSource window, AutoResizedEventHandler handler)
            {
                if (this._window != null)
                    return;
                this._window = window;
                window.AutoResized += handler;
            }

            internal void DetachFromWindow(AutoResizedEventHandler onAutoResizedEventHandler)
            {
                if (this._window == null)
                    return;
                HwndSource window = this._window;
                this._window = (HwndSource)null;
                AutoResizedEventHandler resizedEventHandler = onAutoResizedEventHandler;
                window.AutoResized -= resizedEventHandler;
            }

            internal bool IsWindowAlive()
            {
                if (this._window == null)
                    return false;
                HwndSource window = this._window;
                return window != null && !window.IsDisposed;
            }

            internal void SetPopupPos(bool position, int x, int y, bool size, int width, int height)
            {
                int flags = 20;
                if (!position)
                    flags |= 2;
                if (!size)
                    flags |= 1;
                UnsafeNativeMethods.SetWindowPos(new HandleRef((object)null, this.Handle), new HandleRef((object)null, IntPtr.Zero), x, y, width, height, flags);
            }

            internal Rect GetWindowRect()
            {
                MS.Win32.NativeMethods.RECT rect = new MS.Win32.NativeMethods.RECT(0, 0, 0, 0);
                if (this.IsWindowAlive())
                    SafeNativeMethods.GetWindowRect(this._window.CreateHandleRef(), ref rect);
                return PointUtil.ToRect(rect);
            }

            internal Matrix GetTransformToDevice()
            {
                CompositionTarget compositionTarget = (CompositionTarget)this._window.CompositionTarget;
                if (compositionTarget != null)
                {
                    try
                    {
                        return compositionTarget.TransformToDevice;
                    }
                    catch (ObjectDisposedException ex)
                    {
                    }
                }
                return Matrix.Identity;
            }

            internal Matrix GetTransformFromDevice()
            {
                CompositionTarget compositionTarget = (CompositionTarget)this._window.CompositionTarget;
                if (compositionTarget != null)
                {
                    try
                    {
                        return compositionTarget.TransformFromDevice;
                    }
                    catch (ObjectDisposedException ex)
                    {
                    }
                }
                return Matrix.Identity;
            }

            private static IntPtr GetHandle(HwndSource hwnd) => hwnd == null ? IntPtr.Zero : hwnd.Handle;

            private IntPtr Handle => PopupPositioner.PopupSecurityHelper.GetHandle(this._window);
        }

        private static class Delegates
        {
            static Delegates()
            {
                try
                {
                    PopupPositioner.Delegates.GetPlacementInternal = DelegateHelper.CreatePropertyGetter<Popup, PlacementMode>("PlacementInternal", BindingFlags.Instance | BindingFlags.NonPublic, true);
                    PopupPositioner.Delegates.GetDropOpposite = DelegateHelper.CreatePropertyGetter<Popup, bool>("DropOpposite", BindingFlags.Instance | BindingFlags.NonPublic, true);
                    PopupPositioner.Delegates.GetPlacementTargetInterestPoints = DelegateHelper.CreateDelegate<Func<Popup, PlacementMode, Point[]>>(typeof(Popup), nameof(GetPlacementTargetInterestPoints), BindingFlags.Instance | BindingFlags.NonPublic);
                    PopupPositioner.Delegates.GetChildInterestPoints = DelegateHelper.CreateDelegate<Func<Popup, PlacementMode, Point[]>>(typeof(Popup), nameof(GetChildInterestPoints), BindingFlags.Instance | BindingFlags.NonPublic);
                    PopupPositioner.Delegates.GetScreenBounds = DelegateHelper.CreateDelegate<Func<Popup, Rect, Point, Rect>>(typeof(Popup), nameof(GetScreenBounds), BindingFlags.Instance | BindingFlags.NonPublic);
                }
                catch
                {
                }
            }

            public static Func<Popup, PlacementMode> GetPlacementInternal { get; }

            public static Func<Popup, bool> GetDropOpposite { get; }

            public static Func<Popup, PlacementMode, Point[]> GetPlacementTargetInterestPoints { get; }

            public static Func<Popup, PlacementMode, Point[]> GetChildInterestPoints { get; }

            public static Func<Popup, Rect, Point, Rect> GetScreenBounds { get; }
        }
    }
}
