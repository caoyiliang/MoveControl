using System.ComponentModel;

namespace MoveControl
{
    public partial class StackPanel : Panel
    {
        private Orientation _orientation = Orientation.Vertical;

        public List<Control> StackControls { get; } = new List<Control>();

        [Category("布局"), Description("获取或设置一个值，该值指示子元素是垂直堆叠还是水平堆叠。")]
        public Orientation Orientation { get => _orientation; set { _orientation = value; SizeChange(); } }
        public StackPanel()
        {
            InitializeComponent();
            this.ControlAdded += VerticalStack_ControlAdded;
            this.ControlRemoved += VerticalStack_ControlRemoved;
        }

        public StackPanel(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            this.ControlAdded += VerticalStack_ControlAdded;
            this.ControlRemoved += VerticalStack_ControlRemoved;
        }

        private void VerticalStack_ControlAdded(object? sender, ControlEventArgs e)
        {
            if (e.Control is not MoveControl)
            {
                StackControls.Add(e.Control);
                e.Control.Paint += Control_Paint;
            }
        }

        private void Control_Paint(object? sender, PaintEventArgs e)
        {
            SizeChange();
        }

        private void VerticalStack_ControlRemoved(object? sender, ControlEventArgs e)
        {
            if (StackControls.Contains(e.Control)) StackControls.Remove(e.Control);
            SizeChange();
        }

        private void SizeChange()
        {
            for (int i = 0; i < StackControls.Count; i++)
            {
                if (i == 0)
                {
                    StackControls[i].Location = new Point(6, 6);
                }
                else
                {
                    if (_orientation == Orientation.Vertical)
                    {
                        StackControls[i].Location = new Point(6, StackControls[i - 1].Location.Y + StackControls[i - 1].Height + 6);
                    }
                    else
                    {
                        StackControls[i].Location = new Point(StackControls[i - 1].Location.X + StackControls[i - 1].Width + 6, 6);
                    }
                }
            }
            var index = StackControls.Count - 1;
            if (index > -1)
            {
                if (_orientation == Orientation.Vertical)
                {
                    var h = StackControls[index].Location.Y + StackControls[index].Height + 8;
                    if (h > this.Height)
                    {
                        this.Height = h;
                    }
                }
                else
                {
                    var w = StackControls[index].Location.X + StackControls[index].Width + 8;
                    if (w > this.Width)
                    {
                        this.Width = w;
                    }
                }
            }
        }
    }
}
