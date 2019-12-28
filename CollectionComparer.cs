using System;
using System.Collections.Generic;
using System.Globalization;

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
		public static Func<int, int>? GetIndexComparer<T>(IReadOnlyCollection<T> x, IReadOnlyCollection<T> y, Func<T, T, int> comparison)
		{
			if (x is IReadOnlyList<T> xListReadOnly && y is IReadOnlyList<T> yListReadOnly)
				return i => comparison(xListReadOnly[i], yListReadOnly[i]);
			if (x is IList<T> xList && y is IList<T> yList)
				return i => comparison(xList[i], yList[i]);

			return null;
		}

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

			var comparer = GetIndexComparer(x, y, comparison);
			if (comparer == null)
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

				return 0;
			}

			for (var i = 0; i < len; i++)
			{
				var r = comparer(i);
				if (r != 0) return r;
			}

			return 0;
		}

		public static int Compare<T>(IReadOnlyCollection<T> x, IReadOnlyCollection<T> y)
			where T : IComparable<T>
			=> Compare(x, y, (a, b) => a.CompareTo(b));

		public static class Float
		{
			public static IComparer<IReadOnlyCollection<float>> Ascending { get; } = new CollectionFloatComparer(+1);
			public static IComparer<IReadOnlyCollection<float>> Descending { get; } = new CollectionFloatComparer(-1);

			public static int Compare(IReadOnlyCollection<float> x, IReadOnlyCollection<float> y)
				=> CollectionComparer.Compare(x, y, (a, b) =>
				{
					if (float.IsNaN(a))
						return float.IsNaN(b) ? 0 : -1;

					if (float.IsNaN(b))
						return +1;

					// ReSharper disable once CompareOfFloatsByEqualityOperator
					if (a == b
							|| Math.Abs(a - b) <= float.Epsilon
								&& a.ToString(CultureInfo.InvariantCulture) == b.ToString(CultureInfo.InvariantCulture))
						return 0; // We hate precision issues. :(  1==1 dammit!
					return a.CompareTo(b);
				});
		}

		public static class Double
		{
			public static IComparer<IReadOnlyCollection<double>> Ascending { get; } = new CollectionDoubleComparer(+1);
			public static IComparer<IReadOnlyCollection<double>> Descending { get; } = new CollectionDoubleComparer(-1);

			public static int Compare(IReadOnlyCollection<double> x, IReadOnlyCollection<double> y)
				=> CollectionComparer.Compare(x, y, (a, b) =>
				{
					if (double.IsNaN(a))
						return double.IsNaN(b) ? 0 : -1;

					if (double.IsNaN(b))
						return +1;

					// ReSharper disable once CompareOfFloatsByEqualityOperator
					if (a == b
							|| Math.Abs(a - b) < 0.00000001
								&& a.ToString(CultureInfo.InvariantCulture) == b.ToString(CultureInfo.InvariantCulture))
						return 0; // We hate precision issues. :(  1==1 dammit!

					return a.CompareTo(b);
				});
		}
	}
}
