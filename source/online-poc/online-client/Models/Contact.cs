using ClinetOnline.Common;
using System.ComponentModel.DataAnnotations;

namespace ClinetOnline.Models
{
    public class Contact : ValidatableBindableBase
    {
        private string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _name;
        [Required]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _phone;
        [Phone]
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        private string _email;
        [EmailAddress]
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _address;
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

    }
}
