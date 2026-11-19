using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Modules.Annotations.SourceGenerators;

namespace PlainCEETimer.UI;

public sealed partial class ControlDragInfo(Control source)
{
    [BackingField("m_dragging")]
    public partial bool IsDragging { get; }

    [BackingField("m_started")]
    public partial bool IsStarted { get; }

    [BackingField("m_pos")]
    public partial Point Location { get; }

    [BackingField("m_offset")]
    public partial Point Offset { get; }

    private Point m_start;
    private Size m_ds;

    public void Begin(Point offset)
    {
        if (!m_dragging)
        {
            m_dragging = true;
            m_started = false;
            m_offset = offset;
            m_pos = m_start = Cursor.Position;
            m_ds = SystemInformation.DragSize;
            source.Capture = true;
        }
    }

    public void Update()
    {
        if (m_dragging)
        {
            m_pos = Cursor.Position;

            if (!m_started)
            {
                m_started = Math.Abs(m_pos.X - m_start.X) >= m_ds.Width || Math.Abs(m_pos.Y - m_start.Y) >= m_ds.Height;
            }
        }
    }

    public void End()
    {
        if (m_dragging)
        {
            m_dragging = false;
            m_started = false;
            source.Capture = false;
        }
    }
}
