namespace MoveControl
{
    public static class ControlExtensions
    {
        private static MoveControl _mControl = null;
        private static Point _lastPoint = new();

        private static Dictionary<Control, ControlEvent> _events = new();
        private static Point controlCursorPosition;
        /// <summary>
        /// 设置控件移动和调整大小
        /// </summary>
        public static void BoundsChange(this Control control, bool canMove = true)
        {
            var controlEvent = new ControlEvent();
            controlEvent.MouseDown = (sender, e) => MouseDown(control);
            control.MouseDown += controlEvent.MouseDown;

            controlEvent.MouseClick = (sender, e) => control.BringToFront();
            control.MouseClick += controlEvent.MouseClick;

            controlEvent.MouseMove = (sender, e) =>
            {
                Point currentPoint = new Point();
                Cursor.Current = Cursors.SizeAll;
                if (e.Button == MouseButtons.Left)
                {
                    currentPoint = Cursor.Position;
                    control.Location = new Point(control.Location.X + currentPoint.X - _lastPoint.X,
                        control.Location.Y + currentPoint.Y - _lastPoint.Y);

                    //移动时刷新实线
                    _mControl.DrawSolids();

                    control.BringToFront();
                }

                _lastPoint = currentPoint;
            };
            if (canMove)
            {
                control.MouseMove += controlEvent.MouseMove;
            }

            controlEvent.LostFocus = (sender, e) =>
            {
                ClearParent(control);
                ClearChild(control);
            };
            control.LostFocus += controlEvent.LostFocus;

            controlEvent.MouseUp = (sender, e) =>
            {
                foreach (Control ctrl in control.Parent.Controls)
                {
                    var form = ctrl.FindForm();
                    var current = Cursor.Position;
                    if (control.Parent is Form)
                    {
                        if (control.Name != ctrl.Name)
                            if (ctrl.RectangleToScreen(ctrl.ClientRectangle).Contains(control.RectangleToScreen(control.ClientRectangle)))
                            {
                                control.Parent = ctrl;
                                control.Location = ctrl.PointToClient(new Point(current.X - controlCursorPosition.X, current.Y - controlCursorPosition.Y));
                                MouseDown(control);
                            }
                    }
                    else
                    {
                        if (!ctrl.Parent.RectangleToScreen(ctrl.Parent.ClientRectangle).Contains(control.RectangleToScreen(control.ClientRectangle)))
                        {
                            control.Parent = control.Parent.Parent;
                            control.Location = control.Parent.PointToClient(new Point(current.X - controlCursorPosition.X, current.Y - controlCursorPosition.Y));
                            MouseDown(control);
                        }
                    }
                }
            };
            if (canMove)
            {
                control.MouseUp += controlEvent.MouseUp;
            }

            controlEvent.KeyUp = (sender, e) =>
            {
                if (e.KeyCode == Keys.Delete)
                {
                    control.Dispose();
                    _events.Remove(control);
                }
            };
            control.KeyUp += controlEvent.KeyUp;

            _events.Add(control, controlEvent);
        }

        /// <summary>
        /// 停止控件移动和调整大小
        /// </summary>
        public static void StopBoundsChange(this Control control)
        {
            if (_events.Count > 0)
            {
                control.MouseDown -= _events[control].MouseDown;
                control.MouseUp -= _events[control].MouseUp;
                control.MouseMove -= _events[control].MouseMove;
                control.MouseClick -= _events[control].MouseClick;
                control.LostFocus -= _events[control].LostFocus;
                control.KeyUp -= _events[control].KeyUp;
                _events.Remove(control);
            }
        }

        /// <summary>
        /// 设置控件内所以控件可以移动和调整大小
        /// </summary>
        public static void BoundsChangeChild(this Control control, bool canMove = true)
        {
            ChildEventBindig(control.Controls);
        }

        /// <summary>
        /// 停止控件内所以控件可以移动和调整大小
        /// </summary>
        public static void StopBoundsChangeChild(this Control control)
        {
            ChildEventUnBindig(control.Controls);
        }

        private static void ChildEventUnBindig(Control.ControlCollection controls)
        {
            foreach (Control item in controls)
            {
                if (item is not MoveControl)
                {
                    item.StopBoundsChange();
                    if (item is not PropertyGrid || item is not UserControl)
                        ChildEventBindig(item.Controls);
                }
            }
        }

        private static void ChildEventBindig(Control.ControlCollection controls, bool canMove = true)
        {
            foreach (Control item in controls)
            {
                if (item is not MoveControl)
                {
                    item.BoundsChange(canMove);
                    if (item is not PropertyGrid || item is not UserControl)
                        ChildEventBindig(item.Controls, canMove);
                }
            }
        }

        private static void MouseDown(Control control)
        {
            _lastPoint = Cursor.Position;
            controlCursorPosition = control.PointToClient(_lastPoint);
            ClearParent(control);
            ClearChild(control);
            if (_mControl != null) _mControl.Dispose();
            _mControl = new MoveControl(control);
            _mControl.BackColor = Color.Transparent;
            control.Parent.Controls.Add(_mControl);
            control.Focus();
        }

        private static void ClearParent(Control control)
        {
            Control nctrl = control;
            while (nctrl.Parent != null)
            {
                var controls = nctrl.Parent.Controls;
                foreach (Control ctrl in controls)
                    if (ctrl is MoveControl)
                    {
                        ctrl.Visible = false;
                    }
                nctrl = nctrl.Parent;
            }
        }

        private static void ClearChild(Control control)
        {
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl is MoveControl)
                {
                    ctrl.Visible = false;
                }
                ClearChild(ctrl);
            }
        }
    }
}
