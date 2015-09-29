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
        [Description("Center")]
        Center,
        [Description("Right")]
        Right
    }

    public enum VerticalAlignment
    {
        [Description("Top")]
        Top,
        [Description("Center")]
        Center,
        [Description("Bottom")]
        Bottom
    }

    public enum PagesType
    {
        [Description("First Page Only")]
        First,
        [Description("All Pages")]
        All
    }
}
