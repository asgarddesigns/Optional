using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if !NETSTANDARD10
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace Optional.Tests
{
    [TestClass]
    public class NewEitherTests
    {
        [TestMethod]
        public void Either_CreateAndCheckExistence()
        {
            var noneStruct = Either.Left<string, int>("ex");
            var noneNullable = Either.Left<string, int?>("ex");
            var noneClass = Either.Left<string, string>("ex");

            Assert.IsFalse(noneStruct.IsRight);
            Assert.IsFalse(noneNullable.IsRight);
            Assert.IsFalse(noneClass.IsRight);

            Assert.IsTrue(noneStruct.IsLeft);
            Assert.IsTrue(noneNullable.IsLeft);
            Assert.IsTrue(noneClass.IsLeft);
            
            var someStruct = Either.Right<string, int>(1);
            var someNullable = Either.Right<string,int?>(1);
            var someNullableEmpty = Either.Right<string, int?>(null);
            var someClass = Either.Right<string, string>("1");
            var someClassNull = Either.Right<string, string>(null);

            Assert.IsTrue(someStruct.IsRight);
            Assert.IsTrue(someNullable.IsRight);
            Assert.IsTrue(someNullableEmpty.IsRight);
            Assert.IsTrue(someClass.IsRight);
            Assert.IsTrue(someClassNull.IsRight);
            
            Assert.IsFalse(someStruct.IsLeft);
            Assert.IsFalse(someNullable.IsLeft);
            Assert.IsFalse(someNullableEmpty.IsLeft);
            Assert.IsFalse(someClass.IsLeft);
            Assert.IsFalse(someClassNull.IsLeft);
        }

        [TestMethod]
        public void Either_CheckContainment()
        {
            var noneStruct = Either.Left<string, int>("ex");
            var noneNullable = Either.Left<string, int?>("ex");
            var noneClass = Either.Left<string, string>("ex");

            Assert.IsFalse(noneStruct.Contains(0));
            Assert.IsFalse(noneNullable.Contains(null));
            Assert.IsFalse(noneClass.Contains(null));

            Assert.IsFalse(noneStruct.Exists(val => true));
            Assert.IsFalse(noneNullable.Exists(val => true));
            Assert.IsFalse(noneClass.Exists(val => true));

            var someStruct = Either.Right<string, int>(1);
            var someNullable = Either.Right<string,int?>(1);
            var someNullableEmpty = Either.Right<string, int?>(null);
            var someClass = Either.Right<string, string>("1");
            var someClassNull = Either.Right<string, string>(null);

            Assert.IsTrue(someStruct.Contains(1));
            Assert.IsTrue(someNullable.Contains(1));
            Assert.IsTrue(someNullableEmpty.Contains(null));
            Assert.IsTrue(someClass.Contains("1"));
            Assert.IsTrue(someClassNull.Contains(null));

            Assert.IsTrue(someStruct.Exists(val => val == 1));
            Assert.IsTrue(someNullable.Exists(val => val == 1));
            Assert.IsTrue(someNullableEmpty.Exists(val => val == null));
            Assert.IsTrue(someClass.Exists(val => val == "1"));
            Assert.IsTrue(someClassNull.Exists(val => val == null));

            Assert.IsFalse(someStruct.Contains(-1));
            Assert.IsFalse(someNullable.Contains(-1));
            Assert.IsFalse(someNullableEmpty.Contains(1));
            Assert.IsFalse(someClass.Contains("-1"));
            Assert.IsFalse(someClassNull.Contains("1"));

            Assert.IsFalse(someStruct.Exists(val => val != 1));
            Assert.IsFalse(someNullable.Exists(val => val != 1));
            Assert.IsFalse(someNullableEmpty.Exists(val => val != null));
            Assert.IsFalse(someClass.Exists(val => val != "1"));
            Assert.IsFalse(someClassNull.Exists(val => val != null));
        }

        [TestMethod]
        public void Either_Equality()
        {
            // Basic equality
            Assert.AreEqual(Either.Left<string, string>("ex"), Either.Left<string, string>("ex"));
            Assert.AreEqual(Either.Left<string, string>(null), Either.Left<string, string>(null));
            Assert.AreNotEqual(Either.Left<string, string>("ex"), Either.Left<string, string>(null));
            Assert.AreNotEqual(Either.Left<string, string>(null), Either.Left<string, string>("ex"));
            Assert.AreNotEqual(Either.Left<string, string>("ex"), Either.Left<string, string>("ex1"));

            Assert.AreEqual(Either.Right<string, string>("val"), Either.Right<string, string>("val"));
            Assert.AreEqual(Either.Right<string, string>(null), Either.Right<string, string>(null));
            Assert.AreNotEqual(Either.Right<string, string>("val"), Either.Right<string, string>(null));
            Assert.AreNotEqual(Either.Right<string, string>(null), Either.Right<string, string>("val"));
            Assert.AreNotEqual(Either.Right<string, string>("val"), Either.Right<string, string>("val1"));

            // Must have same types
            Assert.AreNotEqual(Either.Left<string, string>("ex"), Either.Left<object, string>("ex"));
            Assert.AreNotEqual(Either.Left<string, string>("ex"), Either.Left<string, object>("ex"));
            Assert.AreNotEqual(Either.Right<string, string>("val"), Either.Right<object, string>("val"));
            Assert.AreNotEqual(Either.Right<string, string>("val"), Either.Right<string, object>("val"));

            // Some and None are different
            Assert.AreNotEqual(Either.Right<string, string>("ex"), Either.Left<string, string>("ex"));
            Assert.AreNotEqual(Either.Right<string, string>(null), Either.Left<string, string>(null));

            // Works for val. types, nullables and ref. types
            Assert.AreEqual(Either.Left<int, int>(1), Either.Left<int, int>(1));
            Assert.AreEqual(Either.Left<int?, int?>(1), Either.Left<int?, int?>(1));
            Assert.AreEqual(Either.Left<string, string>("1"), Either.Left<string, string>("1"));
            Assert.AreNotEqual(Either.Left<int, int>(1), Either.Left<int, int>(-1));
            Assert.AreNotEqual(Either.Left<int?, int?>(1), Either.Left<int?, int?>(-1));
            Assert.AreNotEqual(Either.Left<string, string>("1"), Either.Left<string, string>("-1"));

            Assert.AreEqual(Either.Right<int, int>(1), Either.Right<int, int>(1));
            Assert.AreEqual(Either.Right<int?, int?>(1), Either.Right<int?, int?>(1));
            Assert.AreEqual(Either.Right<string, string>("1"), Either.Right<string, string>("1"));
            Assert.AreNotEqual(Either.Right<int, int>(1), Either.Right<int, int>(-1));
            Assert.AreNotEqual(Either.Right<int?, int?>(1), Either.Right<int?, int?>(-1));
            Assert.AreNotEqual(Either.Right<string, string>("1"), Either.Right<string, string>("-1"));

            // Works when when boxed
            Assert.AreEqual((object)Either.Left<int, int>(1), (object)Either.Left<int, int>(1));
            Assert.AreEqual((object)Either.Right<int, int>(22), (object)Either.Right<int, int>(22));
            Assert.AreNotEqual((object)Either.Right<int, int>(21), (object)Either.Right<int, int>(22));
            Assert.AreNotEqual((object)Either.Left<int, int>(21), (object)Either.Left<int, int>(22));
            Assert.AreNotEqual((object)Either.Left<int, int>(1), (object)Either.Right<int, int>(22));

            // Works with default equalitycomparer 
            Assert.IsTrue(EqualityComparer<Either<int, int>>.Default.Equals(Either.Left<int, int>(1), Either.Left<int, int>(1)));
            Assert.IsTrue(EqualityComparer<Either<int, int>>.Default.Equals(Either.Right<int, int>(22), Either.Right<int, int>(22)));
            Assert.IsFalse(EqualityComparer<Either<int, int>>.Default.Equals(Either.Right<int, int>(22), Either.Right<int, int>(21)));
            Assert.IsFalse(EqualityComparer<Either<int, int>>.Default.Equals(Either.Left<int, int>(22), Either.Left<int, int>(21)));
            Assert.IsFalse(EqualityComparer<Either<int, int>>.Default.Equals(Either.Right<int, int>(22), Either.Left<int, int>(1)));

            // Works with equality operators
            Assert.IsTrue(Either.Left<int, int>(1) == Either.Left<int, int>(1));
            Assert.IsTrue(Either.Right<int, int>(22) == Either.Right<int, int>(22));
            Assert.IsTrue(Either.Left<int, int>(2) != Either.Left<int, int>(1));
            Assert.IsTrue(Either.Right<int, int>(22) != Either.Left<int, int>(1));
            Assert.IsTrue(Either.Right<int, int>(22) != Either.Right<int, int>(21));
        }

        [TestMethod]
        public void Either_CompareTo()
        {
            void LessThan<TRight, TLeft>(Either<TRight, TLeft> lesser, Either<TRight, TLeft> greater)
            {
                Assert.IsTrue(lesser.CompareTo(greater) < 0);
                Assert.IsTrue(greater.CompareTo(lesser) > 0);
                Assert.IsTrue(lesser < greater);
                Assert.IsTrue(lesser <= greater);
                Assert.IsTrue(greater > lesser);
                Assert.IsTrue(greater >= lesser);
            }

            void EqualTo<TRight, TLeft>(Either<TRight, TLeft> left, Either<TRight, TLeft> right)
            {
                Assert.IsTrue(left.CompareTo(right) == 0);
                Assert.IsTrue(right.CompareTo(left) == 0);
                Assert.IsTrue(left <= right);
                Assert.IsTrue(left >= right);
                Assert.IsTrue(right <= left);
                Assert.IsTrue(right >= left);
            }

            // Value type comparisons
            var noneStruct1 = Either.Left<int, int>(1);
            var noneStruct2 = Either.Left<int, int>(2);
            var someStruct1 = Either.Right<int, int>(1);
            var someStruct2 = Either.Right<int, int>(2);

            LessThan(noneStruct1, noneStruct2);
            LessThan(someStruct1, someStruct2);

            LessThan(noneStruct1, someStruct1);
            LessThan(noneStruct1, someStruct2);
            LessThan(noneStruct2, someStruct1);
            LessThan(noneStruct2, someStruct2);

            EqualTo(noneStruct1, noneStruct1);
            EqualTo(someStruct1, someStruct1);

            // IComparable comparisons
            var noneComparableNull = Either.Left<string, string>(null);
            var noneComparable1 = Either.Left<string, string>("1");
            var noneComparable2 = Either.Left<string, string>("2");
            var someComparableNull = Either.Right<string, string>(null);
            var someComparable1 = Either.Right<string, string>("1");
            var someComparable2 = Either.Right<string, string>("2");

            LessThan(noneComparableNull, noneComparable1);
            LessThan(noneComparable1, noneComparable2);
            LessThan(someComparableNull, someComparable1);
            LessThan(someComparable1, someComparable2);

            LessThan(noneComparable1, someComparable1);
            LessThan(noneComparable1, someComparable2);
            LessThan(noneComparable2, someComparable1);
            LessThan(noneComparable2, someComparable2);

            LessThan(noneComparableNull, someComparableNull);
            LessThan(noneComparableNull, someComparable1);
            LessThan(noneComparable1, someComparableNull);

            EqualTo(noneComparableNull, noneComparableNull);
            EqualTo(noneComparable1, noneComparable1);
            EqualTo(someComparableNull, someComparableNull);
            EqualTo(someComparable1, someComparable1);

            // Non-IComparable comparisons
            var noneNotComparableNull = Either.Left<Dictionary<string, string>, Dictionary<string, string>>(null);
            var noneNotComparable1 = Either.Left<Dictionary<string, string>, Dictionary<string, string>>(new Dictionary<string, string>());
            var noneNotComparable2 = Either.Left<Dictionary<string, string>, Dictionary<string, string>>(new Dictionary<string, string>());
            var someNotComparableNull = Either.Right<Dictionary<string, string>, Dictionary<string, string>>(null);
            var someNotComparable1 = Either.Right<Dictionary<string, string>, Dictionary<string, string>>(new Dictionary<string, string>());
            var someNotComparable2 = Either.Right<Dictionary<string, string>, Dictionary<string, string>>(new Dictionary<string, string>());

            Assert.ThrowsException<ArgumentException>(() => someNotComparable1.CompareTo(someNotComparable2));
            Assert.ThrowsException<ArgumentException>(() => someNotComparable2.CompareTo(someNotComparable1));
            Assert.ThrowsException<ArgumentException>(() => noneNotComparable1.CompareTo(noneNotComparable2));
            Assert.ThrowsException<ArgumentException>(() => noneNotComparable2.CompareTo(noneNotComparable1));

            LessThan(noneNotComparableNull, noneNotComparable1);
            LessThan(someNotComparableNull, someNotComparable1);

            LessThan(noneNotComparable1, someNotComparable1);
            LessThan(noneNotComparable1, someNotComparable2);
            LessThan(noneNotComparable2, someNotComparable1);
            LessThan(noneNotComparable2, someNotComparable2);

            LessThan(noneNotComparableNull, someNotComparableNull);
            LessThan(noneNotComparableNull, someNotComparable1);
            LessThan(noneNotComparable1, someNotComparableNull);

            EqualTo(noneNotComparableNull, noneNotComparableNull);
            EqualTo(noneNotComparable1, noneNotComparable1);
            EqualTo(someNotComparableNull, someNotComparableNull);
            EqualTo(someNotComparable1, someNotComparable1);
        }

        [TestMethod]
        public void Either_Hashing()
        {
            Assert.AreEqual(Either.Left<string, string>("ex").GetHashCode(), Either.Left<string, string>("ex").GetHashCode());
            Assert.AreEqual(Either.Left<string, object>("ex").GetHashCode(), Either.Left<string, object>("ex").GetHashCode());

            Assert.AreEqual(Either.Right<string, string>("val").GetHashCode(), Either.Right<string, string>("val").GetHashCode());
            Assert.AreEqual(Either.Right<string, object>("val").GetHashCode(), Either.Right<string, object>("val").GetHashCode());

            Assert.AreEqual(Either.Left<string, string>(null).GetHashCode(), Either.Left<string, string>(null).GetHashCode());
            Assert.AreEqual(Either.Left<string, object>(null).GetHashCode(), Either.Left<string, object>(null).GetHashCode());

            Assert.AreEqual(Either.Right<string, string>(null).GetHashCode(), Either.Right<string, string>(null).GetHashCode());
            Assert.AreEqual(Either.Right<string, object>(null).GetHashCode(), Either.Right<string, object>(null).GetHashCode());

            Assert.AreNotEqual(Either.Right<string, string>(null).GetHashCode(), Either.Left<string, string>(null).GetHashCode());
            Assert.AreNotEqual(Either.Right<string, object>(null).GetHashCode(), Either.Left<string, object>(null).GetHashCode());
        }

        [TestMethod]
        public void Either_StringRepresentation()
        {
            Assert.AreEqual(Either.Left<int?, int?>(null).ToString(), "Left(null)");
            Assert.AreEqual(Either.Left<string, string>(null).ToString(), "Left(null)");

            Assert.AreEqual(Either.Left<int, int>(1).ToString(), "Left(1)");
            Assert.AreEqual(Either.Left<int?, int?>(1).ToString(), "Left(1)");
            Assert.AreEqual(Either.Left<string, string>("ex").ToString(), "Left(ex)");

            Assert.AreEqual(Either.Right<string, int?>(null).ToString(), "Right(null)");
            Assert.AreEqual(Either.Right<string, string>(null).ToString(), "Right(null)");

            Assert.AreEqual(Either.Right<string, int>(1).ToString(), "Right(1)");
            Assert.AreEqual(Either.Right<string, int?>(1).ToString(), "Right(1)");
            Assert.AreEqual(Either.Right<string, string>("1").ToString(), "Right(1)");

            var now = DateTime.Now;
            Assert.AreEqual(Either.Right<DateTime, DateTime>(now).ToString(), "Right(" + now.ToString() + ")");
            Assert.AreEqual(Either.Left<DateTime, DateTime>(now).ToString(), "Left(" + now.ToString() + ")");
        }

        [TestMethod]
        public void Either_GetValue()
        {
            var noneStruct = Either.Left<string, int>("ex");
            var noneNullable = Either.Left<string, int?>("ex");
            var noneClass = Either.Left<string, string>("ex");

            Assert.AreEqual(noneStruct.RightOr(-1), -1);
            Assert.AreEqual(noneNullable.RightOr(-1), -1);
            Assert.AreEqual(noneClass.RightOr("-1"), "-1");

            var someStruct = Either.Right<string, int>(1);
            var someNullable = Either.Right<string, int?>(1);
            var someNullableEmpty = Either.Right<string, int?>(null);
            var someClass = Either.Right<string, string>("1");
            var someClassNull = Either.Right<string, string>(null);

            Assert.AreEqual(someStruct.RightOr(-1), 1);
            Assert.AreEqual(someNullable.RightOr(-1), 1);
            Assert.AreEqual(someNullableEmpty.RightOr(-1), null);
            Assert.AreEqual(someClass.RightOr("-1"), "1");
            Assert.AreEqual(someClassNull.RightOr("-1"), null);
        }

        [TestMethod]
        public void Either_GetValueLazy()
        {
            var noneStruct = Either.Left<string, int>("ex");
            var noneNullable = Either.Left<string, int?>("ex");
            var noneClass = Either.Left<string, string>("ex");

            Assert.AreEqual(noneStruct.RightOr(() => -1), -1);
            Assert.AreEqual(noneNullable.RightOr(() => -1), -1);
            Assert.AreEqual(noneClass.RightOr(() => "-1"), "-1");

            Assert.AreEqual(noneStruct.RightOr(ex => ex.GetHashCode()), "ex".GetHashCode());
            Assert.AreEqual(noneNullable.RightOr(ex => ex.GetHashCode()), "ex".GetHashCode());
            Assert.AreEqual(noneClass.RightOr(ex => ex), "ex");

            var someStruct = Either.Right<string, int>(1);
            var someNullable = Either.Right<string, int?>(1);
            var someNullableEmpty = Either.Right<string, int?>(null);
            var someClass = Either.Right<string, string>("1");
            var someClassNull = Either.Right<string, string>(null);

            Assert.AreEqual(someStruct.RightOr(() => -1), 1);
            Assert.AreEqual(someNullable.RightOr(() => -1), 1);
            Assert.AreEqual(someNullableEmpty.RightOr(() => -1), null);
            Assert.AreEqual(someClass.RightOr(() => "-1"), "1");
            Assert.AreEqual(someClassNull.RightOr(() => "-1"), null);

            Assert.AreEqual(someStruct.RightOr(ex => ex.GetHashCode()), 1);
            Assert.AreEqual(someNullable.RightOr(ex => ex.GetHashCode()), 1);
            Assert.AreEqual(someNullableEmpty.RightOr(ex => ex.GetHashCode()), null);
            Assert.AreEqual(someClass.RightOr(ex => ex), "1");
            Assert.AreEqual(someClassNull.RightOr(ex => ex), null);

            Assert.AreEqual(someStruct.RightOr(() => { Assert.Fail(); return -1; }), 1);
            Assert.AreEqual(someNullable.RightOr(() => { Assert.Fail(); return -1; }), 1);
            Assert.AreEqual(someNullableEmpty.RightOr(() => { Assert.Fail(); return -1; }), null);
            Assert.AreEqual(someClass.RightOr(() => { Assert.Fail(); return "-1"; }), "1");
            Assert.AreEqual(someClassNull.RightOr(() => { Assert.Fail(); return "-1"; }), null);

            Assert.AreEqual(someStruct.RightOr(ex => { Assert.Fail(); return -1; }), 1);
            Assert.AreEqual(someNullable.RightOr(ex => { Assert.Fail(); return -1; }), 1);
            Assert.AreEqual(someNullableEmpty.RightOr(ex => { Assert.Fail(); return -1; }), null);
            Assert.AreEqual(someClass.RightOr(ex => { Assert.Fail(); return "-1"; }), "1");
            Assert.AreEqual(someClassNull.RightOr(ex => { Assert.Fail(); return "-1"; }), null);
        }

        [TestMethod]
        public void Either_AlternativeValue()
        {
            var noneStruct = Either.Left<string, int>("ex");
            var noneNullable = Either.Left<string, int?>("ex");
            var noneClass = Either.Left<string, string>("ex");

            Assert.IsFalse(noneStruct.IsRight);
            Assert.IsFalse(noneNullable.IsRight);
            Assert.IsFalse(noneClass.IsRight);

            var someStruct = noneStruct.Or(1);
            var someNullable = noneNullable.Or(1);
            var someClass = noneClass.Or("1");

            Assert.IsTrue(someStruct.IsRight);
            Assert.IsTrue(someNullable.IsRight);
            Assert.IsTrue(someClass.IsRight);

            Assert.AreEqual(someStruct.RightOr(-1), 1);
            Assert.AreEqual(someNullable.RightOr(-1), 1);
            Assert.AreEqual(someClass.RightOr("-1"), "1");
        }

        [TestMethod]
        public void Either_AlternativeEither()
        {
            var noneStruct = Either.Left<string, int>("ex");
            var noneNullable = Either.Left<string, int?>("ex");
            var noneClass = Either.Left<string, string>("ex");

            Assert.IsFalse(noneStruct.IsRight);
            Assert.IsFalse(noneNullable.IsRight);
            Assert.IsFalse(noneClass.IsRight);

            var noneStruct2 = noneStruct.Else(Either.Left<string, int>("ex2"));
            var noneNullable2 = noneNullable.Else(Either.Left<string, int?>("ex2"));
            var noneClass2 = noneClass.Else(Either.Left<string, string>("ex2"));

            Assert.IsFalse(noneStruct2.IsRight);
            Assert.IsFalse(noneNullable2.IsRight);
            Assert.IsFalse(noneClass2.IsRight);

            var someStruct = noneStruct.Else(1.Right<string, int>());
            var someNullable = noneNullable.Else(Either.Right<string, int?>(1));
            var someClass = noneClass.Else("1".Right<string, string>());

            Assert.IsTrue(someStruct.IsRight);
            Assert.IsTrue(someNullable.IsRight);
            Assert.IsTrue(someClass.IsRight);

            Assert.AreEqual(someStruct.RightOr(-1), 1);
            Assert.AreEqual(someNullable.RightOr(-1), 1);
            Assert.AreEqual(someClass.RightOr("-1"), "1");
        }

        [TestMethod]
        public void Either_AlternativeValueLazy()
        {
            var noneStruct = Either.Left<string, int>("ex");
            var noneNullable = Either.Left<string, int?>("ex");
            var noneClass = Either.Left<string, string>("ex");

            Assert.IsFalse(noneStruct.IsRight);
            Assert.IsFalse(noneNullable.IsRight);
            Assert.IsFalse(noneClass.IsRight);

            var someStruct = noneStruct.Or(() => 1);
            var someNullable = noneNullable.Or(() => 1);
            var someClass = noneClass.Or(() => "1");

            Assert.IsTrue(someStruct.IsRight);
            Assert.IsTrue(someNullable.IsRight);
            Assert.IsTrue(someClass.IsRight);

            Assert.AreEqual(someStruct.RightOr(() => -1), 1);
            Assert.AreEqual(someNullable.RightOr(() => -1), 1);
            Assert.AreEqual(someClass.RightOr(() => "-1"), "1");

            someStruct.Or(() => { Assert.Fail(); return -1; });
            someNullable.Or(() => { Assert.Fail(); return -1; });
            someClass.Or(() => { Assert.Fail(); return "-1"; });

            var someStructEx = noneStruct.Or(ex => ex.GetHashCode());
            var someNullableEx = noneNullable.Or(ex => ex.GetHashCode());
            var someClassEx = noneClass.Or(ex => ex);

            Assert.IsTrue(someStructEx.IsRight);
            Assert.IsTrue(someNullableEx.IsRight);
            Assert.IsTrue(someClassEx.IsRight);

            Assert.AreEqual(someStructEx.RightOr(() => -1), "ex".GetHashCode());
            Assert.AreEqual(someNullableEx.RightOr(() => -1), "ex".GetHashCode());
            Assert.AreEqual(someClassEx.RightOr(() => "-1"), "ex");

            someStructEx.Or(() => { Assert.Fail(); return -1; });
            someNullableEx.Or(() => { Assert.Fail(); return -1; });
            someClassEx.Or(() => { Assert.Fail(); return "-1"; });
        }

        [TestMethod]
        public void Either_AlternativeEitherLazy()
        {
            var noneStruct = Either.Left<string, int>("ex");
            var noneNullable = Either.Left<string, int?>("ex");
            var noneClass = Either.Left<string, string>("ex");

            Assert.IsFalse(noneStruct.IsRight);
            Assert.IsFalse(noneNullable.IsRight);
            Assert.IsFalse(noneClass.IsRight);

            var noneStruct2 = noneStruct.Else(() => Either.Left<string, int>("ex2"));
            var noneNullable2 = noneNullable.Else(() => Either.Left<string, int?>("ex2"));
            var noneClass2 = noneClass.Else(() => Either.Left<string, string>("ex2"));

            Assert.IsFalse(noneStruct2.IsRight);
            Assert.IsFalse(noneNullable2.IsRight);
            Assert.IsFalse(noneClass2.IsRight);

            var noneStruct3 = noneStruct.Else(ex => Either.Left<string, int>(ex + "3"));
            var noneNullable3 = noneNullable.Else(ex => Either.Left<string, int?>(ex + "3"));
            var noneClass3 = noneClass.Else(ex => Either.Left<string, string>(ex + "3"));

            Assert.IsFalse(noneStruct3.IsRight);
            Assert.IsFalse(noneNullable3.IsRight);
            Assert.IsFalse(noneClass3.IsRight);

            var someStruct = noneStruct.Else(() => 1.Right<string, int>());
            var someNullable = noneNullable.Else(() => Either.Right<string, int?>(1));
            var someClass = noneClass.Else(() => "1".Right<string, string>());

            Assert.IsTrue(someStruct.IsRight);
            Assert.IsTrue(someNullable.IsRight);
            Assert.IsTrue(someClass.IsRight);

            Assert.AreEqual(someStruct.RightOr(() => -1), 1);
            Assert.AreEqual(someNullable.RightOr(() => -1), 1);
            Assert.AreEqual(someClass.RightOr(() => "-1"), "1");

            someStruct.Else(() => { Assert.Fail(); return (-1).Right<string, int>(); });
            someNullable.Else(() => { Assert.Fail(); return Either.Right<string, int?>(-1); });
            someClass.Else(() => { Assert.Fail(); return "-1".Right<string, string>(); });

            var someStructEx = noneStruct.Else(ex => ex.GetHashCode().Right<string, int>());
            var someNullableEx = noneNullable.Else(ex => Either.Right<string, int?>(ex.GetHashCode()));
            var someClassEx = noneClass.Else(ex => ex.Right<string, string>());

            Assert.IsTrue(someStructEx.IsRight);
            Assert.IsTrue(someNullableEx.IsRight);
            Assert.IsTrue(someClassEx.IsRight);

            Assert.AreEqual(someStructEx.RightOr(() => -1), "ex".GetHashCode());
            Assert.AreEqual(someNullableEx.RightOr(() => -1), "ex".GetHashCode());
            Assert.AreEqual(someClassEx.RightOr(() => "-1"), "ex");

            someStructEx.Else(() => { Assert.Fail(); return (-1).Right<string, int>(); });
            someNullableEx.Else(() => { Assert.Fail(); return Either.Right<string, int?>(-1); });
            someClassEx.Else(() => { Assert.Fail(); return "-1".Right<string, string>(); });
        }

        [TestMethod]
        public void Either_CreateExtensions()
        {
            var none = 1.Left("ex");
            var some = 1.Right<string, int>();

            Assert.AreEqual(none.RightOr(-1), -1);
            Assert.AreEqual(some.RightOr(-1), 1);

            var noneLargerThanTen = 1.RightWhen(x => x > 10, "ex");
            var someLargerThanTen = 20.RightWhen(x => x > 10, "ex");

            Assert.AreEqual(noneLargerThanTen.RightOr(-1), -1);
            Assert.AreEqual(someLargerThanTen.RightOr(-1), 20);

            var noneNotNull = ((string)null).RightNotNull("ex");
            var someNotNull = "1".RightNotNull("ex");

            Assert.AreEqual(noneNotNull.RightOr("-1"), "-1");
            Assert.AreEqual(someNotNull.RightOr("-1"), "1");

            var noneNullableNotNull = ((int?)null).RightNotNull("ex");
            var someNullableNotNull = ((int?)1).RightNotNull("ex");

            Assert.IsInstanceOfType(noneNullableNotNull.RightOr(-1), typeof(int?));
            Assert.IsInstanceOfType(someNullableNotNull.RightOr(-1), typeof(int?));
            Assert.AreEqual(noneNullableNotNull.RightOr(-1), -1);
            Assert.AreEqual(someNullableNotNull.RightOr(-1), 1);

            var noneFromNullable = ((int?)null).ToEither("ex");
            var someFromNullable = ((int?)1).ToEither("ex");

            Assert.IsInstanceOfType(noneFromNullable.RightOr(-1), typeof(int));
            Assert.IsInstanceOfType(someFromNullable.RightOr(-1), typeof(int));
            Assert.AreEqual(noneFromNullable.RightOr(-1), -1);
            Assert.AreEqual(someFromNullable.RightOr(-1), 1);
        }

        [TestMethod]
        public void Either_CreateExtensionsLazy()
        {
            var noneIsTen = "1".RightWhen(x => x == "10", () => "ex");
            var someIsTen = "10".RightWhen(x => x == "10", () => "ex");

            Assert.AreEqual(noneIsTen.RightOrException(), "ex");
            Assert.AreEqual(someIsTen.RightOrException(), "10");

            var noneNotNull = ((string)null).RightNotNull(() => "ex");
            var someNotNull = "1".RightNotNull(() => "ex");

            Assert.AreEqual(noneNotNull.RightOrException(), "ex");
            Assert.AreEqual(someNotNull.RightOrException(), "1");

            var noneFromNullable = ((int?)null).ToEither(() => -1);
            var someFromNullable = ((int?)1).ToEither(() => -1);

            Assert.AreEqual(noneFromNullable.RightOrException(), -1);
            Assert.AreEqual(someFromNullable.RightOrException(), 1);

            var some1 = "1".RightWhen(_ => true, () => { Assert.Fail(); return "ex"; });
            var some2 = "1".RightNotNull(() => { Assert.Fail(); return "ex"; });
            var some3 = ((int?)1).ToEither(() => { Assert.Fail(); return -1; });

            Assert.AreEqual(some1.RightOr("-1"), "1");
            Assert.AreEqual(some2.RightOr("-1"), "1");
            Assert.AreEqual(some3.RightOr(-1), 1);
        }

        [TestMethod]
        public void Either_Matching()
        {
            var left = "val".Left("ex");
            var right = "val".Right<string, string>();

            var failure = left.Match(
                right: val => val,
                left: ex => ex
            );

            var success = right.Match(
                right: val => val,
                left: ex => ex
            );

            Assert.AreEqual(failure, "ex");
            Assert.AreEqual(success, "val");

            var hasMatched = false;
            left.Match(
                right: val => Assert.Fail(),
                left: ex => hasMatched = ex == "ex"
            );
            Assert.IsTrue(hasMatched);

            hasMatched = false;
            right.Match(
                right: val => hasMatched = val == "val",
                left: ex => Assert.Fail()
            );
            Assert.IsTrue(hasMatched);

            left.MatchSome(val => Assert.Fail());
            hasMatched = false;
            right.MatchSome(val => hasMatched = val == "val");
            Assert.IsTrue(hasMatched);

            right.MatchNone(ex => Assert.Fail());
            hasMatched = false;
            left.MatchNone(ex => hasMatched = ex == "ex");
            Assert.IsTrue(hasMatched);
        }

        [TestMethod]
        public void Either_Transformation()
        {
            var none = "val".Left("ex");
            var some = "val".Right<string, string>();

            var noneNull = ((string)null).Left("ex");
            var someNull = ((string)null).Right<string, string>();

            var noneUpper = none.Map(x => x.ToUpper());
            var someUpper = some.Map(x => x.ToUpper());

            var noneExUpper = none.MapLeft(x => x.ToUpper());
            var someExUpper = some.MapLeft(x => x.ToUpper());

            Assert.IsFalse(noneUpper.IsRight);
            Assert.IsTrue(someUpper.IsRight);
            Assert.AreEqual(noneUpper.RightOr("ex"), "ex");
            Assert.AreEqual(someUpper.RightOr("ex"), "VAL");
            Assert.AreEqual(noneExUpper.Match(val => val, ex => ex), "EX");
            Assert.AreEqual(someExUpper.Match(val => val, ex => ex), "val");

            var noneNotNull = none.FlatMap(x => x.RightNotNull("ex1"));
            var someNotNull = some.FlatMap(x => x.RightNotNull("ex1"));
            var noneNullNotNull = noneNull.FlatMap(x => x.RightNotNull("ex1"));
            var someNullNotNull = someNull.FlatMap(x => x.RightNotNull("ex1"));

            Assert.IsFalse(noneNotNull.IsRight);
            Assert.IsTrue(someNotNull.IsRight);
            Assert.IsFalse(noneNullNotNull.IsRight);
            Assert.IsFalse(someNullNotNull.IsRight);
            Assert.AreEqual(noneNotNull.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someNotNull.Match(val => val, ex => ex), "val");
            Assert.AreEqual(noneNullNotNull.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someNullNotNull.Match(val => val, ex => ex), "ex1");
        }

        [TestMethod]
        public void Either_Filtering()
        {
            var none = "val".Left("ex");
            var some = "val".Right<string, string>();

            var noneNotVal = none.Filter(x => x != "val", "ex1");
            var someNotVal = some.Filter(x => x != "val", "ex1");
            var noneVal = none.Filter(x => x == "val", "ex1");
            var someVal = some.Filter(x => x == "val", "ex1");

            Assert.IsFalse(noneNotVal.IsRight);
            Assert.IsFalse(someNotVal.IsRight);
            Assert.IsFalse(noneVal.IsRight);
            Assert.IsTrue(someVal.IsRight);
            Assert.AreEqual(noneNotVal.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someNotVal.Match(val => val, ex => ex), "ex1");
            Assert.AreEqual(noneVal.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someVal.Match(val => val, ex => ex), "val");

            var noneFalse = none.Filter(false, "ex1");
            var someFalse = some.Filter(false, "ex1");
            var noneTrue = none.Filter(true, "ex1");
            var someTrue = some.Filter(true, "ex1");

            Assert.IsFalse(noneFalse.IsRight);
            Assert.IsFalse(someFalse.IsRight);
            Assert.IsFalse(noneTrue.IsRight);
            Assert.IsTrue(someTrue.IsRight);
            Assert.AreEqual(noneFalse.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someFalse.Match(val => val, ex => ex), "ex1");
            Assert.AreEqual(noneTrue.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someTrue.Match(val => val, ex => ex), "val");

            var someNull = Either.Right<string, string>(null);
            Assert.IsTrue(someNull.IsRight);
            Assert.IsFalse(someNull.NotNull("ex").IsRight);
            Assert.AreEqual(someNull.NotNull("ex").RightOrException(), "ex");

            var someNullableNull = Either.Right<int?, int?>(null);
            Assert.IsTrue(someNullableNull.IsRight);
            Assert.IsFalse(someNullableNull.NotNull(-1).IsRight);
            Assert.AreEqual(someNullableNull.NotNull(-1).RightOrException(), -1);

            var someStructNull = Either.Right<int, int>(default(int));
            Assert.IsTrue(someStructNull.IsRight);
            Assert.IsTrue(someStructNull.NotNull(-1).IsRight);
            Assert.AreEqual(someStructNull.NotNull(-1).RightOrException(), default(int));

            Assert.IsTrue(some.IsRight);
            Assert.IsTrue(some.NotNull("ex").IsRight);
            Assert.AreEqual(some.NotNull("ex").RightOrException(), "val");

            Assert.IsFalse(none.IsRight);
            Assert.IsFalse(none.NotNull("ex1").IsRight);
            Assert.AreEqual(none.NotNull("ex1").RightOrException(), "ex");
        }

        [TestMethod]
        public void Either_Filtering_Lazy()
        {
            var none = "val".Left("ex");
            var some = "val".Right<string, string>();

            var noneNotVal = none.Filter(x => x != "val", () => "ex1");
            var someNotVal = some.Filter(x => x != "val", () => "ex1");
            var noneVal = none.Filter(x => x == "val", () => "ex1");
            var someVal = some.Filter(x => x == "val", () => { Assert.Fail(); return "ex1"; });

            Assert.IsFalse(noneNotVal.IsRight);
            Assert.IsFalse(someNotVal.IsRight);
            Assert.IsFalse(noneVal.IsRight);
            Assert.IsTrue(someVal.IsRight);
            Assert.AreEqual(noneNotVal.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someNotVal.Match(val => val, ex => ex), "ex1");
            Assert.AreEqual(noneVal.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someVal.Match(val => val, ex => ex), "val");

            var noneFalse = none.Filter(false, () => "ex1");
            var someFalse = some.Filter(false, () => "ex1");
            var noneTrue = none.Filter(true, () => "ex1");
            var someTrue = some.Filter(true, () => { Assert.Fail(); return "ex1"; });

            Assert.IsFalse(noneFalse.IsRight);
            Assert.IsFalse(someFalse.IsRight);
            Assert.IsFalse(noneTrue.IsRight);
            Assert.IsTrue(someTrue.IsRight);
            Assert.AreEqual(noneFalse.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someFalse.Match(val => val, ex => ex), "ex1");
            Assert.AreEqual(noneTrue.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someTrue.Match(val => val, ex => ex), "val");

            var someNull = Either.Right<string, string>(null);
            Assert.IsTrue(someNull.IsRight);
            Assert.IsFalse(someNull.NotNull(() => "ex").IsRight);
            Assert.AreEqual(someNull.NotNull(() => "ex").RightOrException(), "ex");

            var someNullableNull = Either.Right<int?, int?>(null);
            Assert.IsTrue(someNullableNull.IsRight);
            Assert.IsFalse(someNullableNull.NotNull(() => -1).IsRight);
            Assert.AreEqual(someNullableNull.NotNull(() => -1).RightOrException(), -1);

            var someStructNull = Either.Right<int, int>(default(int));
            Assert.IsTrue(someStructNull.IsRight);
            Assert.IsTrue(someStructNull.NotNull(() => -1).IsRight);
            Assert.AreEqual(someStructNull.NotNull(() => -1).RightOrException(), default(int));

            Assert.IsTrue(some.IsRight);
            Assert.IsTrue(some.NotNull(() => "ex").IsRight);
            Assert.AreEqual(some.NotNull(() => "ex").RightOrException(), "val");

            Assert.IsFalse(none.IsRight);
            Assert.IsFalse(none.NotNull(() => "ex1").IsRight);
            Assert.AreEqual(none.NotNull(() => "ex1").RightOrException(), "ex");

            var someNotNull = some.NotNull(() => { Assert.Fail(); return "ex1"; });
            Assert.IsTrue(someNotNull.IsRight);

            var noneNotNull = none.NotNull(() => { Assert.Fail(); return "ex1"; });
            Assert.IsFalse(noneNotNull.IsRight);
        }

        [TestMethod]
        public void Either_Filtering_ExceptionPropagation()
        {
            var none = "val".Left("ex");
            var some = "val".Right<string, string>();

            Assert.AreEqual(none.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(some.Match(val => val, ex => ex), "val");

            var none1a = none.Filter(val => false, "ex1");
            var some1a = some.Filter(val => false, "ex1");
            var none1b = none.Filter(false, "ex1");
            var some1b = some.Filter(false, "ex1");

            Assert.AreEqual(none1a.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(some1a.Match(val => val, ex => ex), "ex1");
            Assert.AreEqual(none1b.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(some1b.Match(val => val, ex => ex), "ex1");

            var none2a = none1a.Filter(val => false, "ex2");
            var some2a = some1a.Filter(val => false, "ex2");
            var none2b = none1b.Filter(false, "ex2");
            var some2b = some1b.Filter(false, "ex2");

            Assert.AreEqual(none2a.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(some2a.Match(val => val, ex => ex), "ex1");
            Assert.AreEqual(none2b.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(some2b.Match(val => val, ex => ex), "ex1");
        }

        [TestMethod]
        public void Either_ToEnumerable()
        {
            var none = "a".Left("ex");
            var some = "a".Right<string, string>();

            var noneAsEnumerable = none.ToEnumerable();
            var someAsEnumerable = some.ToEnumerable();

            foreach (var value in noneAsEnumerable)
            {
                Assert.Fail();
            }

            int count = 0;
            foreach (var value in someAsEnumerable)
            {
                Assert.AreEqual(value, "a");
                count += 1;
            }
            Assert.AreEqual(count, 1);

            foreach (var value in someAsEnumerable)
            {
                Assert.AreEqual(value, "a");
                count += 1;
            }
            Assert.AreEqual(count, 2);

            Assert.AreEqual(noneAsEnumerable.Count(), 0);
            Assert.AreEqual(someAsEnumerable.Count(), 1);
        }

        [TestMethod]
        public void Either_Enumerate()
        {
            var none = "a".Left("ex");
            var some = "a".Right<string, string>();

            foreach (var value in none)
            {
                Assert.Fail();
            }

            int count = 0;
            foreach (var value in some)
            {
                Assert.AreEqual(value, "a");
                count += 1;
            }
            Assert.AreEqual(count, 1);

            foreach (var value in some)
            {
                Assert.AreEqual(value, "a");
                count += 1;
            }
            Assert.AreEqual(count, 2);
        }

        [TestMethod]
        public void Maybe_Default()
        {
            var none1 = default(Either<int, int>);
            var none2 = default(Either<int?, int?>);
            var none3 = default(Either<string, string>);

            Assert.IsFalse(none1.IsRight);
            Assert.IsFalse(none2.IsRight);
            Assert.IsFalse(none3.IsRight);

            Assert.IsFalse(none1.IsLeft);
            Assert.IsFalse(none2.IsLeft);
            Assert.IsFalse(none3.IsLeft);

            Assert.IsTrue(none1.IsBottom);
            Assert.IsTrue(none2.IsBottom);
            Assert.IsTrue(none3.IsBottom);

            var some1 = none1.Or(1);
            var some2 = none2.Or(1);
            var some3 = none3.Or("1");

            Assert.AreEqual(some1.RightOr(-1), 1);
            Assert.AreEqual(some2.RightOr(-1), 1);
            Assert.AreEqual(some3.RightOr("-1"), "1");
        }

#if !NETSTANDARD10
        [TestMethod]
        public void Either_Serialization()
        {
            var some = Either.Right<string, string>("1");
            var none = Either.Left<string, string>("-1");

            var formatter = new BinaryFormatter();

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, some);
                stream.Position = 0;
                var someDeserialized = (Either<string, string>)formatter.Deserialize(stream);
                Assert.AreEqual(some, someDeserialized);
            }

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, none);
                stream.Position = 0;
                var noneDeserialized = (Either<string, string>)formatter.Deserialize(stream);
                Assert.AreEqual(none, noneDeserialized);
            }
        }
#endif
    }
}
