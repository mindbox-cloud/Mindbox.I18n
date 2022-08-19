using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

public class DiscoveringFileSystemTranslationSource : FileSystemTranslationSourceBase
{
	private readonly string _baseDirectory;
	private readonly IReadOnlyList<string> _ignoredPathRules;

	public DiscoveringFileSystemTranslationSource(
		string baseDirectory,
		IReadOnlyList<ILocale> supportedLocales,
		IReadOnlyList<string> ignoredPathRules,
		ILogger logger) : base(supportedLocales, logger)
	{
		_baseDirectory = baseDirectory;
		_ignoredPathRules = ignoredPathRules;
	}

	protected override IEnumerable<string> GetTranslationFiles()
	{
		if (!Directory.Exists(_baseDirectory))
		{
			return Array.Empty<string>();
		}

		return Directory.GetFiles(
				_baseDirectory,
				$"*{TranslationFileSuffix}",
				SearchOption.AllDirectories)
			.Where(path => !_ignoredPathRules.Any(
				ignoredPart => path.IndexOf(ignoredPart, StringComparison.InvariantCultureIgnoreCase) > 0))
			.ToList();
	}
}