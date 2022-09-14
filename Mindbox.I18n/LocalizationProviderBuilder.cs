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
using System.Linq;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

public class LocalizationProviderBuilder
{
	private readonly HashSet<string> _supportedLanguages = new();
	private ILogger? _logger;
	private ITranslationSource? _translationSource;

	public LocalizationProviderBuilder WithSupportedLanguage(string language)
	{
		_supportedLanguages.Add(language);
		return this;
	}

	public LocalizationProviderBuilder WithLogger(ILogger logger)
	{
		if (_logger != null)
			throw new InvalidOperationException("You can't set logger twice");

		_logger = logger;
		return this;
	}

	public LocalizationProviderBuilder WithTranslationSource(ITranslationSource translationSource)
	{
		if (_translationSource != null)
			throw new InvalidOperationException("You can't set translation source twice");

		_translationSource = translationSource ?? throw new ArgumentNullException(nameof(translationSource));
		return this;
	}

	public LocalizationProvider Build()
	{
		return new LocalizationProvider(new InitializationOptions
		{
			SupportedLanguages = _supportedLanguages.ToList(),
			TranslationSource = _translationSource ?? throw new InvalidOperationException(nameof(_translationSource)),
			Logger = _logger ?? throw new InvalidOperationException(nameof(_logger))
		});
	}
}