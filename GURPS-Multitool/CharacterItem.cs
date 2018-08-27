using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GURPS_Multitool
{
    public class CharacterItem
    {
        private int _count;
        private string _header;
        private string _description;

        public CharacterItem()
        {
            _count = 0;
            _header = "Header";
            _description = "Description";
        }

        public CharacterItem(int count, string header, string description)
        {
            _count = count;
            _header = header;
            _description = description;
        }

        public int ItemsCount
        {
            get { return _count; }
            set { _count = value; }
        }

        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        
    }
}
