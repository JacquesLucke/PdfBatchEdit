using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PdfBatchEdit
{
    public class EffectDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AddTextDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return AddTextDataTemplate;
        }
    }

    public class EffectSettingsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AddTextSettingsDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return AddTextSettingsDataTemplate;
        }
    }
}
