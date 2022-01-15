using ClinetOnline.Common;
using ClinetOnline.Data;
using ClinetOnline.Models;
using System;
using System.Linq;
using Contact = ClinetOnline.Models.Contact;

namespace ClinetOnline.ViewModels
{
    public class AddEditContactViewModel : BindableBase
    {
        private readonly IContactRepository _contactRepository;
        public AddEditContactViewModel(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
            CancelCommand = new RelayCommand(OnCancel);
            SaveCommand = new RelayCommand(OnSave, CanSave);
            EditCommand = new RelayCommand(OnEdit);
            EditMode = true;
            NotEditMode = false;
            _contact = new Contact();
        }

        private bool _editMode;
        public bool EditMode
        {
            get => _editMode;
            set { SetProperty(ref _editMode, value); }
        }
        private bool _notEditMode;
        public bool NotEditMode
        {
            get => _notEditMode;
            set => SetProperty(ref _notEditMode, value);
        }

        private bool _addNew;
        public bool AddNew
        {
            get => _addNew;
            set => SetProperty(ref _addNew, value);
        }

        private Contact? _contact;
        public Contact? Contact
        {
            get => _contact;
            set => SetProperty(ref _contact, value);
        }

        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }

        public event Action? Done;

        public void SetContact(Contact contact)
        {
            Contact = contact;
            if (Contact != null)
            {
                Contact.ErrorsChanged -= RaiseCanExecuteChanged;
                Contact.ErrorsChanged += RaiseCanExecuteChanged;
            }
        }

        public async void LoadAsync()
        {
            if (Contact?.Id == null)
            {
                Contact? firstContact = null;
                if (!AddNew)
                    firstContact = (await _contactRepository.GetContactsAsync())?.FirstOrDefault();
                Contact = firstContact ?? new();
                SetContact(Contact);
            }
        }

        private void RaiseCanExecuteChanged(object sender, EventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void OnCancel()
        {
            Done?.Invoke();
        }

        private async void OnSave()
        {
            if (Contact?.Id != null)
                await _contactRepository.UpdateContactAsync(Contact);
            else
                await _contactRepository.AddContactAsync(Contact);

            EditMode = false;
            NotEditMode = true;
            await _contactRepository.SaveChangesAsync();
            Done?.Invoke();
        }
        private void OnEdit()
        {
            EditMode = true;
            NotEditMode = false;
        }

        private bool CanSave()
        {
            return Contact?.HasErrors != true;
        }
    }
}