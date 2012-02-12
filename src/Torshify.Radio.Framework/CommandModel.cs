using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Radio.Framework
{
    public class CommandBar : NotificationObject, ICommandBar
    {
        #region Fields

        private readonly ObservableCollection<CommandModel> _items;

        #endregion Fields

        #region Constructors

        public CommandBar()
        {
            _items = new ObservableCollection<CommandModel>();
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<CommandModel> ChildMenuItems
        {
            get { return _items; }
        }

        #endregion Properties

        #region Methods

        public ICommandBar AddCommand(string displayName, ICommand command)
        {
            return AddCommand(displayName, command, null);
        }

        public ICommandBar AddCommand(string displayName, ICommand command, object commandParameter)
        {
            CommandModel commandMenuItem = new CommandModel();
            commandMenuItem.Content = displayName;
            commandMenuItem.Command = command;
            commandMenuItem.CommandParameter = commandParameter;
            _items.Add(commandMenuItem);
            return this;
        }

        public ICommandBar AddCommand(CommandModel commandPresentation)
        {
            _items.Add(commandPresentation);
            return this;
        }

        public ICommandBar AddSeparator()
        {
            return AddSeparator(null);
        }

        public ICommandBar AddSeparator(string displayName)
        {
            SeparatorCommandModel separator = new SeparatorCommandModel();
            separator.Content = displayName;
            _items.Add(separator);
            return this;
        }

        public ICommandBar AddSubmenu(string displayName)
        {
            SubmenuCommandModel submenuCommandHoster = new SubmenuCommandModel(displayName);
            submenuCommandHoster.Content = displayName;
            _items.Add(submenuCommandHoster);
            return submenuCommandHoster;
        }

        #endregion Methods
    }

    public class CommandModel : NotificationObject
    {
        #region Fields

        private ICommand _command;
        private object _commandParameter;
        private object _content;
        private DataTemplate _contentTemplate;
        private object _icon;
        private bool _isChecked;
        private object _tooltip;
        private Visibility _visibility;

        #endregion Fields

        #region Properties

        public ICommand Command
        {
            get
            {
                return _command;
            }
            set
            {
                _command = value;
                RaisePropertyChanged("Command");
            }
        }

        public object CommandParameter
        {
            get
            {
                return _commandParameter;
            }
            set
            {
                _commandParameter = value;
                RaisePropertyChanged("CommandParameter");
            }
        }

        public object Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                RaisePropertyChanged("Content");
            }
        }

        public DataTemplate ContentTemplate
        {
            get
            {
                return _contentTemplate;
            }
            set
            {
                _contentTemplate = value;
                RaisePropertyChanged("ContentTemplate");
            }
        }

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }

        public object Tooltip
        {
            get
            {
                return _tooltip;
            }
            set
            {
                _tooltip = value;
                RaisePropertyChanged("Tooltip");
            }
        }

        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                RaisePropertyChanged("Visibility");
            }
        }

        public object Icon
        {
            get { return _icon; }
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    RaisePropertyChanged("Icon");
                }
            }
        }

        #endregion Properties
    }

    public class SeparatorCommandModel : CommandModel
    {
    }

    public class SubmenuCommandModel : CommandModel, ICommandBar
    {
        #region Fields

        private readonly CommandBar _commandBar;

        #endregion Fields

        #region Constructors

        public SubmenuCommandModel(string displayName)
        {
            Content = displayName;
            _commandBar = new CommandBar();
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<CommandModel> ChildMenuItems
        {
            get { return _commandBar.ChildMenuItems; }
        }

        #endregion Properties

        #region Methods

        public ICommandBar AddCommand(string displayName, ICommand menuItemCommand)
        {
            return _commandBar.AddCommand(displayName, menuItemCommand);
        }

        public ICommandBar AddCommand(string displayName, ICommand command, object commandParameter)
        {
            return _commandBar.AddCommand(displayName, command, commandParameter);
        }

        public ICommandBar AddCommand(CommandModel commandPresentation)
        {
            return _commandBar.AddCommand(commandPresentation);
        }

        public ICommandBar AddSeparator()
        {
            return _commandBar.AddSeparator();
        }

        public ICommandBar AddSeparator(string displayName)
        {
            return _commandBar.AddSeparator(displayName);
        }

        public ICommandBar AddSubmenu(string displayName)
        {
            return _commandBar.AddSubmenu(displayName);
        }

        #endregion Methods
    }
}