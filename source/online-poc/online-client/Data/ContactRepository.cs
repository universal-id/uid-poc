using ClinetOnline.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Contact = ClinetOnline.Models.Contact;

namespace ClinetOnline.Data
{
    public class ContactRepository : IContactRepository
    {
        private string _contactFilePath;
        private List<Models.Contact>? _contacts;

        public ContactRepository()
        {
        }
        public async Task<List<Models.Contact>?> GetContactsAsync()
        {
            await InitAsync();
            return _contacts;
        }

        public async Task<Contact?> GetContactByIdAsync(string id)
        {
            await InitAsync();
            return _contacts?.FirstOrDefault(c => c.Id == id);
        }

        public async Task<Contact> AddContactAsync(Contact contact)
        {
            await InitAsync();
            contact.Id = Guid.NewGuid().ToString();
            _contacts.Add(contact);

            return contact;
        }

        public async Task<Contact> UpdateContactAsync(Contact contact)
        {
            await InitAsync();
            // Update contacts
            _contacts.Where(x => x.Id == contact.Id).Select(x => { x.Name = contact.Name; return x; }).ToList();

            return contact;
        }

        public async Task SaveChangesAsync()
        {
            string? contactsJson = JsonSerializer.Serialize(_contacts);
            await File.WriteAllTextAsync(_contactFilePath, contactsJson);
        }

        private async Task InitAsync()
        {
            if (_contacts == null)
            {
                string? localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string? path = Path.Combine(localApplicationData, "ClinetOnline");
                Directory.CreateDirectory(path);

                _contactFilePath = Path.Combine(path, "Contacts.Json");

                if (!File.Exists(_contactFilePath))
                {
                    List<Contact> emptyContacts = new();
                    string emptyContactsJson = JsonSerializer.Serialize(emptyContacts);
                    File.WriteAllText(_contactFilePath, emptyContactsJson);
                }

                string? contactsJson = File.ReadAllText(_contactFilePath);
                _contacts = JsonSerializer.Deserialize<List<Contact>>(contactsJson);
            }
        }
    }
}
