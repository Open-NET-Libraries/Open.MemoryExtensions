using System;
using System.Buffers;

namespace Open.Memory
{

	public struct TemporaryArray<T> : IDisposable
	{
		const int MaxLength = 1024 * 1024;
		ArrayPool<T> Pool { get; set; }
		public T[] Array { get; private set; }
		public Span<T> Span
			=> Array.Length == Length
			? Array.AsSpan()
			: Array.AsSpan().Slice(0, Length);

		public int Length { get; }
		public bool ClearOnReturn { get; }

		public TemporaryArray(ArrayPool<T> pool, int length, bool clearOnReturn = false)
		{
			if (length < 0)
				throw new ArgumentOutOfRangeException(nameof(length));

			Length = length;
			ClearOnReturn = clearOnReturn;

			if (length == 0)
			{
				Pool = null;
				Array = System.Array.Empty<T>();
			}
			if (pool==null || length > MaxLength)
			{
				Pool = null;
				Array = new T[length];
			}
			else
			{
				Pool = pool;
				Array = pool.Rent(length);
			}
		}

		public void Dispose()
		{
			var a = Array;
			Array = null;
			Pool?.Return(a, true);
			Pool = null;
		}

		public static implicit operator T[](TemporaryArray<T> source) => source.Array;
		public static implicit operator Span<T>(TemporaryArray<T> source) => source.Span;
	}

	public static class TemporaryArray
	{
		public static TemporaryArray<T> Create<T>(ArrayPool<T> pool, int length, bool clearOnReturn = false)
			=> new TemporaryArray<T>(pool, length, clearOnReturn);

		public static TemporaryArray<T> Create<T>(ArrayPool<T> pool, long length, bool clearOnReturn = false)
		{
			if (length < 0 || length > int.MaxValue)
				throw new ArgumentOutOfRangeException(nameof(length), length, "Must be at least zero and less than maximum 32 bit signed integer.");

			return new TemporaryArray<T>(pool, (int)length, clearOnReturn);
		}
	}
}
