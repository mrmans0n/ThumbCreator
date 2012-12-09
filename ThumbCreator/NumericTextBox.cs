using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ThumbCreator
{
    public class NumericTextBox : System.Windows.Forms.TextBox
    {
        private bool point = false;
        private bool doublepoint = false;
        private bool allowneg = false;

        public bool AllowNegative
        {
            get { return allowneg; }
            set { allowneg = value; }
        }

        public bool Point
        {
            get { return point; }
            set { point = value; }
        }        

        public bool DoublePoint
        {
            get { return doublepoint; }
            set { doublepoint = value; }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (!(e.KeyChar == '.')) && (!(e.KeyChar == '-')) && (!(e.KeyChar == ':')) && (!(e.KeyChar == '+')) && (!(e.KeyChar == '-')))
            {
                e.Handled = true;
            }

            if (!point && (e.KeyChar == '.'))
                e.Handled = true;

            if (!doublepoint && (e.KeyChar == ':'))
                e.Handled = true;

            if (!allowneg && (e.KeyChar == '-'))
                e.Handled = true;

            if (e.KeyChar != '+' && e.KeyChar != '-')
                base.OnKeyPress(e);
            else
            {
                int bleh = int.Parse(this.Text);
                bleh = (e.KeyChar == '+') ? bleh+1 : 
                    (!allowneg&&bleh==0)? 0 : bleh-1;
                this.Text = bleh.ToString();
                e.Handled = true;
            }
        }
    }
}
