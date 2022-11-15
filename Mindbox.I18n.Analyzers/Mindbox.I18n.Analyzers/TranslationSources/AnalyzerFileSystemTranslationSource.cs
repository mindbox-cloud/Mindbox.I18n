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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n.Analyzers;
#nullable disable
public sealed class AnalyzerFileSystemTranslationSource : FileSystemTranslationSourceBase, IDisposable
{
	private readonly string _solutionFilePath;

	private readonly ConcurrentDictionary<string, FileSystemWatcher> _projectFileWatchers =
		new(8, 200);

	private readonly ConcurrentDictionary<string, FileSystemWatcher> _localizationFileSystemWatchers =
		new(8, 200);

	public AnalyzerFileSystemTranslationSource(
		string solutionFilePath,
		IReadOnlyList<ILocale> supportedLocales,
		ILogger logger)
		: base(supportedLocales, logger)
	{
		_solutionFilePath = solutionFilePath;
	}

	public override void Initialize()
	{
		var projectFiles = GetProjectFilesFromSolution(_solutionFilePath);
		LoadProjectFiles(projectFiles);

		var localizationFiles = GetLoadedProjectFiles()
			.Select(TryGetLocalizationFilesFromProjectFile)
			.Where(files => files != null)
			.SelectMany(files => files);

		LoadLocalizationFiles(localizationFiles);

		base.Initialize();
	}

	private ICollection<string> GetLoadedProjectFiles()
	{
		return _projectFileWatchers.Keys;
	}

	private void LoadProjectFiles(IEnumerable<string> projectFiles)
	{
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
			watcher.Renamed += (s, ea) => HandleProjectFileChange(projectFile);

			if (_projectFileWatchers.TryAdd(projectFile, watcher))
			{
				watcher.EnableRaisingEvents = true;
			}
			else
			{
				watcher.Dispose();
			}
		}
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
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
			};

			watcher.Changed += (s, ea) => HandleLocalizationFileChange(localizationFile);
			watcher.Renamed += (s, ea) => HandleLocalizationFileChange(localizationFile);

			if (_localizationFileSystemWatchers.TryAdd(localizationFile, watcher))
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
		var localizationFilesFromProject = TryGetLocalizationFilesFromProjectFile(projectFile);
		if (localizationFilesFromProject == null)
			return;

		var localizationFilesToAdd = localizationFilesFromProject.Except(_localizationFileSystemWatchers.Keys);
		LoadLocalizationFiles(localizationFilesToAdd);
	}

	private IEnumerable<string> TryGetLocalizationFilesFromProjectFile(string projectFile)
	{
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
		var projectFileContent = TryGetProjectFileContentAsync(projectFile).Result;
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
		if (projectFileContent == null)
			return null;

		var document = XDocument.Parse(projectFileContent);

		// TODO: It would be nice to reuse this code with the TranslationChecker, now it's a copy-paste.
		return document.Descendants()
			.Where(x => x.Name.LocalName == "ItemGroup")
			.SelectMany(itemGroup => itemGroup.Descendants()
				.Where(y =>
					y.Attribute("Include")
						?.Value
						.EndsWith(TranslationFileSuffix, StringComparison.InvariantCultureIgnoreCase)
					?? false)
				.Select(node => node.Attribute("Include").Value)
				.SelectMany(path =>
					path.Contains("*") || path.Contains("*")
					? GetFilesFromWildcard(Path.GetDirectoryName(projectFile), path)
					: new[]{ Path.Combine(
						Path.GetDirectoryName(projectFile), path) }));
	}

	private static IEnumerable<string> GetFilesFromWildcard(string projectDirectory, string wildcard)
	{
		try
		{
			return Directory.GetFiles(projectDirectory,
				PathHelpers.ConvertToUnixPath(wildcard),
				searchOption: SearchOption.AllDirectories);
		}
		catch (DirectoryNotFoundException)
		{
			return Array.Empty<string>();
		}
	}

	private static async Task<string> TryGetProjectFileContentAsync(string projectFile, int tryCounter = 0)
	{
		if (tryCounter > 2)
			return null;

		try
		{
			return File.ReadAllText(projectFile);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);

			// Sometimes VS locks the project files so we can't read from them. Maybe we should wait for a bit so we can read?
			await Task.Delay(TimeSpan.FromMilliseconds(100));

			return await TryGetProjectFileContentAsync(projectFile, tryCounter + 1);
		}
	}

	private IEnumerable<string> GetProjectFilesFromSolution(string solutionFilePath)
	{
		var projectRelativePaths = SolutionFileParser.GetProjectRelativePaths(solutionFilePath);
		var solutionDirectory = Path.GetDirectoryName(solutionFilePath);
		if (solutionDirectory == null)
			throw new InvalidOperationException($"Couldn't get directory name from {solutionFilePath}");

		return projectRelativePaths
			.Select(PathHelpers.ConvertToUnixPath)
			.Select(projectRelativePath => Path.Combine(solutionDirectory, projectRelativePath));
	}

	protected override IEnumerable<string> GetTranslationFiles()
	{
		return _localizationFileSystemWatchers.Keys;
	}

	public void Dispose()
	{
		foreach (var localizationFileSystemWatcher in _localizationFileSystemWatchers.Values)
		{
			localizationFileSystemWatcher.EnableRaisingEvents = false;
			localizationFileSystemWatcher.Dispose();
		}

		foreach (var projectFileWatcher in _projectFileWatchers.Values)
		{
			projectFileWatcher.EnableRaisingEvents = false;
			projectFileWatcher.Dispose();
		}
	}
}