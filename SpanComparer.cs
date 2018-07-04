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
			=> Compare(x, y, (a, b) => a.CompareTo(b));

		public static class Float
		{
			public static int Compare(in ReadOnlySpan<float> x, in ReadOnlySpan<float> y)
			=> SpanComparer.Compare(x, y, (a, b) =>
			{
				if (float.IsNaN(a))
					return float.IsNaN(b) ? 0 : -1;
				else if (float.IsNaN(b))
					return +1;

				//if (a == b || Math.Abs(a - b) <= float.Epsilon && a.ToString() == b.ToString()) return 0; // We hate precision issues. :(  1==1 dammit!
				return a.CompareTo(b);
			});
		}

		public static class Double
		{
			public static int Compare(in ReadOnlySpan<double> x, ReadOnlySpan<double> y)
			=> SpanComparer.Compare(x, y, (a, b) =>
			{
				if (double.IsNaN(a))
					return double.IsNaN(b) ? 0 : -1;
				else if (double.IsNaN(b))
					return +1;

				//if (a == b || Math.Abs(a - b) < 0.00000001 && a.ToString() == b.ToString()) return 0; // We hate precision issues. :(  1==1 dammit!
				return a.CompareTo(b);
			});
		}
	}

}
