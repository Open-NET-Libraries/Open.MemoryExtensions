using System;

namespace Open.Memory
{
	public static class SpanComparer
	{
		public static int Compare<T>(in ReadOnlySpan<T> x, in ReadOnlySpan<T> y, Func<T, T, int> comparer)
			where T : IComparable<T>
		{
			if (x == y) return 0;
			var len = x.Length;
			if (len != y.Length)
				throw new ArgumentException("Lengths do not match.");

			for (var i = 0; i < len; i++)
			{
				var r = comparer(x[i], y[i]);
				if (r != 0) return r;
			}

			return 0;
		}

		public static int Compare<T>(in ReadOnlySpan<T> x, in ReadOnlySpan<T> y)
			where T : IComparable<T>
			=> Compare(x, y, Comparisons.Compare);

		public static class Float
		{
			public static int Compare(in ReadOnlySpan<float> x, in ReadOnlySpan<float> y)
			=> SpanComparer.Compare(x, y, Comparisons.Compare);
		}

		public static class Double
		{
			public static int Compare(in ReadOnlySpan<double> x, ReadOnlySpan<double> y)
				=> SpanComparer.Compare(x, y, Comparisons.Compare);
		}
	}

}
