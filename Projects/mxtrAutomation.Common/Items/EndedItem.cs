namespace mxtrAutomation.Common.Items
{
    public class EndedItem<T>
    {
        public T Item { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
        public int Index { get; set; }

        public static implicit operator T(EndedItem<T> endedItem)
        {
            return endedItem.Item;
        }
    }
}
