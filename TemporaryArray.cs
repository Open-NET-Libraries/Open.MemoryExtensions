using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace Open.Memory
{

	public struct TemporaryArray<T> : IDisposable, IList<T>, IReadOnlyList<T>
	{
		ArrayPool<T>? Pool;
		public T[] Array { get; private set; }
		public Span<T> Span
			=> Array.Length == Length
			? Array.AsSpan()
			: Array.AsSpan().Slice(0, Length);

		public T this[int index]
		{
			get => Array[index];
			set {
				if (index < Length) Array[index] = value;
				else throw new IndexOutOfRangeException();
			}
		}

		public int Length { get; }

		public bool ClearOnReturn { get; }

		int IReadOnlyCollection<T>.Count => Length;

		bool ICollection<T>.IsReadOnly => false;

		public int Count => ((IList<T>)Array).Count;

		int ICollection<T>.Count => throw new NotImplementedException();

		internal TemporaryArray(ArrayPool<T> pool, int length, bool clearOnReturn = false)
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
			if (pool == null)
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
			Array = null!;
			Pool?.Return(a, true);
			Pool = null;
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (var i = 0; i < Length; i++)
				yield return Array[i];
		}

		public int IndexOf(T item)
			=> System.Array.IndexOf(Array, item, 0, Length);

		public bool Contains(T item)
			=> IndexOf(item) != -1;

		public void CopyTo(T[] array, int arrayIndex)
		{
			var iLimit = Length;
			var aLimit = array.Length;
			var i = 0;
			if (arrayIndex == 0)
			{

				while (i < iLimit && i < aLimit)
				{
					array[i] = Array[i];
					++i;
				}
			}
			else
			{
				var a = arrayIndex;
				while (i < iLimit && a < aLimit)
					array[a++] = Array[i++];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		void IList<T>.Insert(int index, T item) => throw new NotImplementedException();
		void IList<T>.RemoveAt(int index) => throw new NotImplementedException();
		void ICollection<T>.Add(T item) => throw new NotImplementedException();
		bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
		void ICollection<T>.Clear() => throw new NotImplementedException();

		public static implicit operator Span<T>(TemporaryArray<T> source) => source.Span;
	}

	public static class TemporaryArray
	{
		public static TemporaryArray<T> Create<T>(ArrayPool<T> pool, int length, bool clearOnReturn = false)
			where T : struct
			=> new TemporaryArray<T>(pool, length, clearOnReturn);

		public static TemporaryArray<T> Create<T>(ArrayPool<T> pool, long length, bool clearOnReturn = false)
			where T : struct
		{
			if (length < 0 || length > int.MaxValue)
				throw new ArgumentOutOfRangeException(nameof(length), length, "Must be at least zero and less than maximum 32 bit signed integer.");

			return new TemporaryArray<T>(pool, (int)length, clearOnReturn);
		}

		public static TemporaryArray<T> Create<T>(ArrayPool<T> pool, int length)
			where T : class
			=> new TemporaryArray<T>(pool, length, true);

		public static TemporaryArray<T> Create<T>(ArrayPool<T> pool, long length)
			where T : class
		{
			if (length < 0 || length > int.MaxValue)
				throw new ArgumentOutOfRangeException(nameof(length), length, "Must be at least zero and less than maximum 32 bit signed integer.");

			return new TemporaryArray<T>(pool, (int)length, true);
		}
	}

}
