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

namespace Mindbox.I18n.Tests;

[TestClass]
public sealed class FileSystemTranslationSourceCollisionTests
{
	[TestMethod]
	public void Initialize_LogsError_WhenMultipleFilesResolveToSameNamespaceAndLocale()
	{
		using var directory = new TemporaryLocalizationDirectory();
		directory.WriteFile("Foo.A.en-US.i18n.json", """{ "Foo:key": "A" }""");
		directory.WriteFile("Foo.B.en-US.i18n.json", """{ "Foo:key": "B" }""");

		var logger = new CapturingLogger();
		var source = new DiscoveringFileSystemTranslationSource(
			directory.Path, new[] { Locales.enUS }, Array.Empty<string>(), logger, prefix: null);

		source.Initialize();

		Assert.IsTrue(
			logger.Errors.Exists(message => message.Contains("provided by multiple files", StringComparison.Ordinal)),
			"Expected an error about the namespace being provided by multiple files, got: "
				+ string.Join("; ", logger.Errors));
	}

	[TestMethod]
	public void Initialize_DoesNotLogError_WhenPrefixKeepsFilesApart()
	{
		using var directory = new TemporaryLocalizationDirectory();
		directory.WriteFile("Foo.A.en-US.i18n.json", """{ "Foo:key": "A" }""");
		directory.WriteFile("Foo.B.en-US.i18n.json", """{ "Foo:key": "B" }""");

		var logger = new CapturingLogger();
		var source = new DiscoveringFileSystemTranslationSource(
			directory.Path, new[] { Locales.enUS }, Array.Empty<string>(), logger, prefix: "A");

		source.Initialize();

		Assert.AreEqual(0, logger.Errors.Count, string.Join("; ", logger.Errors));
	}

	private sealed class TemporaryLocalizationDirectory : IDisposable
	{
		public string Path { get; } =
			System.IO.Path.Combine(System.IO.Path.GetTempPath(), "i18n-collision-" + Guid.NewGuid().ToString("N"));

		public TemporaryLocalizationDirectory() => Directory.CreateDirectory(Path);

		public void WriteFile(string name, string content) =>
			File.WriteAllText(System.IO.Path.Combine(Path, name), content);

		public void Dispose()
		{
			if (Directory.Exists(Path))
				Directory.Delete(Path, recursive: true);
		}
	}

	private sealed class CapturingLogger : ILogger
	{
		public List<string> Errors { get; } = new();

		public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

		public bool IsEnabled(LogLevel logLevel) => true;

		public void Log<TState>(
			LogLevel logLevel,
			EventId eventId,
			TState state,
			Exception? exception,
			Func<TState, Exception?, string> formatter)
		{
			if (logLevel == LogLevel.Error)
				Errors.Add(formatter(state, exception));
		}

		private sealed class NullScope : IDisposable
		{
			public static readonly NullScope Instance = new();
			public void Dispose() { }
		}
	}
}
