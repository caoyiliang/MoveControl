using System.ComponentModel;

namespace MoveControl
{
    public partial class VerticalStack : Panel
    {
        public List<Control> StackControls { get; } = new List<Control>();

        public VerticalStack()
        {
            InitializeComponent();
            this.ControlAdded += VerticalStack_ControlAdded;
            this.ControlRemoved += VerticalStack_ControlRemoved;
        }

        public VerticalStack(IContainer container)
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
                e.Control.SizeChanged += VerticalStack_SizeChanged;
            }
            SizeChanged();
        }

        private void VerticalStack_ControlRemoved(object? sender, ControlEventArgs e)
        {
            if (StackControls.Contains(e.Control)) StackControls.Remove(e.Control);
            SizeChanged();
        }

        private void VerticalStack_SizeChanged(object? sender, EventArgs e)
        {
            SizeChanged();
        }

        private void SizeChanged()
        {
            for (int i = 0; i < StackControls.Count; i++)
            {
                if (i == 0)
                {
                    StackControls[i].Location = new Point(3, 3);
                }
                else
                {
                    StackControls[i].Location = new Point(3, StackControls[i - 1].Location.Y + StackControls[i - 1].Height + 3);
                }
            }
            var index = StackControls.Count - 1;
            if (index > -1)
            {
                var h = StackControls[index].Location.Y + StackControls[index].Height + 8;
                if (h > this.Height)
                {
                    this.Height = h;
                }
            }
        }
    }
}
