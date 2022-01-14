namespace MoveControl
{
    public static class ControlExtensions
    {
        /// <summary>
        /// 设置控件移动和调整大小
        /// </summary>
        public static void CanMove(this Control control)
        {
            MoveControl mControl = null;
            Point lastPoint = new Point();

            #region 绑定事件

            control.MouseDown += (sender, e) =>
            {
                lastPoint = MouseDown(control, ref mControl);
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
                    if ((ctrl is not MoveControl) && ctrl.Name != control.Name)
                    {
                        if (ctrl.RectangleToScreen(ctrl.ClientRectangle).Contains(control.RectangleToScreen(control.ClientRectangle)))
                        {
                            control.Parent = ctrl;
                            control.Location = ctrl.PointToClient(Cursor.Position);
                            lastPoint = MouseDown(control, ref mControl);
                        }
                        else
                        {
                            if (control.Parent != null && control.Parent.Parent is Form)
                            {
                                control.Parent = control.Parent.Parent;
                                control.Location = control.Parent.PointToClient(Cursor.Position);
                                lastPoint = MouseDown(control, ref mControl);
                            }
                        }
                    }
                    else if ((ctrl is not MoveControl) && ctrl.Name == control.Name)
                    {
                        if (control.Parent != null && control.Parent.Parent is Form)
                        {
                            if (!ctrl.Parent.RectangleToScreen(ctrl.Parent.ClientRectangle).Contains(control.RectangleToScreen(control.ClientRectangle)))
                            {
                                control.Parent = control.Parent.Parent;
                                control.Location = control.Parent.PointToClient(Cursor.Position);
                                lastPoint = MouseDown(control, ref mControl);
                            }
                        }
                    }
                }
            };

            #endregion
        }

        private static Point MouseDown(Control control, ref MoveControl fControl)
        {
            Point lastPoint = Cursor.Position;

            ClearParent(control);
            ClearChild(control);
            if (fControl == null)
                fControl = new MoveControl(control);
            fControl.BackColor = Color.Transparent;
            control.Parent.Controls.Add(fControl);
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
                        ctrl.Visible = false;
                nctrl = nctrl.Parent;
            }
        }

        private static void ClearChild(Control control)
        {
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl is MoveControl)
                    ctrl.Visible = false;
                ClearChild(ctrl);
            }
        }
    }
}
