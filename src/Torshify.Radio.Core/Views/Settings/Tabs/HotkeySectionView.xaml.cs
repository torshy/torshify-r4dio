using System;
using System.Windows.Forms;
using System.Windows.Input;

using Torshify.Radio.Core.Services;

using KeyEventArgs = System.Windows.Input.KeyEventArgs;

using UserControl = System.Windows.Controls.UserControl;

namespace Torshify.Radio.Core.Views.Settings.Tabs
{
    public partial class HotkeySectionView : UserControl
    {
        #region Constructors

        public HotkeySectionView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void HotkeyTextBloxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var winFormsKey = (Keys)KeyInterop.VirtualKeyFromKey(e.Key);

            Keys modifiers = Keys.None;

            if (Keyboard.PrimaryDevice.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                modifiers |= Keys.Alt;
            }

            if (Keyboard.PrimaryDevice.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (winFormsKey != Keys.LControlKey)
                {
                    modifiers |= Keys.Control;
                }
                else
                {
                    winFormsKey = Keys.Control;
                }
            }

            if (Keyboard.PrimaryDevice.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                if (winFormsKey != Keys.LShiftKey)
                {
                    modifiers |= Keys.Shift;
                }
                else
                {
                    winFormsKey = Keys.Shift;
                }
            }

            if (Keyboard.PrimaryDevice.Modifiers.HasFlag(ModifierKeys.Windows))
            {
                modifiers |= Keys.LWin;
            }

            Console.WriteLine(winFormsKey + " --- " + modifiers + " = " + (winFormsKey|modifiers));

            if (HotkeyList.SelectedItem != null)
            {
                (HotkeyList.SelectedItem as GlobalHotkey).Keys = winFormsKey | modifiers;
            }

            e.Handled = true;
        }

        #endregion Methods
    }
}