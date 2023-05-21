namespace LogicaEsami
{
    //Classi per Lookup
    public interface IIdentified
    {
        public int Key { get; }
    }


    public static class MyExtensions
    {
        //LookUp 30/01/2023
        public static int? Lookup<T>(this IEnumerable<T> enumerable, int what) where T : IIdentified
        {
            if (enumerable is null) throw new ArgumentNullException("Sequence to LookUp is null");
            int index = 0;
            foreach(T item in enumerable)
            {
                if (item is null) throw new ArgumentNullException("Item is null");
                if (item.Key == what)
                {
                    return index;
                }
                index++;
            }
            return null;
        }

        //Filter 12/05/2023
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> s, Predicate<T> onEvenPredicate, Predicate<T> onOddPredicate)
        {
            if(s == null) throw new ArgumentNullException(nameof(s), "s can't be null");
            bool isEven = true;
            foreach(T item in s)
            {
                if(isEven && onEvenPredicate(item)) yield return item;
                if(!isEven && onOddPredicate(item)) yield return item;
                isEven = !isEven;
            }
        }

        //CompareTo 16/02/2023
        public static bool EnoughSmaller<T>(this IEnumerable<T> s, T threshold, int howMany) where T : IComparable
        {
            if (s == null || threshold == null) throw new ArgumentNullException("S or Threshold can't be null.");
            if (howMany <= 0) throw new ArgumentOutOfRangeException(nameof(howMany), "Howmany must be positive");
            int minorCount = 0;
            foreach(T item in s)
            {
                if(item.CompareTo(threshold) < 0)
                    minorCount++;
                if(minorCount == howMany)
                    return true;
            }
            return false;
        }


    }






}