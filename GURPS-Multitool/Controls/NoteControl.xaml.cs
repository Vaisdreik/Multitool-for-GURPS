using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GURPS_Multitool.Controls
{
    /// <summary>
    /// Логика взаимодействия для NoteControl.xaml
    /// </summary>
    public partial class NoteControl : UserControl
    {
        private bool _isChanged = false;
        private bool _isLoaded = false;
        public event PropertyChangedEventHandler NoteChanged;

        public NoteControl()
        {
            InitializeComponent();
            _isLoaded = true;
        }

        public void ChangesSaved()
        {
            _isChanged = false;
        }

        private void Txt_Header_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded) return;
            if (!_isChanged)
            {
                txb_Date.Text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                NoteChanged(this, new PropertyChangedEventArgs("Note"));
                _isChanged = true;
            }
        }
    }
}
