namespace MoveControl
{
    public static class ControlExtensions
    {
        private static MoveControl mControl = null;
        /// <summary>
        /// 设置控件移动和调整大小
        /// </summary>
        public static void CanMove(this Control control)
        {
            Point lastPoint = new Point();

            #region 绑定事件

            control.MouseDown += (sender, e) =>
            {
                lastPoint = MouseDown(control);
            };

            control.MouseClick += (sender, e) =>
            {
                control.BringToFront();
            };

            control.MouseMove += (sender, e) =>
            {
                Point currentPoint = new Point();
                Cursor.Current = Cursors.SizeAll;
                if (e.Button == MouseButtons.Left)
                {
                    currentPoint = Cursor.Position;
                    control.Location = new Point(control.Location.X + currentPoint.X - lastPoint.X,
                        control.Location.Y + currentPoint.Y - lastPoint.Y);

                    //移动时刷新实线
                    mControl.DrawSolids();

                    control.BringToFront();
                }

                lastPoint = currentPoint;
            };

            control.LostFocus += (sender, e) =>
            {
                ClearParent(control);
                ClearChild(control);
            };

            control.MouseUp += (sender, e) =>
            {
                foreach (Control ctrl in control.Parent.Controls)
                {
                    var form = ctrl.FindForm();
                    if (control.Parent is Form)
                    {
                        if (control.Name != ctrl.Name)
                            if (ctrl.RectangleToScreen(ctrl.ClientRectangle).Contains(control.RectangleToScreen(control.ClientRectangle)))
                            {
                                control.Parent = ctrl;
                                control.Location = ctrl.PointToClient(Cursor.Position);
                                lastPoint = MouseDown(control);
                            }
                    }
                    else
                    {
                        if (!ctrl.Parent.RectangleToScreen(ctrl.Parent.ClientRectangle).Contains(control.RectangleToScreen(control.ClientRectangle)))
                        {
                            control.Parent = control.Parent.Parent;
                            control.Location = control.Parent.PointToClient(Cursor.Position);
                            lastPoint = MouseDown(control);
                        }
                    }
                }
            };

            #endregion
        }

        private static Point MouseDown(Control control)
        {
            Point lastPoint = Cursor.Position;

            ClearParent(control);
            ClearChild(control);
            if (mControl != null) mControl.Dispose();
            mControl = new MoveControl(control);
            mControl.BackColor = Color.Transparent;
            control.Parent.Controls.Add(mControl);
            return lastPoint;
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
