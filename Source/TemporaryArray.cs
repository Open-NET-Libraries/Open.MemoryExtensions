using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Open.Memory
{

	public class TemporaryArray<T> : IDisposable, IList<T>, IReadOnlyList<T>
	{
		ArrayPool<T>? _pool;
		T[]? _array;

		public T[] Array => _array ?? throw new ObjectDisposedException(nameof(TemporaryArray));

		public Span<T> Span
			=> Array.Length == Length
			? Array.AsSpan()
			: Array.AsSpan().Slice(0, Length);

		public T this[int index]
		{
			get => Array[index];
			set
			{
				if (index < Length) Array[index] = value;
				else throw new IndexOutOfRangeException();
			}
		}

		public int Length { get; }

		public bool ClearOnReturn { get; }

		bool ICollection<T>.IsReadOnly => false;

		public int Count => Length;
		int IReadOnlyCollection<T>.Count => Length;
		int ICollection<T>.Count => Length;

		public int Capacity => Array.Length;

		internal TemporaryArray(ArrayPool<T> pool, T[] array, int length, bool clearOnReturn = false)
		{
			if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
			_array = array ?? throw new ArgumentNullException(nameof(array));
			Contract.EndContractBlock();

			_pool = pool;
			Length = length;
			ClearOnReturn = clearOnReturn;
		}

		public void Dispose()
		{
			var p = _pool;
			_pool = null;
			var a = _array;
			_array = null;
			p?.Return(a, true);
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
		static TemporaryArray<T> CreateInternal<T>(ArrayPool<T> pool, int length, bool clearOnReturn)
			=> new TemporaryArray<T>(pool, pool?.Rent(length) ?? new T[length], length, clearOnReturn);

		public static TemporaryArray<T> Create<T>(ArrayPool<T> pool, int length, bool clearOnReturn = false)
			where T : struct
			=> CreateInternal(pool, length, clearOnReturn);

		public static TemporaryArray<T> Create<T>(ArrayPool<T> pool, long length, bool clearOnReturn = false)
			where T : struct
		{
			if (length < 0 || length > int.MaxValue)
				throw new ArgumentOutOfRangeException(nameof(length), length, "Must be at least zero and less than maximum 32 bit signed integer.");

			return CreateInternal(pool, (int)length, clearOnReturn);
		}

		public static TemporaryArray<T> Create<T>(ArrayPool<T> pool, int length)
			where T : class
			=> CreateInternal(pool, length, true);

		public static TemporaryArray<T> Create<T>(ArrayPool<T> pool, long length)
			where T : class
		{
			if (length < 0 || length > int.MaxValue)
				throw new ArgumentOutOfRangeException(nameof(length), length, "Must be at least zero and less than maximum 32 bit signed integer.");

			return CreateInternal(pool, (int)length, true);
		}


		public static TemporaryArray<T> Create<T>(int length)
			=> CreateInternal(ArrayPool<T>.Shared, length, true);

		public static TemporaryArray<T> Create<T>(long length)
		{
			if (length < 0 || length > int.MaxValue)
				throw new ArgumentOutOfRangeException(nameof(length), length, "Must be at least zero and less than maximum 32 bit signed integer.");

			return CreateInternal(ArrayPool<T>.Shared, (int)length, true);
		}
	}

}
