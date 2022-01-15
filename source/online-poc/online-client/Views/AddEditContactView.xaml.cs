
using ClinetOnline.ViewModels;

namespace ClinetOnline.Views
{
    /// <summary>
    /// Interaction logic for AddEditContact.xaml
    /// </summary>
    public partial class AddEditContactView : ContentPage
    {
        public AddEditContactView()
        {
            InitializeComponent();
        }

        private void Validate(object sender, TextChangedEventArgs e)
        {
            ((AddEditContactViewModel)BindingContext).SaveCommand.RaiseCanExecuteChanged();
        }
    }
}
