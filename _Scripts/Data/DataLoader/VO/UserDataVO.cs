namespace DataLoader.VO
{
    public class UserDataVO: BaseMutilVO
    {
        public UserDataVO()
        {
            LoadDataByDirectories<BaseVO>("UserDatas");
        }
    }
}