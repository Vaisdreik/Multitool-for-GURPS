using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GURPS_Multitool
{
    public class Note
    {
        private string _title;
        private string _text;
        private string _date;

        public Note()
        {
            _title = "";
            _text = "";
            _date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
        }

        public Note(string title, string text, string date)
        {
            _title = title;
            _text = text;
            _date = date;
        }

        public void ChangeNote(string title, string text, string date)
        {
            _title = title;
            _text = text;
            _date = date;
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public string DateTimeShort
        {
            get { return _date; }
            set { _date = value; }
        }

    }
}
