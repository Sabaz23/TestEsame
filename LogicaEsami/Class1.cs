using System.Collections;

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


        public static char LastChar(this string s)
        {
            if (s == null) throw new ArgumentNullException();
            return s[s.Length - 1];
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

        //MultipleApply 5/6/2023
        public static IEnumerable<int[]> MultipleApply<T> (this IEnumerable<Func<T,int>> s, T val, int n)
        {
            if(s == null) throw new ArgumentNullException(nameof(s));
            if(n <= 0) throw new ArgumentOutOfRangeException(nameof(s));
            //A quanto pare non c'è un modo per determinare se la sorgente è infinita, quindi uso un valore "dummy".
            //InconsistentSourceException() non avevo voglia di implementarla. Questo fa lo stesso
            if(s.Count() > 1000 || s.Count() % n != 0) throw new ArgumentException("InconsistentSourceException()"); //throw new InconsistentSourceException();
            int i = 0;
            int j = 0;
            int k = 0;
            var arrayToReturn = new int[n];
            bool toReturn = false;
            foreach (var function in s)
            {
                arrayToReturn[j] = function(val); 
                if(((i+1)*n)-1==k) toReturn = true; 
                if(toReturn)
                {
                    yield return arrayToReturn;
                    arrayToReturn = new int[n];
                    toReturn = false;
                    i++;
                    j = 0;
                }
                else j++;
                k++;
            }
        }

        //Esame settembre 2023
        public static IEnumerable<bool> FirstWins<T>(this IEnumerable<T> s) where T: IPlayingCard
        {
            
            if (s == null) throw new ArgumentNullException(nameof(s) + " null");
            if (s.Count() % 6 != 0 || s.Count() == 0) throw new ArgumentException(nameof(s) + " non multiplo di 6");
            
            int i = 0;
            
            IPlayingCard[] currentGame = new IPlayingCard[6];
            
            foreach (T card in s)
            {
                
                if (i < 6)
                {       
                    currentGame[i] = card;   
                    i++;
                }
                else
                {
                    int winningIndex = 0;
                    
                    for(int k =0; k < 6; k++)
                    {
                        if (currentGame[k] >= currentGame[winningIndex])
                        {
                            winningIndex = k;
                            
                        }

                    }

                    Console.WriteLine(winningIndex);
                    if (winningIndex % 2 == 0)
                        yield return true;
                    else
                        yield return false;

                    i = 0;
                }
            }
        }

    }


    //Esame del 03/07/2023
    public class MultipleEnumerable<T> : IEnumerable<T[]>
    {
        IEnumerable<T>[] Source { get; }
        public MultipleEnumerable(IEnumerable<T>[] source)
        {
            Source = source;
        }
        public IEnumerator<T[]> GetEnumerator()
        {
            return new MultipleEnumerator<T>(Source.Select(e => e.GetEnumerator()).ToArray());
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    public class MultipleEnumerator<T> : IEnumerator<T[]>
    {
        IEnumerator<T>[] Enumerators { get; }
        public MultipleEnumerator(IEnumerator<T>[] enumerators)
        {
            Enumerators = enumerators;
        }
        public bool MoveNext()
        {
            foreach (var e in Enumerators)
                if (!(e.MoveNext())) return false;
            return true;
        }
        public void Reset()
        {
            foreach (var e in Enumerators)
                e.Reset();
        }
        public T[] Current
        {
            get { return Enumerators.Select(e => e.Current).ToArray(); }
        }
        object IEnumerator.Current
        {
            get { return /*this.*/Current; }
        }
        public void Dispose()
        {
            foreach (var e in Enumerators)
                e.Dispose();
        }
    }


    //Esame settembre 2023
    public enum Cards { Ace, Two, Three, Four, Five, Six, Seven, Jack, Queen, King }
    public enum Suits { Spades, Clubs, Diamonds, Hearts }
    public interface IPlayingCard
    {
        Cards Value { get; }
        Suits Suit { get; }
        static bool operator <=(IPlayingCard first , IPlayingCard second ) {

            if (first.Value == second.Value)
            {
                if (first.Suit == second.Suit)
                    return true;
                else if (first.Suit < second.Suit)
                    return true;
            }
            else if (first.Value < second.Value)
                return true;

            return false;

        }
        static bool operator >=(IPlayingCard first , IPlayingCard second ) {

            if (first.Value == second.Value)
            {
                if (first.Suit == second.Suit)
                    return true;
                else if (first.Suit > second.Suit)
                    return true;
            }
            else if (first.Value > second.Value)
                return true;

            return false;
        }
    }

 





}