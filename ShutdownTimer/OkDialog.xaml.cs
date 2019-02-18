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
    /// Interaction logic for OkDialog.xaml
    /// </summary>
    public partial class OkDialog
    {
        public OkDialog() : this("<Dialog Text>", "Ok") { }

        public OkDialog(string text) : this(text, "Ok") { }

        public OkDialog(string text, string confirmtext)
        {
            DataContext = this;
            InitializeComponent();
            TBl_Text.Text = text;
            Btn_Confirm.Content = confirmtext;
        }

        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
