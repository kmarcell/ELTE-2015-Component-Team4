namespace GTInterfacesLibrary
{
    public delegate void FieldClickedEventHandler(GTGuiInterface gui, int row, int column);
    public interface GTGuiInterface
    {
        event FieldClickedEventHandler FieldClicked;
        void SetField(byte[,] field);
        void SetFieldBackground(byte[,] field);
        string GuiName { get; }
    }
}
