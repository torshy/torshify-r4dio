using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Windows.Input;

using Torshify.Radio.Core.Services;
using Torshify.Radio.Framework;

using KeyEventArgs = System.Windows.Input.KeyEventArgs;

using UserControl = System.Windows.Controls.UserControl;

namespace Torshify.Radio.Core.Views.Settings.Tabs
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class GeneralSettingsView : UserControl, ISettingsPage
    {
        #region Constructors

        public GeneralSettingsView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        [Import]
        public GeneralSettingsViewModel Model
        {
            get
            {
                return DataContext as GeneralSettingsViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        #endregion Properties

        #region Methods

        public void Save()
        {
            Model.Save();
        }

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

            if (HotkeyList.SelectedItem != null)
            {
                (HotkeyList.SelectedItem as GlobalHotkey).Keys = winFormsKey | modifiers;
            }

            e.Handled = true;
        }

        #endregion Methods
    }
}