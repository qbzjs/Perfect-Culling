namespace DataLoader.VO
{
    public class ItemInventoryVO : BaseMutilVO
    {
        public ItemInventoryVO()
        {
            LoadDataByDirectories<BaseVO>("ItemInventory");
        }
    }
}