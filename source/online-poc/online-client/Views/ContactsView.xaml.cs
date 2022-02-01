
using ClinetOnline.ViewModels;

namespace ClinetOnline.Views
{
    /// <summary>
    /// Interaction logic for ContactsView.xaml
    /// </summary>
    public partial class ContactsView : ContentPage
    {
        public ContactsView()
        {
            InitializeComponent();
        }

        private void SearchInputChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: Update SearchInput
            //contactsViewModel.SearchInput = ((Editor)sender).Text;
        }

    }
}