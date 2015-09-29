using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfBatchEdit
{
    public enum HorizontalAlignment
    {
        [Description("Left")]
        Left,
        [Description("Right")]
        Right,
        [Description("Center")]
        Center
    }

    public enum VerticalAlignment
    {
        [Description("Top")]
        Top,
        [Description("Bottom")]
        Bottom,
        [Description("Center")]
        Center
    }

    public enum PagesType
    {
        [Description("First Page Only")]
        First,
        [Description("All Pages")]
        All
    }
}
