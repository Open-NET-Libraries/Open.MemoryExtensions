using System;
using System.Collections.Generic;

namespace Open.Memory
{
	public class ArrayComparer<T> : IComparer<T[]>
		where T : IComparable<T>
	{
		ArrayComparer(int sign = +1)
		{
			Sign = sign;
		}
		public readonly int Sign;
		public int Compare(T[] x, T[] y)
			=> Sign * ArrayComparer.Compare(x, y);

		public static readonly IComparer<T[]> Ascending = new ArrayComparer<T>(+1);
		public static readonly IComparer<T[]> Descending = new ArrayComparer<T>(-1);
	}

	public static class ArrayComparer
	{
		public static int Compare<T>(in T[] x, in T[] y, Func<T, T, int> comparison)
		{
			if (x == y) return 0;
			if (x == null)
				throw new ArgumentNullException(nameof(x));
			if (y == null)
				throw new ArgumentNullException(nameof(y));
			var len = x.Length;
			if (len != y.Length)
				throw new ArgumentException("Lengths do not match.");

			for (var i = 0; i < len; i++)
			{
				var r = comparison(x[i], y[i]);
				if (r != 0) return r;
			}

			return 0;
		}

		public static int Compare<T>(in T[] x, in T[] y)
			where T : IComparable<T>
			=> Compare(x, y, (a, b) => a.CompareTo(b));

		public static int Compare(in float[] x, in float[] y)
			=> Compare(x, y, (a, b) =>
			{
				if (float.IsNaN(a))
					return float.IsNaN(b) ? 0 : -1;
				else if (float.IsNaN(b))
					return +1;

				if (a == b || Math.Abs(a - b) <= float.Epsilon && a.ToString() == b.ToString()) return 0; // We hate precision issues. :(  1==1 dammit!
				return a.CompareTo(b);
			});

		public static int Compare(in double[] x, in double[] y)
			=> Compare(x, y, (a, b) =>
			{
				if (double.IsNaN(a))
					return double.IsNaN(b) ? 0 : -1;
				else if (double.IsNaN(b))
					return +1;

				if (a == b || Math.Abs(a - b) < 0.00000001 && a.ToString() == b.ToString()) return 0; // We hate precision issues. :(  1==1 dammit!

				return a.CompareTo(b);
			});
	}
}
