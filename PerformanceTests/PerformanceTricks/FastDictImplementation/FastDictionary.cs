using System.Collections;

namespace Implementation;

public class FastDictionary<TKey, TValue, TComparer>
    : Dictionary<TKey, TValue> 
    where TKey : notnull
    where TComparer : struct, IEqualityComparer<TKey>
{
    public FastDictionary(TComparer comparer) : base(comparer)
    {
        
    }
}
