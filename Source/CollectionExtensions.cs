using System;
using System.Buffers;
using System.Collections.Generic;

namespace Open.Memory
{

	public static class CollectionExtensions
	{
		public static int CompareTo<T>(this IReadOnlyCollection<T> target, IReadOnlyCollection<T> other)
			where T : IComparable<T>
			=> CollectionComparer.Compare(target, other);

		public static int CompareTo<T>(this in ReadOnlyMemory<T> target, in ReadOnlyMemory<T> other)
			where T : IComparable<T>
			=> SpanComparer.Compare(target.Span, other.Span);

		public static int CompareTo<T>(this in ReadOnlySpan<T> target, in ReadOnlySpan<T> other)
			where T : IComparable<T>
			=> SpanComparer.Compare(target, other);


		public static bool IsLessThan<T>(this IReadOnlyCollection<T> target, IReadOnlyCollection<T> other)
			where T : IComparable<T>
			=> CollectionComparer.Compare(target, other) < 0;

		public static bool IsEqual<T>(this IReadOnlyCollection<T> target, IReadOnlyCollection<T> other)
			where T : IComparable<T>
			=> CollectionComparer.Compare(target, other) == 0;

		public static bool IsGreaterThan<T>(this IReadOnlyCollection<T> target, IReadOnlyCollection<T> other)
			where T : IComparable<T>
			=> CollectionComparer.Compare(target, other) > 0;


		public static bool IsLessThan<T>(this in ReadOnlySpan<T> target, in ReadOnlySpan<T> other)
			where T : IComparable<T>
			=> SpanComparer.Compare(target, other) < 0;

		public static bool IsEqual<T>(this in ReadOnlySpan<T> target, in ReadOnlySpan<T> other)
			where T : IComparable<T>
			=> SpanComparer.Compare(target, other) == 0;

		public static bool IsGreaterThan<T>(this in ReadOnlySpan<T> target, in ReadOnlySpan<T> other)
			where T : IComparable<T>
			=> SpanComparer.Compare(target, other) > 0;


		public static bool IsLessThan<T>(this in ReadOnlyMemory<T> target, in ReadOnlyMemory<T> other)
			where T : IComparable<T>
			=> SpanComparer.Compare(target.Span, other.Span) < 0;

		public static bool IsEqual<T>(this in ReadOnlyMemory<T> target, in ReadOnlyMemory<T> other)
			where T : IComparable<T>
			=> SpanComparer.Compare(target.Span, other.Span) == 0;

		public static bool IsGreaterThan<T>(this in ReadOnlyMemory<T> target, in ReadOnlyMemory<T> other)
			where T : IComparable<T>
			=> SpanComparer.Compare(target.Span, other.Span) > 0;

		/// <summary>
		/// Rents a buffer from the ArrayPool but returns a DisposeHandler with the buffer as it's value.
		/// Facilitiates containing the temporary use of a buffer within a using block.
		/// If the mimimumLength exceeds 1024*1024, an array will be created at that length for use.
		/// </summary>
		/// <typeparam name="T">The type of the array.</typeparam>
		/// <param name="pool">The pool to get the array from.</param>
		/// <param name="minimumLength">The minimum length of the array.</param>
		/// <param name="clearArrayOnReturn">If true, will clear the array when it is returned to the pool.</param>
		/// <returns>A disposable struct containing an array of type T[] that is at least minimumLength in length.</returns>
		public static TemporaryArray<T> RentDisposable<T>(this ArrayPool<T> pool, int minimumLength, bool clearArrayOnReturn = false)
			where T : struct
		{
			if (minimumLength < 0)
				throw new ArgumentOutOfRangeException(nameof(minimumLength));

			return TemporaryArray.Create(pool, minimumLength, clearArrayOnReturn);
		}

		/// <summary>
		/// Rents a buffer from the ArrayPool but returns a DisposeHandler with the buffer as it's value.
		/// Facilitiates containing the temporary use of a buffer within a using block.
		/// If the mimimumLength exceeds 1024*1024, an array will be created at that length for use.
		/// </summary>
		/// <typeparam name="T">The type of the array.</typeparam>
		/// <param name="pool">The pool to get the array from.</param>
		/// <param name="minimumLength">The minimum length of the array.  Must be no greater than Int32.MaxValue.</param>
		/// <param name="clearArrayOnReturn">If true, will clear the array when it is returned to the pool.</param>
		/// <returns>A disposable struct containing an array of type T[] that is at least minimumLength in length.</returns>
		public static TemporaryArray<T> RentDisposable<T>(this ArrayPool<T> pool, long minimumLength, bool clearArrayOnReturn = false)
			where T : struct
		{
			if (minimumLength < 0)
				throw new ArgumentOutOfRangeException(nameof(minimumLength));

			return TemporaryArray.Create(pool, minimumLength, clearArrayOnReturn);
		}


		/// <summary>
		/// Rents a buffer from the ArrayPool but returns a DisposeHandler with the buffer as it's value.
		/// Facilitiates containing the temporary use of a buffer within a using block.
		/// If the mimimumLength exceeds 1024*1024, an array will be created at that length for use.
		/// </summary>
		/// <typeparam name="T">The type of the array.</typeparam>
		/// <param name="pool">The pool to get the array from.</param>
		/// <param name="minimumLength">The minimum length of the array.</param>
		/// <returns>A disposable struct containing an array of type T[] that is at least minimumLength in length.</returns>
		public static TemporaryArray<T> RentDisposable<T>(this ArrayPool<T> pool, int minimumLength)
			where T : class
		{
			if (minimumLength < 0)
				throw new ArgumentOutOfRangeException(nameof(minimumLength));

			// Because they are references, we should surely clear.
			return TemporaryArray.Create(pool, minimumLength);
		}

		/// <summary>
		/// Rents a buffer from the ArrayPool but returns a DisposeHandler with the buffer as it's value.
		/// Facilitiates containing the temporary use of a buffer within a using block.
		/// If the mimimumLength exceeds 1024*1024, an array will be created at that length for use.
		/// </summary>
		/// <typeparam name="T">The type of the array.</typeparam>
		/// <param name="pool">The pool to get the array from.</param>
		/// <param name="minimumLength">The minimum length of the array.  Must be no greater than Int32.MaxValue.</param>
		/// <returns>A disposable struct containing an array of type T[] that is at least minimumLength in length.</returns>
		public static TemporaryArray<T> RentDisposable<T>(this ArrayPool<T> pool, long minimumLength)
			where T : class
		{
			if (minimumLength < 0)
				throw new ArgumentOutOfRangeException(nameof(minimumLength));

			// Because they are references, we should surely clear.
			return TemporaryArray.Create(pool, minimumLength);
		}

	}
}
