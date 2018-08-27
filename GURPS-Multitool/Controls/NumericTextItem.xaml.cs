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
    /// Логика взаимодействия для NumericTextItem.xaml
    /// </summary>
    public partial class NumericTextItem : UserControl
    {
        private bool _isChanged = false;
        private bool _isLoaded = false;
        public event PropertyChangedEventHandler ItemChanged;

        public NumericTextItem()
        {
            InitializeComponent();
            _isLoaded = true;
        }

        private void Num_Count_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ItemChanged != null)
                ItemChanged(this, new PropertyChangedEventArgs("CounterUpDown"));
        }

        private void Txt_Header_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (ItemChanged != null)
                ItemChanged(this, new PropertyChangedEventArgs("CounterUpDown"));
        }
    }
}
