using ClinetOnline.Common;
using ClinetOnline.ViewModels;
using Contact = ClinetOnline.Models.Contact;

namespace ClinetOnline
{
    public class MainWindowViewModel : BindableBase
    {
        public  StartViewModel StartViewModel;
        private BindableBase _currentViewModel=new BindableBase();
        public ContactsViewModel _contactsViewModel;
        public ConnectViewModel _connectViewModel;
        public AddEditContactViewModel _addEditContactViewModel;

        public MainWindowViewModel(StartViewModel startViewModel,
                                   ContactsViewModel contactsViewModel,
                                   ConnectViewModel connectViewModel,
                                   AddEditContactViewModel addEditContactViewModel)
        {
            StartViewModel = startViewModel;
            _contactsViewModel = contactsViewModel;
            _connectViewModel = connectViewModel;
            _addEditContactViewModel = addEditContactViewModel;

            StartViewModel.Started += _startViewModel_Started;
            _contactsViewModel.AddContactRequested += _contactsViewModel_AddContactRequested;
            _contactsViewModel.EditContactRequested += _contactsViewModel_EditContactRequested;
        }

        private void _contactsViewModel_EditContactRequested(Contact contact)
        {
            _addEditContactViewModel.EditMode = true;
            _addEditContactViewModel.NotEditMode = false;

            _addEditContactViewModel.SetContact(contact);
            CurrentViewModel = _addEditContactViewModel;
        }

        private void _contactsViewModel_AddContactRequested(Contact contact)
        {
            _addEditContactViewModel.EditMode = true;
            _addEditContactViewModel.NotEditMode = false;
            _addEditContactViewModel.AddNew = true;
            _addEditContactViewModel.SetContact(contact);
            CurrentViewModel = _addEditContactViewModel;
        }

        private void _startViewModel_Started()
        {
            _addEditContactViewModel.Contact = new Contact();
            CurrentViewModel = _addEditContactViewModel;
            IsNotStartPage = true;
        }

        public BindableBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        private bool _isNotStartPage;
        public bool IsNotStartPage
        {
            get => _isNotStartPage;
            set => SetProperty(ref _isNotStartPage, value);
        }

        public  void LoadAsync()
        {
            string? localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string? path = Path.Combine(localApplicationData, "ClinetOnline");
            Directory.CreateDirectory(path);
            string? stateFilePath = Path.Combine(path, "State.Json");

            if (!File.Exists(stateFilePath))
            {
                IsNotStartPage = false;
                CurrentViewModel = StartViewModel;
            }
            else
            {
                CurrentViewModel = _addEditContactViewModel;
            }
        }
    }
}
