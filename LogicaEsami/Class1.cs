namespace LogicaEsami
{
    //Classi per Esame del 30/01/2023
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

    }






}