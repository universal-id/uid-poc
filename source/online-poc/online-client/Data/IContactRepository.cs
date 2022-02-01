using ClinetOnline.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contact = ClinetOnline.Models.Contact;

namespace ClinetOnline.Data
{
    public interface IContactRepository
    {
        Task<Contact> AddContactAsync(Contact contact);
        Task<Contact?> GetContactByIdAsync(string id);
        Task<List<Contact>?> GetContactsAsync();
        Task SaveChangesAsync();
        Task<Contact> UpdateContactAsync(Contact contact);
    }
}