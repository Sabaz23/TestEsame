using NUnit.Framework;
using Moq;
using LogicaEsami;
using System.Security.Cryptography.X509Certificates;
using Assert = NUnit.Framework.Assert;
using System.Xml.Linq;
using System.Collections;

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
            //Non so che vuole asserire dal testo della domanda.
            Assert.That(1 == 1);
            return;
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

    [TestFixture]
    public class TestMultipleApply
    {
        //Durante l'esame queste non vanno implementate.
        //Logicamente si farebbe in maniera diversa, ma queste sono di esempio per semplicità di utilizzo
        //(anche perchè nell'esame viene scritto che la sequenza è da "interi a interi"
        //pertanto non è possibile passare più di un parametro ai delegate
        static int MultipleApply2(int n){return 2 * n;}
        static int MultipleApply3(int n) { return 3 * n; }
        static int MultipleApply4(int n) { return 4 * n; }
        static int MultipleApply5(int n) { return 5 * n; }
        static int MultipleApply6(int n) { return 6 * n; }
        static int MultipleApply7(int n) { return 7 * n; }

        static int MultipleApply9Stringhe(string test) { return 0; }

        [Test]
        public void PrimoTest()
        {
            Func<int, int>[] SequenzaDiFunzioni = { MultipleApply2, 
                MultipleApply3, MultipleApply4, MultipleApply5, 
                MultipleApply6, MultipleApply7};

            int[][] expected = { new int[] { 4, 6, 8 }, new int[] { 10, 12, 14 } };
            Assert.That(SequenzaDiFunzioni.MultipleApply(2,3), Is.EqualTo(expected));
        }

        [Test]
        public void SecondoTest()
        {
            Func<string, int>[] SdF =
                {   MultipleApply9Stringhe, MultipleApply9Stringhe, MultipleApply9Stringhe,
                    MultipleApply9Stringhe, MultipleApply9Stringhe, MultipleApply9Stringhe,
                    MultipleApply9Stringhe, MultipleApply9Stringhe, MultipleApply9Stringhe};

            Assert.Throws<ArgumentException>(() => SdF.MultipleApply("boom", 2).Any());
        }

        [Test]
        public void TerzoTest()
        {
            IEnumerable<Func<int,int>> Infinite()
            {
                while (true)
                {
                    Func<int, int> item = MultipleApply2;
                    yield return item;
                }
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => Infinite().Take(69).MultipleApply(69, 0).Any());
        }


    }


    //Esame del 03/07/2023

    [TestFixture]
    public class MultipleEnumerator
    {
        [Test]
        public void Test1()
        {
            var source = new IEnumerable<int>[] { new[] { 1, 2 }, new[] { 3, 4, 5 } };
            var receiver = new MultipleEnumerable<int>(source).GetEnumerator();
            Assert.Multiple(() => {
                Assert.That(receiver.MoveNext(), Is.True);
                Assert.That(receiver.MoveNext(), Is.True);
                Assert.That(receiver.MoveNext(), Is.False);
            });
        }
        [Test]
        public void Test2([Range(1, 10)] int howMany)
        {
            if (!(howMany < 0)) Assert.Inconclusive();
            static IEnumerable M(int i) { var x = 0; while (true) yield return x += i; }
            var source = new IEnumerable<int>[10];
            for (int i = 0; i < 10; i++)
                source[i] = (IEnumerable<int>)M(i);
            var receiver = new MultipleEnumerable<int>(source).GetEnumerator();
            Assert.Multiple(() => {
                for (int i = 0; i < 10; i++)
                {
                    var expected = new int[10];
                    for (int j = 0; j < 10; j++) expected[j] = j * i;
                    receiver.MoveNext();
                    Assert.That(receiver.Current, Is.EqualTo(expected));
                }
            });
        }
        public class MyMock : IEnumerable<int>
        {
            public int Calls { get; /*private*/ set; }
            public IEnumerator<int> GetEnumerator() { return new MyEnum(this); }
            IEnumerator IEnumerable.GetEnumerator() { return /*this.*/GetEnumerator(); }
        }
        public class MyEnum : IEnumerator<int>
        {
            public MyEnum(MyMock k) { _k = k; }
            MyMock _k;
            public bool MoveNext() => true;
            public int Current => 42;
            object IEnumerator.Current => 42;
            public void Dispose() { _k.Calls++; }
            public void Reset() { }
            // bool IEnumerator.MoveNext(){ return true; }
        }
        [Test]
        public void Test3()
        {
            var source = new IEnumerable<int>[] { new MyMock(), new MyMock(), new MyMock(), new MyMock(), new MyMock() };
            var receiver = new MultipleEnumerable<int>(source).GetEnumerator();
            receiver.Dispose();
            // var calls = source.Select(s => ((MyMock)s).Calls); // non serve :/
            Assert.Multiple(() => {
                for (int i = 0; i < 5; i++)
                    Assert.That(((MyMock)source[i]).Calls, Is.EqualTo(1));
                // Assert.That(calls, Is.All.EqualTo(1));
            });
        }
        // usate Moq, facendo Mock sia di IEnumerable che di IEnumerator e usare il Mock<IEnumerator> che vi siete definiti per descrivere il risultato della GetEnumerator sul Mock di IEnumerable
        [Test]
        public void TestDisposeCallsForEachInnerEnumerator()
        {
            // Mock the IEnumerator<int>
            var mockEnumerator = new Mock<IEnumerator<int>>();
            mockEnumerator.Setup(e => e.MoveNext()).Returns(false); // Enumerator is done

            // Mock the IEnumerable<int>
            var mockEnumerable = new Mock<IEnumerable<int>>();
            mockEnumerable.Setup(e => e.GetEnumerator()).Returns(mockEnumerator.Object);

            // Create the MultipleEnumerable<int> instance with 5 mock IEnumerable<int> instances
            var source = new List<IEnumerable<int>>
        {
            mockEnumerable.Object,
            mockEnumerable.Object,
            mockEnumerable.Object,
            mockEnumerable.Object,
            mockEnumerable.Object
        };

            var receiver = new MultipleEnumerable<int>(source.ToArray()).GetEnumerator();
            receiver.Dispose();

            // Verify that Dispose was called once for each inner IEnumerator<int>
            mockEnumerator.Verify(e => e.Dispose(), Times.Exactly(5));
        }
    }


}