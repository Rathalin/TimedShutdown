using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShutdownTimer
{
    /// <summary>
    /// Interaction logic for ClosingDialog.xaml
    /// </summary>
    public partial class ClosingDialog
    {
        public ClosingDialog() : this("Do you really want to exit? The shutdown will be canceled.", "No","Yes") { }

        public ClosingDialog(string text) : this(text, "No", "Yes") { }

        public ClosingDialog(string text, string canceltext, string confirmtext)
        {
            DataContext = this;
            InitializeComponent();
            TBl_Text.Text = text;
            Btn_No.Content = canceltext;
            Btn_Yes.Content = confirmtext;
        }

        private void ButtonYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
