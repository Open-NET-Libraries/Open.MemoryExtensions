using System;

namespace Open.Memory
{
	public static class SpanExtensions
	{

		/// <summary>
		/// Finds the first instance of a sequence and returns the set of values up to that sequence and emits the remaining as an out value.
		/// </summary>
		/// <param name="source">The source set to look through.</param>
		/// <param name="character">The sequence to find.</param>
		/// <param name="remaining">The remaining set after the sequence.  Will include entire source sequence if not found.</param>
		/// <returns>The portion of the source up to and excluding the sequence searched for.</returns>
		public static ReadOnlySpan<T> FirstSplit<T>(this in ReadOnlySpan<T> source,
			in ReadOnlySpan<T> splitSequence,
			out ReadOnlySpan<T> remaining)
			where T : IEquatable<T>
		{
			if (splitSequence.Length == 0)
				throw new ArgumentException("Cannot split using empty sequence.", nameof(splitSequence));

			var i = source.Length == 0 ? -1 : source.IndexOf(splitSequence);
			if (i == -1)
			{
				remaining = ReadOnlySpan<T>.Empty;
				return source;
			}

			remaining = source.Slice(i + 1);
			return source.Slice(0, i);
		}

		/// <summary>
		/// Finds the first instance of a character sequence and returns the set of characters up to that sequence and emits the remaining as an out parameter.
		/// </summary>
		/// <param name="source">The source characters to look through.</param>
		/// <param name="character">The sequence to find.</param>
		/// <param name="remaining">The remaining characters after the sequence.  Will include entire source sequence if not found.</param>
		/// <param name="comparisonType">The string comparison type to use.</param>
		/// <returns>The portion of the source up to and excluding the sequence searched for.</returns>
		public static ReadOnlySpan<char> FirstSplit(this in ReadOnlySpan<char> source,
			in ReadOnlySpan<char> splitSequence,
			out ReadOnlySpan<char> remaining,
			StringComparison comparisonType)
		{
			if (splitSequence.Length == 0)
				throw new ArgumentException("Cannot split using empty sequence.", nameof(splitSequence));

			var i = source.Length == 0 ? -1 : source.IndexOf(splitSequence, comparisonType);
			if (i == -1)
			{
				remaining = ReadOnlySpan<char>.Empty;
				return source;
			}

			remaining = source.Slice(i + 1);
			return source.Slice(0, i);
		}

		/// <summary>
		/// Finds the first instance of a character and returns the set of characters up to that character and emits the remaining as an out parameter.
		/// </summary>
		/// <param name="source">The source characters to look through.</param>
		/// <param name="character">The character to find.</param>
		/// <param name="remaining">The remaining characters after the character.  Will include entire source sequence if not found.</param>
		/// <returns>The portion of the source up to and excluding the character searched for.</returns>
		public static ReadOnlySpan<T> FirstSplit<T>(this in ReadOnlySpan<T> source,
			in T character,
			out ReadOnlySpan<T> remaining)
			where T : IEquatable<T>
		{
			var i = source.Length == 0 ? -1 : source.IndexOf(character);
			if (i == -1)
			{
				remaining = ReadOnlySpan<T>.Empty;
				return source;
			}

			remaining = source.Slice(i + 1);
			return source.Slice(0, i);
		}

	}
}
