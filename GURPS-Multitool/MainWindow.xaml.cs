using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GURPS_Multitool.Controls;
using GURPS_Multitool.Properties;
using Microsoft.Win32;

namespace GURPS_Multitool
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _craftPath = "";
        private string _dataPath = "";
        private int _curentLeftTabVisibleID = 0;
        private ItemCollection _tabs;
        private bool _isLoaded = false;

        private int _gadgeteerLvl = 0;
        private int _inventorSkill = 0;
        private int _wcHalfBonus = 0;

        private Craft _craft;
        private Character _character;
        public ObservableCollection<Craft> Crafts = new ObservableCollection<Craft>();


        public MainWindow()
        {
            this.DataContext = this;
            _dataPath = Directory.GetCurrentDirectory() + "\\GURPS-Multitool-Data\\";
            _craftPath = _dataPath + "craft\\";
            if (!Directory.Exists(_dataPath))
                Directory.CreateDirectory(_dataPath);
            if (!Directory.Exists(_craftPath))
                Directory.CreateDirectory(_craftPath);
            _craft = new Craft();
            _character = new Character();
            InitializeComponent();
            _tabs = TabsViewer.Items;
            LoadCharacter();
            LoadCraftsList();
            _isLoaded = true;
        }

        private void Menu_option_developer_OnChecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.isDevMode = true;
            Settings.Default.Save();
        }
        private void Menu_option_developer_OnUnchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.isDevMode = false;
            Settings.Default.Save();
        }

        #region Character

        const string strMTFilter = "GURPS-MT XML files (*.mtxml)|*.mtxml";
        XmlSerializer xmlCharser = new XmlSerializer(typeof(Character));

        private void SaveCharacter()
        {
            if (menu_option_developer.IsChecked.Value)
            {
                SaveFileDialog sfdlg = new SaveFileDialog();
                sfdlg.InitialDirectory = _dataPath;
                sfdlg.FileName = _character.Name;
                sfdlg.Filter = strMTFilter;
                if (sfdlg.ShowDialog() == true)
                {
                    StreamWriter sw = new StreamWriter(sfdlg.FileName);
                    xmlCharser.Serialize(sw, _character);
                    sw.Close();
                }
            }
            else
            {
                StreamWriter sw = new StreamWriter(_dataPath + _character.Name + ".mtxml");
                xmlCharser.Serialize(sw, _character);
                sw.Close();
            }
        }

        private void Btn_SaveCharacter_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateCharacterInfo();
            SaveCharacter();
            btn_SaveCharacter.IsEnabled = false;
        }

        private void LoadCharacter()
        {
            if (menu_option_developer.IsChecked.Value)
            {
                OpenFileDialog ofdlg = new OpenFileDialog();
                ofdlg.InitialDirectory = _dataPath;
                ofdlg.Filter = strMTFilter;
                if (ofdlg.ShowDialog() == true)
                {
                    using (StreamReader sr = new StreamReader(ofdlg.FileName))
                    {
                        _character = (Character)xmlCharser.Deserialize(sr);
                    }
                }
            }
            else
            {
                if (!File.Exists(_dataPath + _character.Name + ".mtxml")) return;
                using (StreamReader sr = new StreamReader(_dataPath + _character.Name + ".mtxml"))
                {
                    _character = (Character)xmlCharser.Deserialize(sr);
                }
            }
            ShowCharacterInfo();
            lst_Items.ItemsSource = _character.CharItems;
            lst_Items.SelectedIndex = 0;
            lst_Notes.ItemsSource = _character.Notes;
            lst_Notes.SelectedIndex = 0;
        }

        private void ShowCharacterInfo()
        {
            num_ST.NumValue = _character.ST;
            num_DX.NumValue = _character.DX;
            num_IQ.NumValue = _character.IQ;
            num_HT.NumValue = _character.HT;
            num_BasicMove.NumValue = _character.BM;
            txt_CraftSkillName.Text = _character.CraftSkillName;
            num_CraftSkillLvl.NumValue = _character.CraftSkill;
            txbl_PointInCraftSkill.Text =
                $"{_character.GetCraftSkillPoints()} points; IQ{((_character.CraftSkill - _character.IQ) >= 0 ? "+" : "")}{(_character.CraftSkill - _character.IQ)}";
            num_WP.NumValue = _character.WP;
            num_WCHalfBonus.NumValue = _character.WCHalfBonus;
            num_GadgeteerLvl.NumValue = _character.GadgeteerLVL;
            num_SuperJump.NumValue = _character.SuperJump;
            num_ShootSkillLvl.NumValue = _character.ShootingSkill;
        }

        private void UpdateCharacterInfo()
        {
            _character.ST = num_ST.NumValue;
            _character.DX = num_DX.NumValue;
            _character.IQ = num_IQ.NumValue;
            _character.HT = num_HT.NumValue;
            _character.BM = num_BasicMove.NumValue;
            _character.CraftSkillName = txt_CraftSkillName.Text;
            _character.CraftSkill = num_CraftSkillLvl.NumValue;
            _character.WP = num_WP.NumValue;
            _character.WCHalfBonus = num_WCHalfBonus.NumValue;
            _character.GadgeteerLVL = num_GadgeteerLvl.NumValue;
            _character.SuperJump = num_SuperJump.NumValue;
        }

        private void Num_CraftSkillLvl_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (num_CraftSkillLvl == null) return;
            _inventorSkill = num_CraftSkillLvl.NumValue;
            btn_SaveCharacter.IsEnabled = true;
            CalculateConceptEffectiveSkill();
            CalculatePrototypeEffectiveSkill();
        }

        private void Num_CraftSkillLvl_OnValueChangebyControl(object sender, PropertyChangedEventArgs e)
        {
            if (num_CraftSkillLvl == null) return;
            Num_Attributes_OnValueChanged(this, new PropertyChangedEventArgs("Num_CraftSkillLvl"));
            _character.ChangeCraftSkill(num_CraftSkillLvl.NumValue);
            ShowCharacterInfo();
        }

        private void Num_GadgeteerLvl_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (num_GadgeteerLvl == null) return;
            _gadgeteerLvl = num_GadgeteerLvl.NumValue;
            Num_Attributes_OnValueChanged(this, new PropertyChangedEventArgs("Num_IQ"));
            CalculateComplexity();
        }

        private void Num_GadgeteerLvl_OnLoaded(object sender, RoutedEventArgs e)
        {
            _gadgeteerLvl = num_GadgeteerLvl.NumValue;
            CalculateComplexity();
        }

        private void Num_WCHalfBonus_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (num_WCHalfBonus == null) return;
            _wcHalfBonus = num_WCHalfBonus.NumValue;
            btn_SaveCharacter.IsEnabled = true;
            CalculateConceptEffectiveSkill();
            CalculatePrototypeEffectiveSkill();
        }

        private void Num_Attributes_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_isLoaded) return;
            btn_SaveCharacter.IsEnabled = true;
        }

        private void Num_IQ_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            _character.ChangeIQ(num_IQ.NumValue);
            if (!IsInitialized) return;
            ShowCharacterInfo();
            Num_Attributes_OnValueChanged(this, new PropertyChangedEventArgs("Num_IQ"));
        }

        #endregion


        #region Craft

        const string strFilter = "Craft XML files (*.cxml)|*.cxml";
        XmlSerializer xmlCraftSerializer = new XmlSerializer(typeof(Craft));

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var item = cb.DataContext;
            lst_SavedGadgets.SelectedItem = item;
            using (Cursor)
            {
                {
                    StreamWriter sw = new StreamWriter(_craftPath + (lst_SavedGadgets.SelectedItem as Craft).Name + ".cxml");
                    xmlCraftSerializer.Serialize(sw, (lst_SavedGadgets.SelectedItem as Craft));
                    sw.Close();
                }
            }
        }

        private void LoadCraftsList()
        {
            if (!Directory.Exists(_dataPath))
                Directory.CreateDirectory(_dataPath);
            if (!Directory.Exists(_craftPath))
                Directory.CreateDirectory(_craftPath);
            string[] files = Directory.GetFiles(_craftPath);
            Crafts.Clear();
            foreach (string file in files)
            {
                using (
                    StreamReader sr =
                        new StreamReader(file))
                {
                    Crafts.Add((Craft)xmlCraftSerializer.Deserialize(sr));
                }
            }
            lst_SavedGadgets.ItemsSource = Crafts;
            lst_SavedGadgets.SelectedIndex = 0;
        }

        private void SaveCraft()
        {
            if (menu_option_developer.IsChecked.Value)
            {
                SaveFileDialog sfdlg = new SaveFileDialog();
                sfdlg.InitialDirectory = _craftPath;
                sfdlg.FileName = _craft.Name;
                sfdlg.Filter = strFilter;
                if (sfdlg.ShowDialog() == true)
                {
                    StreamWriter sw = new StreamWriter(sfdlg.FileName);
                    xmlCraftSerializer.Serialize(sw, _craft);
                    sw.Close();
                }
            }
            else
            {
                StreamWriter sw = new StreamWriter(_craftPath + _craft.Name + ".cxml");
                xmlCraftSerializer.Serialize(sw, _craft);
                sw.Close();
            }
            LoadCraftsList();
        }

        private void LoadCraft()
        {
            if (menu_option_developer.IsChecked.Value)
            {
                OpenFileDialog ofdlg = new OpenFileDialog();
                ofdlg.InitialDirectory = _craftPath;
                ofdlg.Filter = strFilter;
                if (ofdlg.ShowDialog() == true)
                {
                    using (StreamReader sr = new StreamReader(ofdlg.FileName))
                    {
                        _craft = (Craft)xmlCraftSerializer.Deserialize(sr);
                    }
                }
            }
            else
            {
                using (
                    StreamReader sr =
                        new StreamReader(_craftPath + (lst_SavedGadgets.SelectedItem as Craft).Name +
                                         ".cxml"))
                {
                    _craft = (Craft)xmlCraftSerializer.Deserialize(sr);
                }
            }
            txt_GadgetName.Text = _craft.Name;
            txt_GadgetPoints.Text = _craft.Points.ToString();
            CalculateComplexity();
            LoadCraftsList();
            UpdateCraftsNumView();
        }

        private void DeleteCraft()
        {
            var result = MessageBox.Show("Действительно удалить?", "Удаление крафта!", MessageBoxButton.YesNo,
                MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
                try
                {
                    File.Delete(_craftPath + (lst_SavedGadgets.SelectedItem as Craft).Name + ".cxml");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            LoadCraftsList();
        }

        private void UpdateCraftsNumView()
        {
            num_TechLevel.NumValue = _craft.TechLevel;
            num_ConceptTime.NumValue = _craft.ConceptTimeBonus;
            num_ConceptDiceOnTime.NumValue = _craft.ConceptDiceResult;
            chkbx_ConceptIsUseWCHalfBonus.IsChecked = _craft.ConceptIsUseWCHalfBonus;
            num_ConceptHelpBonus.NumValue = _craft.ConceptHelpersBonus;
            num_ConceptWP.NumValue = _craft.ConceptWP;

            num_PrototypeTime.NumValue = _craft.PrototypeTimeBonus;
            num_PrototypeDiceOnTime.NumValue = _craft.PrototypeDiceResult;
            chkbx_PrototypeIsUseWCHalfBonus.IsChecked = _craft.PrototypeIsUseWCHalfBonus;
            num_PrototypeHelpBonus.NumValue = _craft.PrototypeHelpersBonus;
            num_PrototypeWP.NumValue = _craft.PrototypeWP;
        }

        private void Btn_SaveGadget_OnClick(object sender, RoutedEventArgs e)
        {
            SaveCraft();
        }
        private void Btn_LoadGadget_OnClick(object sender, RoutedEventArgs e)
        {
            LoadCraft();
        }
        private void Btn_DeleteGadget_OnClick(object sender, RoutedEventArgs e)
        {
            DeleteCraft();
        }

        private void Txt_GadgetName_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _craft.Name = txt_GadgetName.Text;
            }
        }
        private void Txt_GadgetName_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _craft.Name = txt_GadgetName.Text;
        }

        private void CalculateComplexity()
        {
            _craft.Complexity = _craft.Points + 600;
            if (_gadgeteerLvl < 2)
            {
                if (_craft.Complexity % 75 != 0)
                    _craft.ComplexityPenalty = 6 + (_craft.Complexity / 75) + 1;
                else
                    _craft.ComplexityPenalty = 6 + (_craft.Complexity / 75);
            }
            else
            {
                if (_craft.Complexity % 150 != 0)
                    _craft.ComplexityPenalty = _craft.Complexity / 150 + 1;
                else
                    _craft.ComplexityPenalty = _craft.Complexity / 150;
            }
            if (txb_Complexity == null) return;
            txb_Complexity.Text = _craft.Complexity.ToString();
            CalculateConceptTime();
            CalculateConceptEffectiveSkill();
            CalculatePrototypeTime();
            CalculatePrototypeEffectiveSkill();
        }

        private void Txt_GadgetPoints_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txt_GadgetPoints == null) return;
                int tmp_points = 0;
                if (!int.TryParse(txt_GadgetPoints.Text, out tmp_points))
                {
                    MessageBox.Show("Количество очков должно быть введено только цифрами и являться целым числом");
                    txt_GadgetPoints.Text = "0";
                }
                _craft.Points = tmp_points;
                CalculateComplexity();
            }
        }
        private void Txt_GadgetPoints_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (txt_GadgetPoints == null) return;
            int tmp_points = 0;
            if (!int.TryParse(txt_GadgetPoints.Text, out tmp_points))
            {
                MessageBox.Show("Количество очков должно быть введено только цифрами и являться целым числом");
                txt_GadgetPoints.Text = "0";
            }
            _craft.Points = tmp_points;
            CalculateComplexity();
        }

        private void Num_TechLevel_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (num_TechLevel == null) return;
            _craft.TechLevel = num_TechLevel.NumValue;
            CalculateConceptEffectiveSkill();
            CalculatePrototypeEffectiveSkill();
        }

        #endregion

        #region Concept

        private void CalculateConceptTime()
        {
            if (txb_ConceptTotalTime == null) return;

            if (_gadgeteerLvl < 2)
            {
                _craft.ConceptTimeTotal = _craft.ConceptDiceResult * _craft.ConceptMultiplier;
                txb_ConceptTotalTime.Text = $"Общее время:  {_craft.ConceptTimeTotal} раб. дней";
            }
            else
            {
                _craft.ConceptTimeTotal = (_craft.ConceptDiceResult - 2) * _craft.ConceptMultiplier;
                txb_ConceptTotalTime.Text = $"Общее время:  {_craft.ConceptTimeTotal} мин.";
            }
        }

        private void CalculateConceptEffectiveSkill()
        {
            if (txb_ConceptEffectiveSkill == null) return;
            _craft.ConceptEffectiveSkill = _inventorSkill - 5 * (_craft.TechLevel - _craft.TechLevelCampain)
                                           - _craft.ComplexityPenalty + _craft.ConceptTimeBonus +
                                           _craft.ConceptHelpersBonus + 2 * _craft.ConceptWP +
                                           (_craft.ConceptIsUseWCHalfBonus ? _wcHalfBonus : 0);
            if (txb_ConceptEffectiveSkill == null) return;
            txb_ConceptEffectiveSkill.Text = $"Эффективный скилл: {_craft.ConceptEffectiveSkill}";
            txb_ConceptEffectiveSkill.ToolTip =
                $"{_inventorSkill}! - {(5 * (_craft.TechLevel - _craft.TechLevelCampain))}({_craft.TechLevel}TL)" +
                $" - {_craft.ComplexityPenalty}(на {(_gadgeteerLvl == 2 ? "150p" : "75p")})" +
                $" + {_craft.ConceptTimeBonus}(time) " +
                $" + {_craft.ConceptHelpersBonus}({txt_ConceptHelpers.Text.Replace("\r\n", ",")})" +
                $" + {2 * _craft.ConceptWP}({_craft.ConceptWP}WP)"
                + (_craft.ConceptIsUseWCHalfBonus ? (" + " + _wcHalfBonus + "(WCHalfBonus)") : "");
            txb_ConceptFormula.Text =
                $"{_inventorSkill}! - {(5 * (_craft.TechLevel - _craft.TechLevelCampain))}({_craft.TechLevel}TL)" +
                $" - {_craft.ComplexityPenalty}(на {(_gadgeteerLvl == 2 ? "150p" : "75p")}) + {_craft.ConceptTimeBonus}(time)" +
                $" + {_craft.ConceptHelpersBonus}({txt_ConceptHelpers.Text.Replace("\r\n", ",")})" +
                $" + {2 * _craft.ConceptWP}({_craft.ConceptWP}WP)"
                + (_craft.ConceptIsUseWCHalfBonus ? (" + " + _wcHalfBonus + "(WCHalfBonus)") : "");
        }

        private void Num_ConceptTime_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            _craft.SetConceptTimeBonus(num_ConceptTime.NumValue);
            CalculateConceptTime();
            CalculateConceptEffectiveSkill();
        }

        private void Num_ConceptDiceOnTime_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            _craft.ConceptDiceResult = num_ConceptDiceOnTime.NumValue;
            CalculateConceptTime();
        }

        private void Chkbx_ConceptIsUseWCHalfBonus_OnChecked(object sender, RoutedEventArgs e)
        {
            if (chkbx_ConceptIsUseWCHalfBonus == null) return;
            _craft.ConceptIsUseWCHalfBonus = chkbx_ConceptIsUseWCHalfBonus.IsChecked.Value;
            CalculateConceptEffectiveSkill();
        }

        private void Num_ConceptHelpBonus_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (num_ConceptHelpBonus == null) return;
            _craft.ConceptHelpersBonus = num_ConceptHelpBonus.NumValue;
            CalculateConceptEffectiveSkill();
        }

        private void Txt_ConceptHelpers_OnLostFocus(object sender, RoutedEventArgs e)
        {
            CalculateConceptEffectiveSkill();
        }

        private void Num_ConceptWP_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (num_ConceptWP == null) return;
            if (num_ConceptWP.NumValue <= _character.WP - num_PrototypeWP.NumValue)
            {
                _craft.ConceptWP = num_ConceptWP.NumValue;
                num_ConceptWP.MaxValue = _character.WP - num_PrototypeWP.NumValue;
                num_PrototypeWP.MaxValue = _character.WP - num_ConceptWP.NumValue;
            }
            CalculateConceptEffectiveSkill();
        }

        private void CopyConceptSkillFormula_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(txb_ConceptFormula.Text);
            }
            catch (Exception exception)
            {
                MessageBox.Show("error");
            }
        }

        #endregion

        #region Prototype

        private void CalculatePrototypeTime()
        {
            if (txb_PrototypeTotalTime != null)
            {
                if (_gadgeteerLvl < 2)
                {
                    _craft.PrototypeTimeTotal = _craft.PrototypeDiceResult *
                                                ((_craft.Complexity * _craft.Complexity) / 20000) *
                                                _craft.PrototypeMultiplier;
                    txb_PrototypeTotalTime.Text = $"Общее время:  {_craft.PrototypeTimeTotal} раб. дней";
                }
                else
                {
                    _craft.PrototypeTimeTotal = _craft.PrototypeDiceResult *
                                                ((_craft.Complexity * _craft.Complexity) / 20000) *
                                                _craft.PrototypeMultiplier;
                    txb_PrototypeTotalTime.Text = $"Общее время:  {_craft.PrototypeTimeTotal} мин.";
                }
            }
        }

        private void CalculatePrototypeEffectiveSkill()
        {
            if (txb_PrototypeEffectiveSkill == null) return;

            _craft.PrototypeEffectiveSkill = _inventorSkill - 5 * (_craft.TechLevel - _craft.TechLevelCampain)
                                             - _craft.ComplexityPenalty + _craft.PrototypeTimeBonus +
                                             _craft.PrototypeHelpersBonus + 2 * _craft.PrototypeWP +
                                             (_craft.PrototypeIsUseWCHalfBonus ? _wcHalfBonus : 0);
            if (txb_PrototypeEffectiveSkill == null) return;
            txb_PrototypeEffectiveSkill.Text = $"Эффективный скилл: {_craft.PrototypeEffectiveSkill}";
            txb_PrototypeEffectiveSkill.ToolTip =
                $"{_inventorSkill}! - {(5 * (_craft.TechLevel - _craft.TechLevelCampain))}({_craft.TechLevel}TL)" +
                $" - {_craft.ComplexityPenalty}(на {(_gadgeteerLvl == 2 ? "150p" : "75p")}) + {_craft.PrototypeTimeBonus}(time)" +
                $" + {_craft.PrototypeHelpersBonus}({txt_PrototypeHelpers.Text.Replace("\r\n", ",")})" +
                $" + {2 * _craft.PrototypeWP}({_craft.PrototypeWP}WP)"
                + (_craft.PrototypeIsUseWCHalfBonus ? (" + " + _wcHalfBonus + "(WCHalfBonus)") : "");
            txb_PrototypeFormula.Text =
                $"{_inventorSkill}! - {(5 * (_craft.TechLevel - _craft.TechLevelCampain))}({_craft.TechLevel}TL)" +
                $" - {_craft.ComplexityPenalty}(на {(_gadgeteerLvl == 2 ? "150p" : "75p")})  + {_craft.PrototypeTimeBonus}(time)" +
                $" + {_craft.PrototypeHelpersBonus}({txt_PrototypeHelpers.Text.Replace("\r\n", ",")})" +
                $" + {2 * _craft.PrototypeWP}({_craft.PrototypeWP}WP)"
                + (_craft.PrototypeIsUseWCHalfBonus ? (" + " + _wcHalfBonus + "(WCHalfBonus)") : "");
        }

        private void Num_PrototypeTime_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            _craft.SetPrototypetTimeBonus(num_PrototypeTime.NumValue);

            CalculatePrototypeTime();
            CalculatePrototypeEffectiveSkill();
        }

        private void Num_PrototypeDiceOnTime_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            _craft.PrototypeDiceResult = num_PrototypeDiceOnTime.NumValue;
            CalculatePrototypeTime();
        }

        private void Chkbx_PrototypeIsUseWCHalfBonus_OnChecked(object sender, RoutedEventArgs e)
        {
            if (chkbx_PrototypeIsUseWCHalfBonus == null) return;
            _craft.PrototypeIsUseWCHalfBonus = chkbx_PrototypeIsUseWCHalfBonus.IsChecked.Value;
            CalculatePrototypeEffectiveSkill();
        }

        private void Num_PrototypeHelpBonus_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (num_PrototypeHelpBonus == null) return;
            _craft.PrototypeHelpersBonus = num_PrototypeHelpBonus.NumValue;
            CalculatePrototypeEffectiveSkill();
        }

        private void Txt_PrototypeHelpers_OnLostFocus(object sender, RoutedEventArgs e)
        {
            CalculatePrototypeEffectiveSkill();
        }

        private void Num_PrototypeWP_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (num_PrototypeWP == null) return;
            if (num_PrototypeWP.NumValue <= _character.WP - num_ConceptWP.NumValue)
            {
                _craft.PrototypeWP = num_PrototypeWP.NumValue;
                num_ConceptWP.MaxValue = _character.WP - num_PrototypeWP.NumValue;
                num_PrototypeWP.MaxValue = _character.WP - num_ConceptWP.NumValue;
            }

            CalculatePrototypeEffectiveSkill();
        }

        private void CopyPrototypeSkillFormula_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(txb_PrototypeFormula.Text);
            }
            catch (Exception exception)
            {
                MessageBox.Show("error");
            }
        }

        #endregion


        #region Jumps

        private void Num_BasicMove_OnValueChangebyControl(object sender, PropertyChangedEventArgs e)
        {
            if (num_BasicMove == null) return;
            _character.BM = num_BasicMove.NumValue;
            btn_SaveCharacter.IsEnabled = true;
            CalculateJumps();
        }

        private void Num_JumpingSkill_OnValueChangebyControl(object sender, PropertyChangedEventArgs e)
        {
            if (num_JumpingSkill == null) return;
            btn_SaveCharacter.IsEnabled = true;
            CalculateJumps();
        }

        private void Num_SuperJump_OnValueChangebyControl(object sender, PropertyChangedEventArgs e)
        {
            if (num_SuperJump == null) return;
            _character.SuperJump = num_SuperJump.NumValue;
            btn_SaveCharacter.IsEnabled = true;
            CalculateJumps();
        }

        private void Chbx_FlyingLeap_CheckChange(object sender, RoutedEventArgs e)
        {
            CalculateJumps();
        }

        private void Chbx_InCombat_CheckChange(object sender, RoutedEventArgs e)
        {
            CalculateJumps();
        }

        private void Num_Running_OnValueChangebyControl(object sender, PropertyChangedEventArgs e)
        {
            CalculateJumps();
        }

        private void Chbx_Running_CheckChange(object sender, RoutedEventArgs e)
        {
            CalculateJumps();
        }


        private void CalculateJumps()
        {
            if (txb_JumpInHighFormula == null || txb_JumpInLongFormula == null /*|| txb_JumpInRunningFormula == null*/) return;
            if (chbx_Running.IsChecked.Value == false)
            {
                stp_running.Visibility = Visibility.Collapsed;
                JumpInHigh(0);
                JumpInLong(0);
            }
            else
            {
                stp_running.Visibility = Visibility.Visible;
                JumpInHigh(num_Running.NumValue);
                JumpInLong(num_Running.NumValue);
            }
        }

        private void JumpInHigh(int run)
        {
            float jump = 0;
            txb_JumpInHighFormula.Text = $"Lh=(6×БД[или Jumping]{(run > 0 ? " + РП" : "")}) - 10 дюймов = ";
            if (_character.BM < 2)
                jump = 0;
            else
                jump = (6 * ((_character.BM > num_JumpingSkill.NumValue / 2 ? _character.BM : (int)num_JumpingSkill.NumValue / 2) + run)) - 10;
            txb_JumpInHighFormula.Text += $"(6×({(_character.BM > num_JumpingSkill.NumValue ? _character.BM : (int)num_JumpingSkill.NumValue / 2)}+{run})) - 10\"=";
            if (_character.SuperJump > 0)
            {
                txb_JumpInHighFormula.Text += $"{jump}\" x {Math.Pow(2, _character.SuperJump)}(SJ)=";
                jump = jump * (float)Math.Pow(2, _character.SuperJump);
            }
            if (chbx_FlyingLeap.IsChecked.Value)
            {
                txb_JumpInHighFormula.Text += $"{jump}\" x 3(FL)=";
                jump = jump * 3;
            }
            if (chbx_InCombat.IsChecked.Value)
            {
                txb_JumpInHighFormula.Text += $"{jump}\" / 2(In combat)=";
                jump = jump / 2;
            }
            txb_JumpInHighFormula.Text += $"{jump}\"=";
            jump = jump * 2.54f / 100;
            txb_JumpInHighFormula.Text += $"{jump}м";
            exp_JumpInHigh.Header = $"Прыжок в высоту {(run > 0 ? "с разбегом " : "")}= {jump} м";
        }

        private void JumpInLong(int run)
        {
            float jump = 0;
            txb_JumpInLongFormula.Text = $"Lh=(2×БД[or Jumping]{(run > 0 ? " + РП" : "")}) - 3 фута = ";
            if (_character.BM < 2)
                jump = 0;
            else
                jump = (2 * ((_character.BM > num_JumpingSkill.NumValue / 2 ? _character.BM : (int)num_JumpingSkill.NumValue / 2) + run)) - 3;
            txb_JumpInLongFormula.Text += $"(2×{(_character.BM > num_JumpingSkill.NumValue ? _character.BM : (int)num_JumpingSkill.NumValue / 2)}+{run})) - 3\'=";
            if (_character.SuperJump > 0)
            {
                txb_JumpInLongFormula.Text += $"{jump}\' x {Math.Pow(2, _character.SuperJump)}(SJ)=";
                jump = jump * (float)Math.Pow(2, _character.SuperJump);
            }
            if (chbx_FlyingLeap.IsChecked.Value)
            {
                txb_JumpInLongFormula.Text += $"{jump}\' x 3(FL)=";
                jump = jump * 3;
            }
            if (chbx_InCombat.IsChecked.Value)
            {
                txb_JumpInLongFormula.Text += $"{jump}\' / 2(In combat)=";
                jump = jump / 2;
            }
            txb_JumpInLongFormula.Text += $"{jump}\'=";
            jump = jump * 0.3048f;
            txb_JumpInLongFormula.Text += $"{jump}м";
            exp_JumpInLong.Header = $"Прыжок в длину {(run > 0 ? "с разбегом " : "")}= {jump} м";
        }


        #endregion

        #region Items

        private void Btn_AddItem_OnClick(object sender, RoutedEventArgs e)
        {
            _character.CharItems.Add(new CharacterItem());
            btn_SaveCharacter.IsEnabled = true;
        }

        private void Btn_DeleteItem_OnClick(object sender, RoutedEventArgs e)
        {
            _character.CharItems.Remove(lst_Items.SelectedItem as CharacterItem);
            btn_SaveCharacter.IsEnabled = true;
        }

        #endregion

        #region Notes

        private void Btn_AddNote_OnClick(object sender, RoutedEventArgs e)
        {
            _character.Notes.Add(new Note());
            btn_SaveCharacter.IsEnabled = true;
        }

        private void Btn_DeleteNote_OnClick(object sender, RoutedEventArgs e)
        {
            _character.Notes.Remove(lst_Notes.SelectedItem as Note);
            btn_SaveCharacter.IsEnabled = true;
        }

        private void Lst_Notes_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Settings.Default.isDevMode)
                MessageBox.Show((lst_Notes.SelectedItem as Note).Title);
        }

        #endregion

        private int _shootingSkillLvl = 10;
        private int _effectShootingSkill = 0;
        private int _MaxCountOfShootingWP = 0;

        private void Num_ShootSkillLvl_OnValueChangebyControl(object sender, PropertyChangedEventArgs e)
        {
            if (num_ShootSkillLvl == null) return;
            _character.ShootingSkill = num_ShootSkillLvl.NumValue;
            btn_SaveCharacter.IsEnabled = true;
            CalculateEffectiveShootingSkill();
        }

        private void Txt_ShooSkillName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO: processing text
        }

        private void Num_ShootingWPUsed_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (num_ShootingWPUsed == null) return;
            CalculateEffectiveShootingSkill();
        }

        private void Num_AimBonus_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (num_AimBonus == null) return;
            CalculateEffectiveShootingSkill();
        }

        private void Num_TargetSize_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (num_TargetSize == null) return;
            CalculateEffectiveShootingSkill();
        }

        private void Num_DistanceSpeed_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (num_DistanceSpeed == null) return;
            CalculateEffectiveShootingSkill();
        }

        private void Cmbx_RateOfFire_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbx_RateOfFire == null) return;
            CalculateEffectiveShootingSkill();
        }

        private void Num_OtherBonus_OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (num_OtherBonus == null) return;
            CalculateEffectiveShootingSkill();
        }

        private void CalculateEffectiveShootingSkill()
        {
            if (_character == null || txb_PointInShootingSkill == null || num_OtherBonus == null) return;
            _shootingSkillLvl = num_ShootSkillLvl.NumValue;
            txb_PointInShootingSkill.Text = _character.GetWildCardSkillPoints(_shootingSkillLvl).ToString();
            num_ShootingHalfHBonus.NumValue = _character.GetWildCardWCHalfBonus(_shootingSkillLvl);
            int HB = num_ShootingHalfHBonus.NumValue;
            int WP = num_ShootingWPUsed.NumValue;
            num_ShootingWPRemain.NumValue = _character.GetWildCardWPCounts(_shootingSkillLvl);
            int AimBonus = num_AimBonus.NumValue;
            //TODO: check size modifer bonus
            int SMBonus = num_TargetSize.NumValue;
            int DSmodifer = GetDistanceModifer(num_DistanceSpeed.NumValue); // negative numbers
            _effectShootingSkill = _shootingSkillLvl + HB + WP*2 + AimBonus + SMBonus + DSmodifer + cmbx_RateOfFire.SelectedIndex + num_OtherBonus.NumValue;
            txb_ShootingEffectSkill.Text = _effectShootingSkill.ToString();
            txb_ShootingEffectSkill.ToolTip = $"{_shootingSkillLvl}(skill) + {HB}(HalfBonus) + {WP * 2}(WPBonus) + {AimBonus}(Acc.) + {SMBonus}(SM) + {DSmodifer}(Расстояние) +"+
                                              $" {cmbx_RateOfFire.SelectedIndex}(RoFBonus) + {num_OtherBonus.NumValue}(Other modifer)";
        }

        List<int> lineraSize = new List<int>()
        {
            2,3,5,7,10,15,20,30,50,70,100,150,200,300,500,700,1000,1500,2000,3000,5000,7000,10000,15000,20000,30000,50000,70000,100000,150000,200000
        };

        private int GetDistanceModifer(int distance)
        {
            int modifer = 0;
            foreach (int size in lineraSize)
            {
                if (size < distance)
                    modifer--;
                else
                    break;
            }
            return modifer;
        }


        
    }
}