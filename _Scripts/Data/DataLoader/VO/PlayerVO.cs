public class PlayerVO : BaseMutilVO
{
    public PlayerVO()
    {
        LoadDataByDirectories<BaseVO>("Player");
    }
}
