using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUIImplementation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        byte[,] damaBackGround = { 
                { 12, 11, 12, 11, 12, 11, 12, 11 }, 
                { 11, 12, 11, 12, 11, 12, 11, 12 },
                { 12, 11, 12, 11, 12, 11, 12, 11 }, 
                { 11, 12, 11, 12, 11, 12, 11, 12 },
                { 12, 11, 12, 11, 12, 11, 12, 11 }, 
                { 11, 12, 11, 12, 11, 12, 11, 12 },
                { 12, 11, 12, 11, 12, 11, 12, 11 }, 
                { 11, 12, 11, 12, 11, 12, 11, 12 }};

        byte[,] malomBackGround = { 
                { 0, 9, 9, 4, 9, 9, 1 }, 
                { 10, 0, 9, 8, 9, 1, 10 },
                { 10, 10, 0, 6, 1, 10, 10 }, 
                { 7, 8, 5, 11, 7, 8, 5 },
                { 10, 10, 3, 4, 2, 10, 10 }, 
                { 10, 3, 9, 8, 9, 2, 10 },
                { 3, 9, 9, 6, 9, 9, 2 }};
        byte[,] malomField = { 
                { 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0 }};
        byte[,] damaField = { 
                { 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 }};
        public MainWindow()
        {

            
            InitializeComponent();
            
            
            this.Width = this.Height = 1150;
            //gui.Background = new SolidColorBrush(Color.FromRgb(0,0,0));
            damagui.Width = damagui.Height = 500;
            damagui.SetFieldBackground(damaBackGround);
            damagui.SetField(damaField);
            damagui.InvalidateVisual();
            damagui.FieldClicked += damagui_FieldClicked;
            malomgui.Width = malomgui.Height = 500;
            malomgui.SetFieldBackground(malomBackGround);
            malomgui.SetField(malomField);
            malomgui.InvalidateVisual();
            malomgui.FieldClicked += malomgui_FieldClicked;

        }

        void damagui_FieldClicked(GUILibrary.GUI gui, int row, int column)
        {
            damaField[row, column] = (byte)((damaField[row, column] + 1) % 3);
            gui.SetField(damaField);
        }

        void malomgui_FieldClicked(GUILibrary.GUI gui, int row, int column)
        {
            malomField[row, column] = (byte)((malomField[row, column] + 1) % 3);
            gui.SetField(malomField);
        }
    }
}
