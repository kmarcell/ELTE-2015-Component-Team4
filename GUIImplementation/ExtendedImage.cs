using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIImplementation
{
    class ExtendedImage : System.Windows.Controls.Image
    {
        private int row;

        public int Row
        {
            get { return row; }
            set { row = value; }
        }

        private int col;

        public int Column
        {
            get { return col; }
            set { col = value; }
        }
        
    }
}
