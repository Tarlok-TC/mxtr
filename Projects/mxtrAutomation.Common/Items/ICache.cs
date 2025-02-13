namespace mxtrAutomation.Common.Items
{
    public interface ICache<in TKey, out TValue>
    {
        TValue Get(TKey key);
        void Clear(TKey key);
        void Clear();
    }

    public interface ICacheDictionary<in TKey, TValue>
    {
        void Add(TKey key, TValue value);
        TValue Get(TKey key);
        void Remove(TKey key);
        bool ContainsKey(TKey key);
        void Clear();
    }    
}
