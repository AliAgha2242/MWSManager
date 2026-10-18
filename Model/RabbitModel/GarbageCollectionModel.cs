namespace MWSMan.Core.Model.RabbitModel
{
    public class GarbageCollectionModel
    {
        public int FullsweepAfter { get; set; }
        public int MaxHeapSize { get; set; }
        public int MinBinVheapSize { get; set; }
        public int MinHeapSize { get; set; }
        public int MinorGcs { get; set; }
    }
}
