using ClinetOnline.Common;
using ClinetOnline.Data;
using ClinetOnline.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Contact = ClinetOnline.Models.Contact;

namespace ClinetOnline.ViewModels
{
    public class ContactsViewModel : BindableBase
    {
        private readonly IContactRepository _contactRepository;
        private List<Contact>? _allContacts;

        public ContactsViewModel(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
            AddContactCommand = new RelayCommand(OnAddContact);
            EditContactCommand = new RelayCommand<Contact>(OnEditContact);
            ClearSearchCommand = new RelayCommand(OnClearSearch);
        }

        private ObservableCollection<Contact> _contacts;
        public ObservableCollection<Contact> Contacts
        {
            get => _contacts;
            set => SetProperty(ref _contacts, value);
        }

        private Contact _contact;
        public Contact Contact
        {
            get => _contact;
            set => SetProperty(ref _contact, value);
        }

        private string? _searchInput;
        public string? SearchInput
        {
            get => _searchInput;
            set
            {
                SetProperty(ref _searchInput, value);
                FilterContacts(_searchInput);
            }
        }

        private void FilterContacts(string? searchInput)
        {
            if (string.IsNullOrWhiteSpace(searchInput))
            {
                Contacts = new ObservableCollection<Contact>(_allContacts);
                return;
            }
            else
            {
                Contacts = new ObservableCollection<Contact>(_allContacts.Where(c => c.Name.ToLower().Contains(searchInput.ToLower())));
            }
        }

        public RelayCommand<Contact> PlaceOrderCommand { get; private set; }
        public RelayCommand AddContactCommand { get; private set; }
        public RelayCommand<Contact> EditContactCommand { get; private set; }
        public RelayCommand ClearSearchCommand { get; private set; }

        public event Action<Contact> AddContactRequested;
        public event Action<Contact> EditContactRequested;


        public async void LoadAsync()
        {
            _allContacts = await _contactRepository.GetContactsAsync() ?? new();
            Contacts = new ObservableCollection<Contact>(_allContacts);
        }


        private void OnAddContact()
        {
            AddContactRequested(null);
        }
        private void OnEditContact(Contact contact)
        {
            EditContactRequested(contact);
        }

        private void OnClearSearch()
        {
            SearchInput = null;
        }
    }
}
