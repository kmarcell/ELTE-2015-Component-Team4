using GTInterfacesLibrary;
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
using FieldClickedEventHandler = GTInterfacesLibrary.FieldClickedEventHandler;

namespace GUIImplementation
{
    /// <summary>
    /// Interaction logic for AbstractGUIControl.xaml
    /// </summary>
    public partial class AbstractGUIControl : UserControl, GTGuiInterface
    {
        public event FieldClickedEventHandler FieldClicked;
        private List<List<System.Windows.Controls.Image>> field;
        private List<List<System.Windows.Controls.Image>> background;
        public AbstractGUIControl()
        {
            InitializeComponent();
        }

        protected virtual Uri GetImageURI(bool isBackground, byte value) { throw new NotImplementedException(); }

        public void SetField(byte[,] field)
        {
            if (field != null && this.field != null){
                if (field.GetLength(0)== this.field.Count() && field.GetLength(1) == this.field.Count())
                {

                } else {
                    DeleteField();
                    CreateField(field);
                }
            }
            else
            {
                CreateField(field);
            }
            RefreshField(field);
        }

        private void RefreshField(byte[,] field)
        {
            int row = field.GetLength(0);
            int col = field.GetLength(1);

            Dispatcher.Invoke(() =>
            {
                int height = (int) Height/row;
                int width = (int) Width/col;
                for (int i = 0; i < row; ++i)
                {
                    for (int j = 0; j < col; ++j)
                    {
                        ExtendedImage label = (ExtendedImage) this.field[i][j];
                        label.Stretch = Stretch.Fill;
                        label.StretchDirection = StretchDirection.Both;
                        label.Height = height;
                        label.Width = width;
                        label.Source = new BitmapImage(GetImageURI(false, field[i, j]));
                        Canvas.SetTop(label, i*height);
                        Canvas.SetLeft(label, j*width);
                    }
                }
            });
        }

        private void DeleteField()
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < field.Count; ++i)
                {
                    for (int j = 0; j < field[i].Count; ++j)
                    {

                        this.RemoveVisualChild(field[i][j]);
                        field[i][j] = null;
                    }
                    field[i] = null;
                }
                field = null;
            });
        }

        private void CreateField(byte[,] field)
        {
            int row = field.GetLength(0);
            int col = field.GetLength(1);

            Dispatcher.Invoke(() =>
            {
                int height = (int) Height/row;
                int width = (int) Width/col;
                this.field = new List<List<Image>>(row);
                for (int i = 0; i < row; ++i)
                {
                    List<Image> rowList = new List<Image>(col);
                    this.field.Add(rowList);
                    for (int j = 0; j < col; ++j)
                    {
                        ExtendedImage label = new ExtendedImage();
                        rowList.Add(label);
                        label.Row = i;
                        label.Column = j;
                        grid.Children.Add(label);
                        label.MouseLeftButtonUp +=
                            new MouseButtonEventHandler(delegate(Object o, MouseButtonEventArgs a)
                            {
                                ExtendedImage img = (ExtendedImage) a.Source;
                                FieldClicked(this, img.Row, img.Column);
                            });
                    }
                }
            });
        }

        void label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }


        public void SetFieldBackground(byte[,] field)
        {
            Dispatcher.Invoke(() =>
            {
                if (field != null && this.field != null)
                {
                    if (field.GetLength(0) == this.background.Count() && field.GetLength(1) == this.background.Count())
                    {

                    }
                    else
                    {
                        DeleteBackground();
                        CreateBackGround(field);
                    }
                }
                else
                {
                    CreateBackGround(field);
                }
                RefreshBackGround(field);
            });
        }

        public virtual string GuiName { get { throw new NotImplementedException(); } }

        private void DeleteBackground()
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < background.Count; ++i)
                {
                    for (int j = 0; j < background[i].Count; ++j)
                    {

                        this.RemoveVisualChild(background[i][j]);
                        background[i][j] = null;
                    }
                    background[i] = null;
                }
                background = null;
            });
        }

        private void CreateBackGround(byte[,] field)
        {
            int row = field.GetLength(0);
            int col = field.GetLength(1);

            Dispatcher.Invoke(() =>
            {
                int height = (int)Height / row;
                int width = (int)Width / col;
                this.background = new List<List<Image>>(row);
                for (int i = 0; i < row; ++i)
                {
                    List<Image> rowList = new List<Image>(col);
                    this.background.Add(rowList);
                    for (int j = 0; j < col; ++j)
                    {
                        System.Windows.Controls.Image label = new System.Windows.Controls.Image();
                        rowList.Add(label);
                        grid.Children.Add(label);
                    }
                }
            });
        }

        private void RefreshBackGround(byte[,] field)
        {
            int row = field.GetLength(0);
            int col = field.GetLength(1);

            Dispatcher.Invoke(() =>
            {
                int height = (int) Height/row;
                int width = (int) Width/col;
                for (int i = 0; i < row; ++i)
                {
                    for (int j = 0; j < col; ++j)
                    {
                        System.Windows.Controls.Image label = this.background[i][j];
                        label.Stretch = Stretch.Fill;
                        label.StretchDirection = StretchDirection.Both;
                        label.Height = height;
                        label.Width = width;
                        label.Source = new BitmapImage(GetImageURI(true, field[i, j]));
                        Canvas.SetTop(label, i*height);
                        Canvas.SetLeft(label, j*width);
                    }
                }
            });
        }
    }
}
