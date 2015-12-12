﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optional.Tests
{
    [TestClass]
    public class EitherTests
    {
        [TestMethod]
        public void Either_CreateAndCheckExistence()
        {
            var noneStruct = Option.None<int, string>("ex");
            var noneNullable = Option.None<int?, string>("ex");
            var noneClass = Option.None<string, string>("ex");

            Assert.IsFalse(noneStruct.HasValue);
            Assert.IsFalse(noneNullable.HasValue);
            Assert.IsFalse(noneClass.HasValue);

            var someStruct = Option.Some<int, string>(1);
            var someNullable = Option.Some<int?, string>(1);
            var someNullableEmpty = Option.Some<int?, string>(null);
            var someClass = Option.Some<string, string>("1");
            var someClassNull = Option.Some<string, string>(null);

            Assert.IsTrue(someStruct.HasValue);
            Assert.IsTrue(someNullable.HasValue);
            Assert.IsTrue(someNullableEmpty.HasValue);
            Assert.IsTrue(someClass.HasValue);
            Assert.IsTrue(someClassNull.HasValue);
        }

        [TestMethod]
        public void Either_CheckContainment()
        {
            var noneStruct = Option.None<int, string>("ex");
            var noneNullable = Option.None<int?, string>("ex");
            var noneClass = Option.None<string, string>("ex");

            Assert.IsFalse(noneStruct.Contains(0));
            Assert.IsFalse(noneNullable.Contains(null));
            Assert.IsFalse(noneClass.Contains(null));

            Assert.IsFalse(noneStruct.Exists(val => true));
            Assert.IsFalse(noneNullable.Exists(val => true));
            Assert.IsFalse(noneClass.Exists(val => true));

            var someStruct = Option.Some<int, string>(1);
            var someNullable = Option.Some<int?, string>(1);
            var someNullableEmpty = Option.Some<int?, string>(null);
            var someClass = Option.Some<string, string>("1");
            var someClassNull = Option.Some<string, string>(null);

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
            Assert.AreEqual(Option.None<string, string>("ex"), Option.None<string, string>("ex"));
            Assert.AreEqual(Option.None<string, string>(null), Option.None<string, string>(null));
            Assert.AreNotEqual(Option.None<string, string>("ex"), Option.None<string, string>(null));
            Assert.AreNotEqual(Option.None<string, string>(null), Option.None<string, string>("ex"));
            Assert.AreNotEqual(Option.None<string, string>("ex"), Option.None<string, string>("ex1"));

            Assert.AreEqual(Option.Some<string, string>("val"), Option.Some<string, string>("val"));
            Assert.AreEqual(Option.Some<string, string>(null), Option.Some<string, string>(null));
            Assert.AreNotEqual(Option.Some<string, string>("val"), Option.Some<string, string>(null));
            Assert.AreNotEqual(Option.Some<string, string>(null), Option.Some<string, string>("val"));
            Assert.AreNotEqual(Option.Some<string, string>("val"), Option.Some<string, string>("val1"));

            // Must have same types
            Assert.AreNotEqual(Option.None<string, string>("ex"), Option.None<string, object>("ex"));
            Assert.AreNotEqual(Option.None<string, string>("ex"), Option.None<object, string>("ex"));
            Assert.AreNotEqual(Option.Some<string, string>("val"), Option.Some<string, object>("val"));
            Assert.AreNotEqual(Option.Some<string, string>("val"), Option.Some<object, string>("val"));

            // Some and None are different
            Assert.AreNotEqual(Option.Some<string, string>("ex"), Option.None<string, string>("ex"));
            Assert.AreNotEqual(Option.Some<string, string>(null), Option.None<string, string>(null));

            // Works for val. types, nullables and ref. types
            Assert.AreEqual(Option.None<int, int>(1), Option.None<int, int>(1));
            Assert.AreEqual(Option.None<int?, int?>(1), Option.None<int?, int?>(1));
            Assert.AreEqual(Option.None<string, string>("1"), Option.None<string, string>("1"));
            Assert.AreNotEqual(Option.None<int, int>(1), Option.None<int, int>(-1));
            Assert.AreNotEqual(Option.None<int?, int?>(1), Option.None<int?, int?>(-1));
            Assert.AreNotEqual(Option.None<string, string>("1"), Option.None<string, string>("-1"));

            Assert.AreEqual(Option.Some<int, int>(1), Option.Some<int, int>(1));
            Assert.AreEqual(Option.Some<int?, int?>(1), Option.Some<int?, int?>(1));
            Assert.AreEqual(Option.Some<string, string>("1"), Option.Some<string, string>("1"));
            Assert.AreNotEqual(Option.Some<int, int>(1), Option.Some<int, int>(-1));
            Assert.AreNotEqual(Option.Some<int?, int?>(1), Option.Some<int?, int?>(-1));
            Assert.AreNotEqual(Option.Some<string, string>("1"), Option.Some<string, string>("-1"));

            // Works when when boxed
            Assert.AreEqual((object)Option.None<int, int>(1), (object)Option.None<int, int>(1));
            Assert.AreEqual((object)Option.Some<int, int>(22), (object)Option.Some<int, int>(22));
            Assert.AreNotEqual((object)Option.Some<int, int>(21), (object)Option.Some<int, int>(22));
            Assert.AreNotEqual((object)Option.None<int, int>(21), (object)Option.None<int, int>(22));
            Assert.AreNotEqual((object)Option.None<int, int>(1), (object)Option.Some<int, int>(22));

            // Works with default equalitycomparer 
            Assert.IsTrue(EqualityComparer<Option<int, int>>.Default.Equals(Option.None<int, int>(1), Option.None<int, int>(1)));
            Assert.IsTrue(EqualityComparer<Option<int, int>>.Default.Equals(Option.Some<int, int>(22), Option.Some<int, int>(22)));
            Assert.IsFalse(EqualityComparer<Option<int, int>>.Default.Equals(Option.Some<int, int>(22), Option.Some<int, int>(21)));
            Assert.IsFalse(EqualityComparer<Option<int, int>>.Default.Equals(Option.None<int, int>(22), Option.None<int, int>(21)));
            Assert.IsFalse(EqualityComparer<Option<int, int>>.Default.Equals(Option.Some<int, int>(22), Option.None<int, int>(1)));

            // Works with equality operators
            Assert.IsTrue(Option.None<int, int>(1) == Option.None<int, int>(1));
            Assert.IsTrue(Option.Some<int, int>(22) == Option.Some<int, int>(22));
            Assert.IsTrue(Option.None<int, int>(2) != Option.None<int, int>(1));
            Assert.IsTrue(Option.Some<int, int>(22) != Option.None<int, int>(1));
            Assert.IsTrue(Option.Some<int, int>(22) != Option.Some<int, int>(21));
        }

        [TestMethod]
        public void Either_Hashing()
        {
            Assert.AreEqual(Option.None<string, string>("ex").GetHashCode(), Option.None<string, string>("ex").GetHashCode());
            Assert.AreEqual(Option.None<object, string>("ex").GetHashCode(), Option.None<object, string>("ex").GetHashCode());

            Assert.AreEqual(Option.Some<string, string>("val").GetHashCode(), Option.Some<string, string>("val").GetHashCode());
            Assert.AreEqual(Option.Some<object, string>("val").GetHashCode(), Option.Some<object, string>("val").GetHashCode());

            Assert.AreEqual(Option.None<string, string>(null).GetHashCode(), Option.None<string, string>(null).GetHashCode());
            Assert.AreEqual(Option.None<object, string>(null).GetHashCode(), Option.None<object, string>(null).GetHashCode());

            Assert.AreEqual(Option.Some<string, string>(null).GetHashCode(), Option.Some<string, string>(null).GetHashCode());
            Assert.AreEqual(Option.Some<object, string>(null).GetHashCode(), Option.Some<object, string>(null).GetHashCode());

            Assert.AreNotEqual(Option.Some<string, string>(null).GetHashCode(), Option.None<string, string>(null).GetHashCode());
            Assert.AreNotEqual(Option.Some<object, string>(null).GetHashCode(), Option.None<object, string>(null).GetHashCode());
        }

        [TestMethod]
        public void Either_StringRepresentation()
        {
            Assert.AreEqual(Option.None<int?, int?>(null).ToString(), "None(null)");
            Assert.AreEqual(Option.None<string, string>(null).ToString(), "None(null)");

            Assert.AreEqual(Option.None<int, int>(1).ToString(), "None(1)");
            Assert.AreEqual(Option.None<int?, int?>(1).ToString(), "None(1)");
            Assert.AreEqual(Option.None<string, string>("ex").ToString(), "None(ex)");

            Assert.AreEqual(Option.Some<int?, string>(null).ToString(), "Some(null)");
            Assert.AreEqual(Option.Some<string, string>(null).ToString(), "Some(null)");

            Assert.AreEqual(Option.Some<int, string>(1).ToString(), "Some(1)");
            Assert.AreEqual(Option.Some<int?, string>(1).ToString(), "Some(1)");
            Assert.AreEqual(Option.Some<string, string>("1").ToString(), "Some(1)");

            var now = DateTime.Now;
            Assert.AreEqual(Option.Some<DateTime, DateTime>(now).ToString(), "Some(" + now.ToString() + ")");
            Assert.AreEqual(Option.None<DateTime, DateTime>(now).ToString(), "None(" + now.ToString() + ")");
        }

        [TestMethod]
        public void Either_GetValue()
        {
            var noneStruct = Option.None<int, string>("ex");
            var noneNullable = Option.None<int?, string>("ex");
            var noneClass = Option.None<string, string>("ex");

            Assert.AreEqual(noneStruct.ValueOr(-1), -1);
            Assert.AreEqual(noneNullable.ValueOr(-1), -1);
            Assert.AreEqual(noneClass.ValueOr("-1"), "-1");

            var someStruct = Option.Some<int, string>(1);
            var someNullable = Option.Some<int?, string>(1);
            var someNullableEmpty = Option.Some<int?, string>(null);
            var someClass = Option.Some<string, string>("1");
            var someClassNull = Option.Some<string, string>(null);

            Assert.AreEqual(someStruct.ValueOr(-1), 1);
            Assert.AreEqual(someNullable.ValueOr(-1), 1);
            Assert.AreEqual(someNullableEmpty.ValueOr(-1), null);
            Assert.AreEqual(someClass.ValueOr("-1"), "1");
            Assert.AreEqual(someClassNull.ValueOr("-1"), null);

            // Value or exception
            Assert.AreEqual(noneClass.ValueOrException(), "ex");
            Assert.AreEqual(someClass.ValueOrException(), "1");
            Assert.AreEqual(someClassNull.ValueOrException(), null);
        }

        [TestMethod]
        public void Either_GetValueLazy()
        {
            var noneStruct = Option.None<int, string>("ex");
            var noneNullable = Option.None<int?, string>("ex");
            var noneClass = Option.None<string, string>("ex");

            Assert.AreEqual(noneStruct.ValueOr(() => -1), -1);
            Assert.AreEqual(noneNullable.ValueOr(() => -1), -1);
            Assert.AreEqual(noneClass.ValueOr(() => "-1"), "-1");

            Assert.AreEqual(noneStruct.ValueOr(ex => ex.GetHashCode()), "ex".GetHashCode());
            Assert.AreEqual(noneNullable.ValueOr(ex => ex.GetHashCode()), "ex".GetHashCode());
            Assert.AreEqual(noneClass.ValueOr(ex => ex), "ex");

            var someStruct = Option.Some<int, string>(1);
            var someNullable = Option.Some<int?, string>(1);
            var someNullableEmpty = Option.Some<int?, string>(null);
            var someClass = Option.Some<string, string>("1");
            var someClassNull = Option.Some<string, string>(null);

            Assert.AreEqual(someStruct.ValueOr(() => -1), 1);
            Assert.AreEqual(someNullable.ValueOr(() => -1), 1);
            Assert.AreEqual(someNullableEmpty.ValueOr(() => -1), null);
            Assert.AreEqual(someClass.ValueOr(() => "-1"), "1");
            Assert.AreEqual(someClassNull.ValueOr(() => "-1"), null);

            Assert.AreEqual(someStruct.ValueOr(ex => ex.GetHashCode()), 1);
            Assert.AreEqual(someNullable.ValueOr(ex => ex.GetHashCode()), 1);
            Assert.AreEqual(someNullableEmpty.ValueOr(ex => ex.GetHashCode()), null);
            Assert.AreEqual(someClass.ValueOr(ex => ex), "1");
            Assert.AreEqual(someClassNull.ValueOr(ex => ex), null);

            Assert.AreEqual(someStruct.ValueOr(() => { Assert.Fail(); return -1; }), 1);
            Assert.AreEqual(someNullable.ValueOr(() => { Assert.Fail(); return -1; }), 1);
            Assert.AreEqual(someNullableEmpty.ValueOr(() => { Assert.Fail(); return -1; }), null);
            Assert.AreEqual(someClass.ValueOr(() => { Assert.Fail(); return "-1"; }), "1");
            Assert.AreEqual(someClassNull.ValueOr(() => { Assert.Fail(); return "-1"; }), null);

            Assert.AreEqual(someStruct.ValueOr(ex => { Assert.Fail(); return -1; }), 1);
            Assert.AreEqual(someNullable.ValueOr(ex => { Assert.Fail(); return -1; }), 1);
            Assert.AreEqual(someNullableEmpty.ValueOr(ex => { Assert.Fail(); return -1; }), null);
            Assert.AreEqual(someClass.ValueOr(ex => { Assert.Fail(); return "-1"; }), "1");
            Assert.AreEqual(someClassNull.ValueOr(ex => { Assert.Fail(); return "-1"; }), null);
        }

        [TestMethod]
        public void Either_AlternativeValue()
        {
            var noneStruct = Option.None<int, string>("ex");
            var noneNullable = Option.None<int?, string>("ex");
            var noneClass = Option.None<string, string>("ex");

            Assert.IsFalse(noneStruct.HasValue);
            Assert.IsFalse(noneNullable.HasValue);
            Assert.IsFalse(noneClass.HasValue);

            var someStruct = noneStruct.Or(1);
            var someNullable = noneNullable.Or(1);
            var someClass = noneClass.Or("1");

            Assert.IsTrue(someStruct.HasValue);
            Assert.IsTrue(someNullable.HasValue);
            Assert.IsTrue(someClass.HasValue);

            Assert.AreEqual(someStruct.ValueOr(-1), 1);
            Assert.AreEqual(someNullable.ValueOr(-1), 1);
            Assert.AreEqual(someClass.ValueOr("-1"), "1");
        }

        [TestMethod]
        public void Either_AlternativeValueLazy()
        {
            var noneStruct = Option.None<int, string>("ex");
            var noneNullable = Option.None<int?, string>("ex");
            var noneClass = Option.None<string, string>("ex");

            Assert.IsFalse(noneStruct.HasValue);
            Assert.IsFalse(noneNullable.HasValue);
            Assert.IsFalse(noneClass.HasValue);

            var someStruct = noneStruct.Or(() => 1);
            var someNullable = noneNullable.Or(() => 1);
            var someClass = noneClass.Or(() => "1");

            Assert.IsTrue(someStruct.HasValue);
            Assert.IsTrue(someNullable.HasValue);
            Assert.IsTrue(someClass.HasValue);

            Assert.AreEqual(someStruct.ValueOr(() => -1), 1);
            Assert.AreEqual(someNullable.ValueOr(() => -1), 1);
            Assert.AreEqual(someClass.ValueOr(() => "-1"), "1");

            someStruct.Or(() => { Assert.Fail(); return -1; });
            someNullable.Or(() => { Assert.Fail(); return -1; });
            someClass.Or(() => { Assert.Fail(); return "-1"; });

            var someStructEx = noneStruct.Or(ex => ex.GetHashCode());
            var someNullableEx = noneNullable.Or(ex => ex.GetHashCode());
            var someClassEx = noneClass.Or(ex => ex);

            Assert.IsTrue(someStructEx.HasValue);
            Assert.IsTrue(someNullableEx.HasValue);
            Assert.IsTrue(someClassEx.HasValue);

            Assert.AreEqual(someStructEx.ValueOr(() => -1), "ex".GetHashCode());
            Assert.AreEqual(someNullableEx.ValueOr(() => -1), "ex".GetHashCode());
            Assert.AreEqual(someClassEx.ValueOr(() => "-1"), "ex");

            someStructEx.Or(() => { Assert.Fail(); return -1; });
            someNullableEx.Or(() => { Assert.Fail(); return -1; });
            someClassEx.Or(() => { Assert.Fail(); return "-1"; });
        }

        [TestMethod]
        public void Either_CreateExtensions()
        {
            var none = 1.None("ex");
            var some = 1.Some<int, string>();

            Assert.AreEqual(none.ValueOr(-1), -1);
            Assert.AreEqual(some.ValueOr(-1), 1);

            var noneLargerThanTen = 1.SomeWhen<int, string>(x => x > 10, "ex");
            var someLargerThanTen = 20.SomeWhen<int, string>(x => x > 10, "ex");

            Assert.AreEqual(noneLargerThanTen.ValueOr(-1), -1);
            Assert.AreEqual(someLargerThanTen.ValueOr(-1), 20);

            var noneNotNull = ((string)null).SomeNotNull<string, string>("ex");
            var someNotNull = "1".SomeNotNull<string, string>("ex");

            Assert.AreEqual(noneNotNull.ValueOr("-1"), "-1");
            Assert.AreEqual(someNotNull.ValueOr("-1"), "1");

            var noneNullableNotNull = ((int?)null).SomeNotNull<int?, string>("ex");
            var someNullableNotNull = ((int?)1).SomeNotNull<int?, string>("ex");

            Assert.IsInstanceOfType(noneNullableNotNull.ValueOr(-1), typeof(int?));
            Assert.IsInstanceOfType(someNullableNotNull.ValueOr(-1), typeof(int?));
            Assert.AreEqual(noneNullableNotNull.ValueOr(-1), -1);
            Assert.AreEqual(someNullableNotNull.ValueOr(-1), 1);

            var noneFromNullable = ((int?)null).ToOption<int, string>("ex");
            var someFromNullable = ((int?)1).ToOption<int, string>("ex");

            Assert.IsInstanceOfType(noneFromNullable.ValueOr(-1), typeof(int));
            Assert.IsInstanceOfType(someFromNullable.ValueOr(-1), typeof(int));
            Assert.AreEqual(noneFromNullable.ValueOr(-1), -1);
            Assert.AreEqual(someFromNullable.ValueOr(-1), 1);
        }

        [TestMethod]
        public void Either_CreateExtensionsLazy()
        {
            var noneIsTen = "1".SomeWhen<string, string>(x => x == "10", () => "ex");
            var someIsTen = "10".SomeWhen<string, string>(x => x == "10", () => "ex");

            Assert.AreEqual(noneIsTen.ValueOrException(), "ex");
            Assert.AreEqual(someIsTen.ValueOrException(), "10");

            var noneNotNull = ((string)null).SomeNotNull<string, string>(() => "ex");
            var someNotNull = "1".SomeNotNull<string, string>(() => "ex");

            Assert.AreEqual(noneNotNull.ValueOrException(), "ex");
            Assert.AreEqual(someNotNull.ValueOrException(), "1");

            var noneFromNullable = ((int?)null).ToOption<int, int>(() => -1);
            var someFromNullable = ((int?)1).ToOption<int, int>(() => -1);

            Assert.AreEqual(noneFromNullable.ValueOrException(), -1);
            Assert.AreEqual(someFromNullable.ValueOrException(), 1);

            var some1 = "1".SomeWhen<string, string>(_ => true, () => { Assert.Fail(); return "ex"; });
            var some2 = "1".SomeNotNull<string, string>(() => { Assert.Fail(); return "ex"; });
            var some3 = ((int?)1).ToOption<int, int>(() => { Assert.Fail(); return -1; });

            Assert.AreEqual(some1.ValueOr("-1"), "1");
            Assert.AreEqual(some2.ValueOr("-1"), "1");
            Assert.AreEqual(some3.ValueOr(-1), 1);
        }

        [TestMethod]
        public void Either_Matching()
        {
            var none = "val".None("ex");
            var some = "val".Some<string, string>();

            var failure = none.Match(
                some: val => val,
                none: ex => ex
            );

            var success = some.Match(
                some: val => val,
                none: ex => ex
            );

            Assert.AreEqual(failure, "ex");
            Assert.AreEqual(success, "val");

            none.Match(
                some: val => Assert.Fail(),
                none: ex => { }
            );

            some.Match(
                some: val => { },
                none: ex => Assert.Fail()
            );
        }

        [TestMethod]
        public void Either_Transformation()
        {
            var none = "val".None("ex");
            var some = "val".Some<string, string>();

            var noneNull = ((string)null).None("ex");
            var someNull = ((string)null).Some<string, string>();

            var noneUpper = none.Map(x => x.ToUpper());
            var someUpper = some.Map(x => x.ToUpper());

            var noneExUpper = none.MapException(x => x.ToUpper());
            var someExUpper = some.MapException(x => x.ToUpper());

            Assert.IsFalse(noneUpper.HasValue);
            Assert.IsTrue(someUpper.HasValue);
            Assert.AreEqual(noneUpper.ValueOr("ex"), "ex");
            Assert.AreEqual(someUpper.ValueOr("ex"), "VAL");
            Assert.AreEqual(noneExUpper.Match(val => val, ex => ex), "EX");
            Assert.AreEqual(someExUpper.Match(val => val, ex => ex), "val");

            var noneNotNull = none.FlatMap(x => x.SomeNotNull<string, string>("ex1"));
            var someNotNull = some.FlatMap(x => x.SomeNotNull<string, string>("ex1"));
            var noneNullNotNull = noneNull.FlatMap(x => x.SomeNotNull<string, string>("ex1"));
            var someNullNotNull = someNull.FlatMap(x => x.SomeNotNull<string, string>("ex1"));

            Assert.IsFalse(noneNotNull.HasValue);
            Assert.IsTrue(someNotNull.HasValue);
            Assert.IsFalse(noneNullNotNull.HasValue);
            Assert.IsFalse(someNullNotNull.HasValue);
            Assert.AreEqual(noneNotNull.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someNotNull.Match(val => val, ex => ex), "val");
            Assert.AreEqual(noneNullNotNull.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someNullNotNull.Match(val => val, ex => ex), "ex1");
        }

        [TestMethod]
        public void Either_Filtering()
        {
            var none = "val".None("ex");
            var some = "val".Some<string, string>();

            var noneNotVal = none.Filter(x => x != "val", "ex1");
            var someNotVal = some.Filter(x => x != "val", "ex1");
            var noneVal = none.Filter(x => x == "val", "ex1");
            var someVal = some.Filter(x => x == "val", "ex1");

            Assert.IsFalse(noneNotVal.HasValue);
            Assert.IsFalse(someNotVal.HasValue);
            Assert.IsFalse(noneVal.HasValue);
            Assert.IsTrue(someVal.HasValue);
            Assert.AreEqual(noneNotVal.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someNotVal.Match(val => val, ex => ex), "ex1");
            Assert.AreEqual(noneVal.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someVal.Match(val => val, ex => ex), "val");

            var noneFalse = none.Filter(false, "ex1");
            var someFalse = some.Filter(false, "ex1");
            var noneTrue = none.Filter(true, "ex1");
            var someTrue = some.Filter(true, "ex1");

            Assert.IsFalse(noneFalse.HasValue);
            Assert.IsFalse(someFalse.HasValue);
            Assert.IsFalse(noneTrue.HasValue);
            Assert.IsTrue(someTrue.HasValue);
            Assert.AreEqual(noneFalse.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someFalse.Match(val => val, ex => ex), "ex1");
            Assert.AreEqual(noneTrue.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someTrue.Match(val => val, ex => ex), "val");
        }

        [TestMethod]
        public void Either_Filtering_Lazy()
        {
            var none = "val".None("ex");
            var some = "val".Some<string, string>();

            var noneNotVal = none.Filter(x => x != "val", () => "ex1");
            var someNotVal = some.Filter(x => x != "val", () => "ex1");
            var noneVal = none.Filter(x => x == "val", () => "ex1");
            var someVal = some.Filter(x => x == "val", () => { Assert.Fail(); return "ex1"; });

            Assert.IsFalse(noneNotVal.HasValue);
            Assert.IsFalse(someNotVal.HasValue);
            Assert.IsFalse(noneVal.HasValue);
            Assert.IsTrue(someVal.HasValue);
            Assert.AreEqual(noneNotVal.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someNotVal.Match(val => val, ex => ex), "ex1");
            Assert.AreEqual(noneVal.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someVal.Match(val => val, ex => ex), "val");

            var noneFalse = none.Filter(false, () => "ex1");
            var someFalse = some.Filter(false, () => "ex1");
            var noneTrue = none.Filter(true, () => "ex1");
            var someTrue = some.Filter(true, () => { Assert.Fail(); return "ex1"; });

            Assert.IsFalse(noneFalse.HasValue);
            Assert.IsFalse(someFalse.HasValue);
            Assert.IsFalse(noneTrue.HasValue);
            Assert.IsTrue(someTrue.HasValue);
            Assert.AreEqual(noneFalse.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someFalse.Match(val => val, ex => ex), "ex1");
            Assert.AreEqual(noneTrue.Match(val => val, ex => ex), "ex");
            Assert.AreEqual(someTrue.Match(val => val, ex => ex), "val");
        }

        [TestMethod]
        public void Either_Filtering_ExceptionPropagation()
        {
            var none = "val".None("ex");
            var some = "val".Some<string, string>();

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
            var none = "a".None("ex");
            var some = "a".Some<string, string>();

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
            var none = "a".None("ex");
            var some = "a".Some<string, string>();

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
    }
}
