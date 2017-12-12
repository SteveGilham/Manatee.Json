﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class NullParser : IJsonParser
	{
		private const string _unexpectedEndOfInput = "Unexpected end of input.";

		public bool Handles(char c)
		{
			return c == 'n' || c == 'N';
		}

		public string TryParse(string source, ref int index, out JsonValue value, bool allowExtraChars)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			value = null;

			if (index + 4 > source.Length)
				return _unexpectedEndOfInput;

			if (source.IndexOf("null", index, 4, StringComparison.OrdinalIgnoreCase) != index)
				return $"Value not recognized: '{source.Substring(index, 4)}'.";

			index += 4;
			value = JsonValue.Null;
			return null;
		}

		public string TryParse(TextReader stream, out JsonValue value)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			value = null;

			var buffer = SmallBufferCache.Acquire(4);
			var charsRead = stream.ReadBlock(buffer, 0, 4);
			if (charsRead != 4)
			{
				SmallBufferCache.Release(buffer);
				return _unexpectedEndOfInput;
			}

			string errorMessage = null;
			if ((buffer[0] == 'n' || buffer[0] == 'N') &&
			    (buffer[1] == 'u' || buffer[1] == 'U') &&
			    (buffer[2] == 'l' || buffer[2] == 'L') &&
			    (buffer[3] == 'l' || buffer[3] == 'L'))
				value = JsonValue.Null;
			else
				errorMessage = $"Value not recognized: '{new string(buffer).Trim('\0')}'.";

			SmallBufferCache.Release(buffer);
			return errorMessage;
		}
		public async Task<(string errorMessage, JsonValue value)> TryParseAsync(TextReader stream, CancellationToken token)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			var buffer = SmallBufferCache.Acquire(4);
			var count = await stream.ReadBlockAsync(buffer, 0, 4);
			if (count < 4)
			{
				SmallBufferCache.Release(buffer);
				return ("Unexpected end of input.", null);
			}

			var value = JsonValue.Null;
			string errorMessage = null;
			if (buffer[0] != 'n' && buffer[0] != 'N' &&
			    buffer[1] != 'u' && buffer[1] != 'U' &&
			    buffer[2] != 'l' && buffer[2] != 'L' &&
			    buffer[3] != 'l' && buffer[3] != 'L')
			{
				value = null;
				errorMessage = $"Value not recognized: '{new string(buffer).Trim('\0')}'.";
			}

			SmallBufferCache.Release(buffer);
			return (errorMessage, value);
		}
	}
}