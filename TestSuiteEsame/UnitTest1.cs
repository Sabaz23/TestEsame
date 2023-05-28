using NUnit.Framework;
using Moq;
using LogicaEsami;
using System.Security.Cryptography.X509Certificates;
using Assert = NUnit.Framework.Assert;
using System.Xml.Linq;

namespace TestSuiteEsame
{
    //Test per Esame del 30/01/2023

    [TestFixture]
    public class TestLookup
    {
        [Test]
        public void PrimoTest()
        {
            var source = new Mock<IIdentified>[5];
            for (int i = 0; i < 5; i++)
                source[i] = new Mock<IIdentified>();
            source[0].Setup(x => x.Key).Returns(8);
            source[1].Setup(x => x.Key).Returns(-70);
            source[2].Setup(x => x.Key).Returns(5);
            source[3].Setup(x => x.Key).Returns(7);
            source[4].Setup(x => x.Key).Returns(5);

            var enumerable = source.Select(x => x.Object);
            Assert.That(enumerable.Lookup(8),Is.EqualTo(0));
        }

        [Test]
        public void SecondoTest()
        {
            IEnumerable<IIdentified> Infinite()
            {
                int index = 0;
                while (true)
                {
                    var a = new Mock<IIdentified>();
                    if (index != 41)
                    {
                        Random r = new Random();
                        a.Setup(x => x.Key).Returns(r.Next(41));
                    }
                    else
                    {
                        yield return null;
                    }
                    index++;
                    //Console.WriteLine(index);
                    yield return a.Object;
                }
            }
                Assert.Throws<ArgumentNullException>(() => Infinite().Take(100).Lookup(42));
   
            }


        [TestCase(60)]
        public void TerzoTest(int size)
        {
            if (size < 20) Assert.Inconclusive();
            int what = 10;
            var db = new Mock<IIdentified>[size];
            for (int i = 0; i < size; i++)
            {
                db[i] = new Mock<IIdentified>();
                db[i].Setup(x => x.Key).Returns(i);
            }

            var enumerable = db.Select(x => x.Object);
            enumerable.Lookup(what);

            for(int i=0; i < size;  i++)
            {
                if (i <= what)
                    db[i].VerifyGet(x => x.Key, Times.AtLeastOnce());
                else
                    db[i].VerifyGet(x => x.Key, Times.Never());
            }
        }


    }


    //Test per Filter
    [TestFixture]
    public class TestFilter
    {

        static char OtherChar(char c)
        {
            if (c == 'a') return 'b';
            if (c == 'b') return 'a';
            return 'e';
        }


        static bool onEvenFunction<T>(T argomento)
        {
            return argomento.ToString().Length < 10;
        }

        static bool onOddFunction<T>(T argomento)
        {
            return argomento.ToString().StartsWith('p');
        }

        static bool PredicateFunctionLastTest<T>(T argomento)
        {
            return argomento.ToString().EndsWith('a');
        }     

        [Test]
        public void PrimoTest()
        {
            Predicate<string> onEvenPredicate = onEvenFunction;
            Predicate<string> onOddPredicate = onOddFunction;

            var s = new string[] { "banana", "plutocratico", "questo proprio no", "pera", "questo si", "mela" };

            var result = s.Filter(onEvenPredicate, onOddPredicate);
            var expected = new string[] { "banana", "plutocratico", "pera", "questo si" };
            Assert.AreEqual(result, expected);
        }

        [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13})]
        public void SecondoTest(int[] source)
        {
            if(source.Length < 2) Assert.Inconclusive();
            
            Predicate<int> onEvenPredicate = onEvenFunction;
            Predicate<int> onOddPredicate = onOddFunction;

            var result = source.Filter(onEvenPredicate, onOddPredicate);
            var expected = new int[source.Length / 2 + 1]; //Sicuramente sono la metà quelli pari, + 1 in caso la sequenza fosse dispari
            int index = 0;
            int j = 0;
            while(index < source.Length)
            {
                if (index%2 == 0)
                {
                    expected[j] = source[index];
                    j++;
                }
                index++;
            }

            Assert.AreEqual(result, expected);
        }

        [Test]
        public void TerzoTest()
        {
            Predicate<string> DelegateLastTest = PredicateFunctionLastTest;
            string[] sequence = new string[] {};
            int iteration = 0;
            sequence[0] = "a";
            sequence[1] = "b";
            IEnumerator<string> Infinite()
            {
                sequence[iteration + 2] = sequence[iteration] + OtherChar(sequence[iteration].LastChar());
                iteration++;
                yield return sequence[iteration +1];
            };
            //Non so che vuole asserire dal testo della domanda.
            Assert.That(1 == 1);
        }

        [Test]
        public void QuartoTest()
        {

            Predicate<int> onEvenPredicate = onEvenFunction;
            Predicate<int> onOddPredicate = onOddFunction;
            Assert.Throws<ArgumentNullException>(() => MyExtensions.Filter(null,onEvenPredicate,onOddPredicate).Any());

        }

    }

    [TestFixture]
    public class TestCompareTo
    {
        [Test]
        public void PrimoTest()
        {
            var s = new char[] { 'a', 'b', 'c' };
            char threshold = 'a';
            int howmany = 0;
            Assert.Throws<ArgumentOutOfRangeException>(() => s.EnoughSmaller(threshold, howmany));
        }

        [Test]
        public void SecondoTest()
        {
            var s = new string[] { "asl", "asjdj", "askdnjk", "aksjd", "askjdjk" };
            string threshold = "sium";
            int howmany = 42;
            Assert.IsFalse(s.EnoughSmaller(threshold,howmany));
        }

        [TestCase(73)]
        public void TerzoTest(int n)
        {
            if (n <= 0) Assert.Inconclusive();
            float threshold = 7.42f;

            IEnumerable<double> Infinite()
            {
                Random r = new Random();
                while (true)
                {
                    yield return (double)r.NextDouble() * -1;
                }
            }

            Assert.IsTrue(Infinite().Take(100).EnoughSmaller(threshold, n));

        }

        //Funziona ma non sono sicuro sia esattamente quello che voleva.
        [Test]
        public void QuartoTest()
        {
            var s = new Mock<IComparable>[20];
            //Threshold è a caso
            int threshold = 11;
            for (int i = 0; i < 20; i++)
            {
                s[i] = new Mock<IComparable>();
                //Ogni volta che compara l'item con threshold ritorna -1 (un valore negativo, quindi è per forza minore di threshold)
                s[i].Setup(x => x.CompareTo(threshold)).Returns(-1);
            }
            var sequenceEnum = s.Select(x => x.Object);
            int howmany = 7;
            int count = 0;
            sequenceEnum.EnoughSmaller(threshold, howmany);
            for(int i = 0; i< s.Length; i++)
            { 
                if (i < howmany)
                {
                    s[i].Verify(x => x.CompareTo(threshold), Times.Once());
                    count++;
                }
                else
                    s[i].Verify(x => x.CompareTo(threshold), Times.Never());
                
            }
            Assert.AreEqual(howmany, count);

                
        }

    }


}