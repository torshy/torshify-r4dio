using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Torshify.Radio.Core.Views.Settings
{
    public class FormGroupHost : ItemsControl, IFormPanelCoordinator
    {
        protected override Size MeasureOverride(Size constraint)
        {
            Size result = base.MeasureOverride(constraint);
            DoSizing();
            return result;
        }

        private void DoSizing()
        {
            List<FormPanel> groups = new List<FormPanel>();

            for (int i = 0; i < Items.Count; ++i)
            {
                var group = ItemContainerGenerator.ContainerFromIndex(i) as FormGroup;
                if (group != null && group.Items.Count > 0)
                {
                    var container = group.ItemContainerGenerator.ContainerFromIndex(0);
                    var panel = VisualTreeHelper.GetParent(container) as FormPanel;
                    if (panel != null)
                    {
                        panel.Coordinator = this;
                        groups.Add(panel);
                    }
                }
            }

            double labelMaxWidth = 0;
            double labelMaxHeight = 0;
            double controlMaxWidth = 0;
            double controlMaxHeight = 0;

            foreach (var current in groups)
            {
                labelMaxWidth = Math.Max(labelMaxWidth, current.LabelSize.Width);
                labelMaxHeight = Math.Max(labelMaxHeight, current.LabelSize.Height);
                controlMaxWidth = Math.Max(controlMaxWidth, current.ControlSize.Width);
                controlMaxHeight = Math.Max(controlMaxHeight, current.ControlSize.Height);
            }

            foreach (var current in groups)
            {
                current.LabelSize = new Size(
                    labelMaxWidth, labelMaxHeight);
                current.ControlSize = new Size(
                    controlMaxWidth, controlMaxHeight);
            }
        }

        #region IFormPanelCoordinator Members

        void IFormPanelCoordinator.ControlOrLabelSizeChanged(FormPanel sender)
        {
            DoSizing();
        }

        #endregion
    }
}
