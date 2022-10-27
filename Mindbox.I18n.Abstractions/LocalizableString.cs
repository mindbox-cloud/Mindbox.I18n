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

using Microsoft.Extensions.Logging;

namespace Mindbox.I18n.Abstractions;

public abstract class LocalizableString
{
	private static ILogger? Logger { get; set; }

	public static void InitializeLogger(ILogger logger)
	{
		Logger = logger;
	}

	public static LocalizableString ForKey([LocalizationKey] string key) =>
		new LocaleDependentString(key ?? throw new ArgumentNullException(nameof(key)));

	public static LocalizableString LocaleIndependent(string localeIndependentString) =>
		new LocaleIndependentString(localeIndependentString);

#pragma warning disable CA2225
	public static implicit operator LocalizableString(string key) =>
		new LocaleDependentString(key);
#pragma warning restore CA2225

	public abstract string Key { get; }

	public LocalizationTemplateParameters? LocalizationParameters { get; set; }

	public override string ToString()
	{
		Logger?.LogError($"ToString() called on LocalizableString with key {Key}");
		return Key;
	}

	public LocalizableString WithParameters(Action<LocalizationTemplateParameters> configureParameters)
	{
		if (configureParameters == null)
			throw new ArgumentNullException(nameof(configureParameters));

		var parameters = new LocalizationTemplateParameters();
		configureParameters?.Invoke(parameters);
		LocalizationParameters = parameters;

		return this;
	}
}