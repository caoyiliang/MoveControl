using System.ComponentModel;

namespace MoveControl
{
    public partial class StackPanel : Panel
    {
        private Orientation _orientation = Orientation.Vertical;

        private ContextMenuStrip _contextMenuStrip;
        private ToolStripMenuItem _up;
        private ToolStripMenuItem _down;
        private List<Control> StackControls = new List<Control>();

        private Control SourceControl;

        [Category("布局"), Description("获取或设置一个值，该值指示子元素是垂直堆叠还是水平堆叠。")]
        public Orientation Orientation { get => _orientation; set { _orientation = value; SizeChange(); } }

        public StackPanel()
        {
            Init();
        }

        public StackPanel(IContainer container)
        {
            container.Add(this);
            Init();
        }

        private void Init()
        {
            InitializeComponent();
            this.ControlAdded += VerticalStack_ControlAdded;
            this.ControlRemoved += VerticalStack_ControlRemoved;
            _contextMenuStrip = new();
            _up = new() { Name = "up", Text = "上移" };
            _up.Click += Up_Click;
            _down = new() { Name = "down", Text = "下移" };
            _down.Click += Down_Click;
            _contextMenuStrip.Items.AddRange(new ToolStripItem[] {
                _up,
                _down
            });
            _contextMenuStrip.Opening += _contextMenuStrip_Opening;
        }

        private void _contextMenuStrip_Opening(object? sender, CancelEventArgs e)
        {
            SourceControl = (sender as ContextMenuStrip).SourceControl;
            int index = StackControls.IndexOf(SourceControl);
            _up.Enabled = index != 0;
            _down.Enabled = index != StackControls.Count - 1;
        }

        private void Down_Click(object? sender, EventArgs e)
        {
            int index = StackControls.IndexOf(SourceControl);
            var next = StackControls[index + 1];
            next.TabIndex--;
            SourceControl.TabIndex++;
            var temp = StackControls[index];
            StackControls[index] = StackControls[index + 1];
            StackControls[index + 1] = temp;
            SizeChange();
        }

        private void Up_Click(object? sender, EventArgs e)
        {
            int index = StackControls.IndexOf(SourceControl);
            var last = StackControls[index - 1];
            last.TabIndex++;
            SourceControl.TabIndex--;
            var temp = StackControls[index];
            StackControls[index] = StackControls[index - 1];
            StackControls[index - 1] = temp;
            SizeChange();
        }

        private void VerticalStack_ControlAdded(object? sender, ControlEventArgs e)
        {
            if (e.Control is not MoveControl)
            {
                e.Control.TabIndex = StackControls.Count;
                e.Control.ContextMenuStrip = _contextMenuStrip;
                StackControls.Add(e.Control);
                e.Control.Paint += (sender, e) => SizeChange();
                e.Control.SizeChanged += (sender, e) => SizeChange();
            }
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
