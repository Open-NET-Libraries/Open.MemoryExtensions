using Open.Memory;
using System;
using System.Buffers;
using Xunit;

namespace Open.MemoryExtensions.Tests
{
	public class TemporaryArrayTests
	{
		[Fact]
		public static void Copy()
		{
			using var array = TemporaryArray.Create<int>(1000);
			CopyTest(array);
		}

		[Fact]
		public static void ArrayOfArrays()
		{
			using var array = TemporaryArray.Create<TemporaryArray<int>>(1000);
			array[0] = TemporaryArray.Create<int>(1000);
			CopyTest2(array);
		}

		static void CopyTest<T>(TemporaryArray<T> array)
		{
			Assert.NotNull(array.Array);
			var a = array;
			Assert.NotNull(a.Array);
		}

		static void CopyTest2<T>(TemporaryArray<TemporaryArray<T>> array)
		{
			CopyTest(array);
			var a = array[0];
			Assert.NotNull(a.Array);
		}
	}
}
