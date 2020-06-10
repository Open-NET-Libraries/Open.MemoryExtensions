using System;
using System.Globalization;

namespace Open.Memory
{
	public static class Comparisons
	{
		public static int Compare<T>(T a, T b)
			where T : IComparable<T>
			=> a.CompareTo(b);

		public static int Compare(float a, float b)
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
		}

		public static int Compare(double a, double b)
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
		}
	}
}
