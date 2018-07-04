using System;

namespace Open.Memory
{

	public static class ArrayExtensions
	{
		public static int CompareTo<T>(this T[] target, T[] other)
			where T : IComparable<T>
			=> ArrayComparer.Compare(target, other);

		public static int CompareTo<T>(this in ReadOnlyMemory<T> target, in ReadOnlyMemory<T> other)
			where T : IComparable<T>
			=> SpanComparer.Compare(target.Span, other.Span);

		public static int CompareTo<T>(this in ReadOnlySpan<T> target, in ReadOnlySpan<T> other)
			where T : IComparable<T>
			=> SpanComparer.Compare(target, other);


		public static bool IsLessThan<T>(this T[] target, T[] other)
			where T : IComparable<T>
			=> ArrayComparer.Compare(target, other) < 0;

		public static bool IsEqual<T>(this T[] target, T[] other)
			where T : IComparable<T>
			=> ArrayComparer.Compare(target, other) == 0;

		public static bool IsGreaterThan<T>(this T[] target, T[] other)
			where T : IComparable<T>
			=> ArrayComparer.Compare(target, other) > 0;


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

	}
}
