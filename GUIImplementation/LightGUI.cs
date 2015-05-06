using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIImplementation
{
    public class LightGUI : AbstractGUIControl
    {
        public override string GuiName { get { return "LightGui"; } }

        protected override Uri GetImageURI(bool isBackground, byte value) {
            if (isBackground)
            {
                return new Uri("pack://application:,,,/GUIImplementation;component/Skins/Light/BackgroundElements/" + value + ".png");
            }
            else
            {
                return new Uri("pack://application:,,,/GUIImplementation;component/Skins/Light/ForegroundElements/" + value + ".png");
            }
        }
    }
}
