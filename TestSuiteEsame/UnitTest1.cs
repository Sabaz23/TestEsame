using NUnit.Framework;
using Moq;
using LogicaEsami;
using System.Security.Cryptography.X509Certificates;
using Assert = NUnit.Framework.Assert;

namespace TestSuiteEsame
{
    //Test per Esame del 30/01/2023

    [TestFixture]
    public class TestsPrimoEsame
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


        [TestCase(2)]
        [TestCase(5)]
        [TestCase(8)]
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(20)]
        [TestCase(25)]
        [TestCase(30)]
        [TestCase(40)]
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


}