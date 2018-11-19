using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Mindbox.I18n.Analyzers
{
	public sealed class AnalyzerFileSystemTranslationSource : FileSystemTranslationSourceBase, IDisposable
	{
		private readonly string solutionFilePath;

		private readonly ConcurrentDictionary<string, FileSystemWatcher> projectFileWatchers = 
			new ConcurrentDictionary<string, FileSystemWatcher>(8, 200);

		private readonly ConcurrentDictionary<string, FileSystemWatcher> localizationFileSystemWatchers = 
			new ConcurrentDictionary<string, FileSystemWatcher>(8, 200);

		public AnalyzerFileSystemTranslationSource(
			string solutionFilePath,
			IReadOnlyList<Locale> supportedLocales)
			: base(supportedLocales)
		{
			this.solutionFilePath = solutionFilePath;
		}

		public override void Initialize()
		{
			var projectFiles = GetProjectFiles(solutionFilePath);

			foreach (var projectFile in projectFiles)
			{
				var watcher = new FileSystemWatcher
				{
					Path = Path.GetDirectoryName(projectFile),
					Filter = Path.GetFileName(projectFile),
					IncludeSubdirectories = false,
					NotifyFilter = NotifyFilters.LastWrite
				};

				watcher.Changed += (s, ea) => HandleProjectFileChange(projectFile);

				if (projectFileWatchers.TryAdd(projectFile, watcher))
				{
					watcher.EnableRaisingEvents = true;
				}
				else
				{
					watcher.Dispose();
				}
			}

			var localizationFiles = projectFileWatchers.Keys.SelectMany(GetLocalizationFilesFromProjectFile);
			LoadLocalizationFiles(localizationFiles);

			base.Initialize();
		}

		private void LoadLocalizationFiles(IEnumerable<string> localizationFiles)
		{
			foreach (var localizationFile in localizationFiles)
			{
				var watcher = new FileSystemWatcher
				{
					Path = Path.GetDirectoryName(localizationFile),
					Filter = Path.GetFileName(localizationFile),
					IncludeSubdirectories = false,
					NotifyFilter = NotifyFilters.LastWrite
				};

				watcher.Changed += (s, ea) => HandleLocalizationFileChange(localizationFile);

				if (localizationFileSystemWatchers.TryAdd(localizationFile, watcher))
				{
					watcher.EnableRaisingEvents = true;
				}
				else
				{
					watcher.Dispose();
				}
			}
		}

		private void HandleLocalizationFileChange(string localizationFile)
		{
			LoadTranslationFile(localizationFile);
		}

		private void HandleProjectFileChange(string projectFile)
		{
			var localizationFilesFromProject = GetLocalizationFilesFromProjectFile(projectFile);

			var localizationFilesToAdd = localizationFilesFromProject.Except(localizationFileSystemWatchers.Keys);
			LoadLocalizationFiles(localizationFilesToAdd);
		}

		private IEnumerable<string> GetLocalizationFilesFromProjectFile(string projectFile)
		{
			var document = XDocument.Parse(File.ReadAllText(projectFile));

			return document.Descendants()
				.Where(x => x.Name.LocalName == "ItemGroup")
				.SelectMany(itemGroup => itemGroup.Descendants()
					.Where(y => y.Attribute("Include")?.Value
				        .EndsWith("i18n.json", StringComparison.InvariantCultureIgnoreCase) ?? false)
					.Select(node => Path.Combine(
						Path.GetDirectoryName(projectFile),
						node.Attribute("Include").Value)));
		}

		private IEnumerable<string> GetProjectFiles(string solutionFilePath)
		{
			var projectRelativePaths = SolutionFileParser.GetProjectRelativePaths(solutionFilePath);
			var solutionDirectory = Path.GetDirectoryName(solutionFilePath);
			if (solutionDirectory == null)
				throw new InvalidOperationException($"Couldn't get directory name from {solutionFilePath}");

			return projectRelativePaths.Select(projectRelativePath =>
				Path.Combine(solutionDirectory, projectRelativePath));
		}

		protected override IEnumerable<string> GetTranslationFiles()
		{
			return localizationFileSystemWatchers.Keys;
		}

		public void Dispose()
		{
			foreach (var localizationFileSystemWatcher in localizationFileSystemWatchers.Values)
			{
				localizationFileSystemWatcher.EnableRaisingEvents = false;
				localizationFileSystemWatcher.Dispose();
			}

			foreach (var projectFileWatcher in projectFileWatchers.Values)
			{
				projectFileWatcher.EnableRaisingEvents = false;
				projectFileWatcher.Dispose();
			}
		}
	}
}
