namespace DataLoader.VO
{
    public class MainmenuCharacter3DVO : BaseMutilVO
    {
        public MainmenuCharacter3DVO()
        {
            LoadDataByDirectories<BaseVO>("Mainmenu");
        }
    }
}