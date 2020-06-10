using System;
using System.Collections.Generic;

namespace Open.Memory
{
	public class CollectionComparer<T> : IComparer<IReadOnlyCollection<T>>
		where T : IComparable<T>
	{
		CollectionComparer(int sign = +1)
		{
			Sign = sign;
		}
		public readonly int Sign;
		public int Compare(IReadOnlyCollection<T> x, IReadOnlyCollection<T> y)
			=> Sign * CollectionComparer.Compare(x, y);

		public static IComparer<IReadOnlyCollection<T>> Ascending { get; } = new CollectionComparer<T>(+1);
		public static IComparer<IReadOnlyCollection<T>> Descending { get; } = new CollectionComparer<T>(-1);
	}

	class CollectionFloatComparer : IComparer<IReadOnlyCollection<float>>
	{
		internal CollectionFloatComparer(int sign = +1)
		{
			Sign = sign;
		}
		public readonly int Sign;
		public int Compare(IReadOnlyCollection<float> x, IReadOnlyCollection<float> y)
			=> Sign * CollectionComparer.Compare(x, y);
	}

	class CollectionDoubleComparer : IComparer<IReadOnlyCollection<double>>
	{
		internal CollectionDoubleComparer(int sign = +1)
		{
			Sign = sign;
		}
		public readonly int Sign;
		public int Compare(IReadOnlyCollection<double> x, IReadOnlyCollection<double> y)
			=> Sign * CollectionComparer.Compare(x, y);
	}

	public static class CollectionComparer
	{
		public static int Compare<T>(IReadOnlyCollection<T> x, IReadOnlyCollection<T> y, Func<T, T, int> comparison)
		{
			if (x == y) return 0;
			if (x == null)
				throw new ArgumentNullException(nameof(x));
			if (y == null)
				throw new ArgumentNullException(nameof(y));
			var len = x.Count;
			if (len != y.Count)
				throw new ArgumentException("Lengths do not match.");

			if (x is IReadOnlyList<T> xListReadOnly && y is IReadOnlyList<T> yListReadOnly)
			{
				for (var i = 0; i < len; i++)
				{
					var r = comparison(xListReadOnly[i], yListReadOnly[i]);
					if (r != 0) return r;
				}
			}
			else if (x is IList<T> xList && y is IList<T> yList)
			{
				for (var i = 0; i < len; i++)
				{
					var r = comparison(xList[i], yList[i]);
					if (r != 0) return r;
				}
			}
			else
			{
				using var eX = x.GetEnumerator();
				using var eY = y.GetEnumerator();

			retry:
				bool more = eX.MoveNext();
				if (more != eY.MoveNext()) throw new ArgumentException("Enumeration count does not match.");
				if (more)
				{
					var r = comparison(eX.Current, eY.Current);
					if (r != 0) return r;
					goto retry;
				}
			}

			return 0;
		}

		public static int Compare<T>(IReadOnlyCollection<T> x, IReadOnlyCollection<T> y)
			where T : IComparable<T>
			=> Compare(x, y, Comparisons.Compare);
		public static class Float
		{
			public static IComparer<IReadOnlyCollection<float>> Ascending { get; } = new CollectionFloatComparer(+1);
			public static IComparer<IReadOnlyCollection<float>> Descending { get; } = new CollectionFloatComparer(-1);

			public static int Compare(IReadOnlyCollection<float> x, IReadOnlyCollection<float> y)
				=> CollectionComparer.Compare(x, y, Comparisons.Compare);
		}

		public static class Double
		{
			public static IComparer<IReadOnlyCollection<double>> Ascending { get; } = new CollectionDoubleComparer(+1);
			public static IComparer<IReadOnlyCollection<double>> Descending { get; } = new CollectionDoubleComparer(-1);

			public static int Compare(IReadOnlyCollection<double> x, IReadOnlyCollection<double> y)
				=> CollectionComparer.Compare(x, y, Comparisons.Compare);
		}
	}
}
