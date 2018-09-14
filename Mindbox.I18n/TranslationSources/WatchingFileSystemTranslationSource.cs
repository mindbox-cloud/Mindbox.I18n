using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mindbox.I18n
{
	public sealed class WatchingFileSystemTranslationSource : FileSystemTranslationSource, IDisposable
	{
		private FileSystemWatcher watcher;
		private DateTime lastReloadDateTime;

		public WatchingFileSystemTranslationSource(
			string baseDirectory,
			IReadOnlyList<Locale> supportedLocales,
			IReadOnlyList<string> ignoredPathRules)
			: base(baseDirectory, supportedLocales, ignoredPathRules)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			watcher = new FileSystemWatcher
			{
				Path = BaseDirectory,
				Filter = $"*{TranslationFileSuffix}",
				IncludeSubdirectories = true,
				NotifyFilter = NotifyFilters.LastWrite
					| NotifyFilters.FileName
					| NotifyFilters.DirectoryName
			};
			// Can be optimized to handle file changes in a more granular manner
			watcher.Changed += (s, ea) => HandleFileChange(ea.FullPath);
			watcher.Created += (s, ea) => HandleFileChange(ea.FullPath);
			watcher.Deleted += (s, ea) => HandleFileChange(ea.FullPath);
			watcher.Renamed += (s, ea) => HandleFileChange(ea.FullPath);

			watcher.EnableRaisingEvents = true;
		}

		private void HandleFileChange(string filePath)
		{
			if (IgnoredPathRules.Any(ignoredPart => filePath.IndexOf(ignoredPart, StringComparison.InvariantCultureIgnoreCase) > 0))
				return;

			// In some scenarios a lot of file changes might occur almost simultaneously
			// (for example, editing files in VS results in multiple file changes and temp file manipulations).

			// This prevents excessive reloading of translation files.
			if (lastReloadDateTime == default || DateTime.Now - lastReloadDateTime > TimeSpan.FromSeconds(2))
			{
				lastReloadDateTime = DateTime.Now;
				LoadTranslationFiles();
			}
		}

		public void Dispose()
		{
			if (watcher != null)
			{
				watcher.EnableRaisingEvents = false;
				watcher?.Dispose();
			}
		}
	}
}
