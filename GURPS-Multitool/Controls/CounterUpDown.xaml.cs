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
    /// Interaction logic for NumerUpDown.xaml
    /// </summary>
    public partial class CounterUpDown : UserControl
    {
        public event PropertyChangedEventHandler ValueChanged;
        public event PropertyChangedEventHandler ValueChangebyControl;

        public CounterUpDown()
        {
            InitializeComponent();
        }

        private int _minValue = Int32.MinValue;
        private int _maxValue = Int32.MaxValue;
        private bool _isChangable = true;

        /// <summary>
        ///  Allow change value or not
        /// </summary>
        public bool IsChangable
        {
            get { return _isChangable; }
            set
            {
                _isChangable = value;
                if (!_isChangable)
                {
                    btnUp.Visibility = Visibility.Hidden;
                    btnDown.Visibility = Visibility.Hidden;
                }
                else
                {
                    btnUp.Visibility = Visibility.Visible;
                    btnDown.Visibility = Visibility.Visible;
                }
            }
        }

        public static DependencyProperty IntegerMyProperty = 
            DependencyProperty.Register(
                "Counter", 
                typeof(int), 
                typeof(CounterUpDown),
                new PropertyMetadata(0, new PropertyChangedCallback((o,e)=>
                {
                    ((CounterUpDown) o).txtNum.Text = e.NewValue.ToString();
                })));

        public int Counter
        {
            get { return (int)GetValue(IntegerMyProperty); }
            set
            {
                SetValue(IntegerMyProperty, value);
            }
        }

        public TextAlignment NumValueAlignement
        {
            get { return txtNum.TextAlignment; }
            set { txtNum.TextAlignment = value; }
        }
        /// <summary>
        ///  Set or get minimal value
        /// </summary>
        public int MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }
        /// <summary>
        ///  Set or get maximal value
        /// </summary>
        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public bool IsTextReadOnly
        {
            get { return txtNum.IsReadOnly; }
            set { txtNum.IsReadOnly = value; }
        }

        private void TxtNum_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNum == null)
            {
                return;
            }

            if (ValueChanged != null)
                ValueChanged(this, new PropertyChangedEventArgs("Counter"));
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (!_isChangable) return;
            if (Counter < _maxValue)
                Counter++;
            if (ValueChangebyControl != null)
                ValueChangebyControl(this, new PropertyChangedEventArgs("Counter"));
        }

        private void btnDoqn_Click(object sender, RoutedEventArgs e)
        {
            if (!_isChangable) return;
            if (Counter > _minValue)
                Counter--;
            if (ValueChangebyControl != null)
                ValueChangebyControl(this, new PropertyChangedEventArgs("Counter"));
        }
    }
}
