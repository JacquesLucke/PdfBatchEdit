using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PdfBatchEdit
{
    public class GlobalEffectSetttingsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextEffectTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return TextEffectTemplate;
        }
    }

    public class LocalEffectSettingsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextEffectTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return TextEffectTemplate;
        }
    }
}
