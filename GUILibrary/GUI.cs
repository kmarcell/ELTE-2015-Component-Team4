using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUILibrary
{
    public delegate void FieldClickedEventHandler(GUI gui, int row, int column);
    public interface GUI
    {
        event FieldClickedEventHandler FieldClicked;
        void SetField(byte[,] field);
        void SetFieldBackground(byte[,] field);
    }
}
