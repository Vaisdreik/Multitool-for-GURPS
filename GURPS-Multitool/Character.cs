using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GURPS_Multitool.Controls;

namespace GURPS_Multitool
{
    public class Character
    {
        public string Name { get; set; }
        public int Points { get; set; }
        public int ST { get; set; }
        public int DX { get; set; }
        public int IQ { get; set; }
        public int HT { get; set; }
        public int HP { get; set; }
        public int Will { get; set; }
        public int Per { get; set; }
        public int FP { get; set; }
        public int BS { get; set; }
        public int BM { get; set; }

        public string CraftSkillName { get; set; }
        public int CraftSkill { get; set; }
        public int WP { get; set; }
        public int WCHalfBonus { get; set; }
        public int GadgeteerLVL { get; set; }
        public int SuperJump { get; set; }
        public int ShootingSkill { get; set; }
        public ObservableCollection<CharacterItem> CharItems { get; set; }
        public ObservableCollection<Note> Notes { get; set; }

        public Character()
        {
            Name = "MyCharacter";
            Points = 0;
            ST = 10;
            DX = 10;
            IQ = 10;
            HT = 10;
            HP = 10;
            BS = 5;
            BM = 5;
            Will = 10;
            Per = 10;
            FP = 10;
            CraftSkillName = "навык крафта";
            CraftSkill = 10;
            WP = 0;
            WCHalfBonus = 0;
            GadgeteerLVL = 0;
            SuperJump = 0;
            CharItems = new ObservableCollection<CharacterItem>();
            Notes = new ObservableCollection<Note>();
        }
        public void ChangeST(int value)
        {

        }
        public void ChangeDX(int value)
        {

        }
        public void ChangeIQ(int value)
        {
            int dvalue = value - IQ;
            IQ = value;
            CraftSkill += dvalue;
        }
        public void ChangeHT(int value)
        {

        }
        public void ChangeHP(int value)
        {

        }
        public void ChangeBS(int value)
        {

        }
        public void ChangeBM(int value)
        {

        }
        public void ChangeWill(int value)
        {

        }
        public void ChangePer(int value)
        {

        }
        public void ChangeFP(int value)
        {

        }

        public void ChangeCraftSkill(int value)
        {
            CraftSkill = value;
            int csp = GetCraftSkillPoints();
            if (csp >= 12)
            {
                WP = csp / 12;
                WCHalfBonus = (csp - 12) / 24;
            }
        }

        public int GetCraftSkillPoints()
        {
            int result = 0;
            if ((CraftSkill - IQ) < -1)
            {
                //switch
            }
            else
            {
                result = (CraftSkill - IQ + 2) * 12;
            }
            return result;
        }

        public int GetWildCardSkillPoints(int skillLvl)
        {
            int result = 0;
            if ((skillLvl - DX) < -1)
            {
                //switch
            }
            else
            {
                result = (skillLvl - DX + 2) * 12;
            }
            return result;
        }

        public int GetWildCardWPCounts(int skillLvl)
        {
            int swp = 0;
            int wcsp = GetWildCardSkillPoints(skillLvl);
            if (wcsp >= 12)
                swp = wcsp / 12;
            return swp;
        }

        public int GetWildCardWCHalfBonus(int skillLvl)
        {
            int shb = 0;
            int wcsp = GetWildCardSkillPoints(skillLvl);
            if (wcsp >= 12)
                shb = (wcsp - 12) / 24;
            return shb;
        }
    }
}


