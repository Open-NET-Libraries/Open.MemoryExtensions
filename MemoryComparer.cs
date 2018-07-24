using System;
using System.Collections.Generic;

namespace Open.Memory
{
	public class MemoryComparer<T> : IComparer<ReadOnlyMemory<T>>
		where T : IComparable<T>
	{
		MemoryComparer(int sign = +1)
		{
			Sign = sign;
		}
		public readonly int Sign;
		public int Compare(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y)
			=> Sign * MemoryComparer.Compare(x, y);

		// ReSharper disable once RedundantArgumentDefaultValue
		public static readonly IComparer<ReadOnlyMemory<T>> Ascending = new MemoryComparer<T>(+1);
		public static readonly IComparer<ReadOnlyMemory<T>> Descending = new MemoryComparer<T>(-1);
	}

	class MemoryFloatComparer : IComparer<ReadOnlyMemory<float>>
	{
		internal MemoryFloatComparer(int sign = +1)
		{
			Sign = sign;
		}
		public readonly int Sign;
		public int Compare(ReadOnlyMemory<float> x, ReadOnlyMemory<float> y)
			=> Sign * MemoryComparer.Compare(x, y);
	}

	class MemoryDoubleComparer : IComparer<ReadOnlyMemory<double>>
	{
		internal MemoryDoubleComparer(int sign = +1)
		{
			Sign = sign;
		}
		public readonly int Sign;
		public int Compare(ReadOnlyMemory<double> x, ReadOnlyMemory<double> y)
			=> Sign * MemoryComparer.Compare(x, y);
	}

	public static class MemoryComparer
	{
		public static int Compare<T>(in ReadOnlyMemory<T> target, in ReadOnlyMemory<T> other)
			where T : IComparable<T>
			=> SpanComparer.Compare(target.Span, other.Span);

		public static class Float
		{
			public static readonly IComparer<ReadOnlyMemory<float>> Ascending = new MemoryFloatComparer();
			public static readonly IComparer<ReadOnlyMemory<float>> Descending = new MemoryFloatComparer(-1);
			public static int Compare(in ReadOnlyMemory<float> target, in ReadOnlyMemory<float> other)
				=> SpanComparer.Float.Compare(target.Span, other.Span);
		}

		public static class Double
		{
			public static readonly IComparer<ReadOnlyMemory<double>> Ascending = new MemoryDoubleComparer();
			public static readonly IComparer<ReadOnlyMemory<double>> Descending = new MemoryDoubleComparer(-1);
			public static int Compare(in ReadOnlyMemory<double> target, in ReadOnlyMemory<double> other)
				=> SpanComparer.Double.Compare(target.Span, other.Span);
		}
	}

}
