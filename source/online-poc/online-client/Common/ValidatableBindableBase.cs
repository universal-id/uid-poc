using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace ClinetOnline.Common
{
    public class ValidatableBindableBase : BindableBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public System.Collections.IEnumerable? GetErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
                return _errors[propertyName];
            else
                return null;
        }
        [JsonIgnore]
        public bool HasErrors => _errors.Count > 0;

        protected override void SetProperty<T>(ref T member, T val, [CallerMemberName] string? propertyName = null)
        {
            base.SetProperty<T>(ref member, val, propertyName);
            ValidateProperty(propertyName, val);
        }

        private void ValidateProperty<T>(string propertyName, T value)
        {
            List<ValidationResult> results = new();
            ValidationContext context = new(this)
            {
                MemberName = propertyName
            };
            Validator.TryValidateProperty(value, context, results);

            if (results.Any())
            {

                _errors[propertyName] = results.Select(c => c.ErrorMessage).ToList();
            }
            else
            {
                _errors.Remove(propertyName);
            }
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }

}
