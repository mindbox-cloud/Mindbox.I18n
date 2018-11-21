using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mindbox.I18n
{
	public class DiscoveringFileSystemTranslationSource : FileSystemTranslationSourceBase
	{
		protected readonly string BaseDirectory;
		protected readonly IReadOnlyList<string> IgnoredPathRules;

		public DiscoveringFileSystemTranslationSource(
			string baseDirectory,
			IReadOnlyList<Locale> supportedLocales,
			IReadOnlyList<string> ignoredPathRules) : base(supportedLocales)
		{
			BaseDirectory = baseDirectory;
			IgnoredPathRules = ignoredPathRules;
		}

		protected override IEnumerable<string> GetTranslationFiles()
		{
			return Directory.GetFiles(
					BaseDirectory,
					$"*{TranslationFileSuffix}",
					SearchOption.AllDirectories)
				.Where(path => !IgnoredPathRules.Any(
					ignoredPart => path.IndexOf(ignoredPart, StringComparison.InvariantCultureIgnoreCase) > 0))
				.ToList();
		}
	}
}