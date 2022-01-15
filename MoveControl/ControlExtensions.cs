﻿namespace MoveControl
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
        public static void CanMove(this Control control)
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
            control.MouseMove += controlEvent.MouseMove;

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
            control.MouseUp += controlEvent.MouseUp;

            _events.Add(control, controlEvent);
        }

        /// <summary>
        /// 停止控件移动和调整大小
        /// </summary>
        public static void StopMove(this Control control)
        {
            control.MouseDown -= _events[control].MouseDown;
            control.MouseUp -= _events[control].MouseUp;
            control.MouseMove -= _events[control].MouseMove;
            control.MouseClick -= _events[control].MouseClick;
            control.LostFocus -= _events[control].LostFocus;
            _events.Remove(control);
        }

        /// <summary>
        /// 设置控件内所以控件可以移动和调整大小
        /// </summary>
        public static void CanMoveChild(this Control control)
        {
            ChildEventBindig(control.Controls);
        }

        /// <summary>
        /// 停止控件内所以控件可以移动和调整大小
        /// </summary>
        public static void StopMoveChild(this Control control)
        {
            ChildEventUnBindig(control.Controls);
        }

        private static void ChildEventUnBindig(Control.ControlCollection controls)
        {
            foreach (Control item in controls)
            {
                if (item is not MoveControl)
                {
                    item.StopMove();
                    ChildEventBindig(item.Controls);
                }
            }
        }

        private static void ChildEventBindig(Control.ControlCollection controls)
        {
            foreach (Control item in controls)
            {
                if (item is not MoveControl)
                {
                    item.CanMove();
                    ChildEventBindig(item.Controls);
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
