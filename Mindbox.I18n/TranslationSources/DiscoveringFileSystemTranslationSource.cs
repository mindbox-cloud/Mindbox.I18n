// Copyright 2022 Mindbox Ltd
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

public class DiscoveringFileSystemTranslationSource : FileSystemTranslationSourceBase
{
	private readonly string _baseDirectory;
	private readonly IReadOnlyList<string> _ignoredPathRules;
	// TODO make not nullable after migration
	private readonly Subdivision? _subdivision;

	public DiscoveringFileSystemTranslationSource(
		string baseDirectory,
		IReadOnlyList<ILocale> supportedLocales,
		IReadOnlyList<string> ignoredPathRules,
		ILogger logger,
		Subdivision? subdivision = null) : base(supportedLocales, logger)
	{
		_baseDirectory = baseDirectory;
		_ignoredPathRules = ignoredPathRules;
		_subdivision = subdivision;
	}

	protected override IEnumerable<string> GetTranslationFiles()
	{
		if (!Directory.Exists(_baseDirectory))
		{
			return Array.Empty<string>();
		}

		var searchPattern = _subdivision is null
			? $"*{TranslationFileSuffix}"
			: $"*.{_subdivision.ToString()}*{TranslationFileSuffix}";

		return Directory.GetFiles(
				_baseDirectory,
				searchPattern,
				SearchOption.AllDirectories)
			.Where(path => !_ignoredPathRules.Any(
				ignoredPart => path.IndexOf(ignoredPart, StringComparison.InvariantCultureIgnoreCase) > 0))
			.ToList();
	}
}