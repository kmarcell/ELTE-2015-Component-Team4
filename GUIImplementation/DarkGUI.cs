using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIImplementation
{
    public class DarkGUI : AbstractGUIControl
    {
        public override string GuiName { get { return "DarkGui"; } }

        protected override Uri GetImageURI(bool isBackground, byte value) {
            if (isBackground)
            {
                return new Uri("pack://application:,,,/GUIImplementation;component/Skins/Dark/BackgroundElements/" + value + ".png");
            }
            else
            {
                return new Uri("pack://application:,,,/GUIImplementation;component/Skins/Dark/ForegroundElements/" + value + ".png");
            }
        }
    }
}
