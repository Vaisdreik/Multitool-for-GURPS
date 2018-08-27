using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GURPS_Multitool.Annotations;

namespace GURPS_Multitool
{
    public class Craft
    {
        public string Name { get; set; }
        public bool IsFinished { get; set; }
        public int Points { get; set; }
        public int Complexity { get; set; }
        public int ComplexityPenalty { get; set; }
        public int TechLevel { get; set; }
        public int TechLevelCampain { get; set; }

        public int ConceptTimeBonus { get; set; }
        public int ConceptMultiplier { get; set; }
        public int ConceptDiceResult { get; set; }
        public int ConceptTimeTotal { get; set; }
        public bool ConceptIsUseWCHalfBonus { get; set; }
        public int ConceptHelpersBonus { get; set; }
        public int ConceptWP { get; set; }
        public int ConceptEffectiveSkill { get; set; }

        public int PrototypeTimeBonus { get; set; }
        public int PrototypeMultiplier { get; set; }
        public int PrototypeDiceResult { get; set; }
        public int PrototypeTimeTotal { get; set; }
        public bool PrototypeIsUseWCHalfBonus { get; set; }
        public int PrototypeHelpersBonus { get; set; }
        public int PrototypeWP { get; set; }
        public int PrototypeEffectiveSkill { get; set; }

        public Craft()
        {
            Name = "typical electronic";
            IsFinished = false;
            Points = 0;
            Complexity = 600;
            ComplexityPenalty = 0;
            TechLevel = 9;
            TechLevelCampain = 8;

            ConceptTimeBonus = 0;
            ConceptMultiplier = 1;
            ConceptDiceResult = 0;
            ConceptTimeTotal = 0;
            ConceptIsUseWCHalfBonus = false;
            ConceptHelpersBonus = 0;
            ConceptWP = 0;
            ConceptEffectiveSkill = 0;

            PrototypeTimeBonus = 0;
            PrototypeMultiplier = 1;
            PrototypeDiceResult = 0;
            PrototypeTimeTotal = 0;
            PrototypeIsUseWCHalfBonus = false;
            PrototypeHelpersBonus = 0;
            PrototypeWP = 0;
            PrototypeEffectiveSkill = 0;
        }

        public void SetConceptTimeBonus(int bonus)
        {
            ConceptTimeBonus = bonus;
            switch (bonus)
            {
                case 1:
                    ConceptMultiplier = 2;
                    break;
                case 2:
                    ConceptMultiplier = 4;
                    break;
                case 3:
                    ConceptMultiplier = 8;
                    break;
                case 4:
                    ConceptMultiplier = 15;
                    break;
                case 5:
                    ConceptMultiplier = 30;
                    break;
                default:
                    ConceptMultiplier = 1;
                    break;
            }
        }

        public void SetPrototypetTimeBonus(int bonus)
        {
            PrototypeTimeBonus = bonus;
            switch (bonus)
            {
                case 1:
                    PrototypeMultiplier = 2;
                    break;
                case 2:
                    PrototypeMultiplier = 4;
                    break;
                case 3:
                    PrototypeMultiplier = 8;
                    break;
                case 4:
                    PrototypeMultiplier = 15;
                    break;
                case 5:
                    PrototypeMultiplier = 30;
                    break;
                default:
                    PrototypeMultiplier = 1;
                    break;
            }
        }
    }
}
