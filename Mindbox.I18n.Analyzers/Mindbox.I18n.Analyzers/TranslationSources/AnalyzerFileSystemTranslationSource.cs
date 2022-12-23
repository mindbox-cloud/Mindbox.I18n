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
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n.Analyzers;
#nullable disable
public sealed class AnalyzerFileSystemTranslationSource : FileSystemTranslationSourceBase, IDisposable
{
	private readonly string _solutionFilePath;

	private readonly HashSet<string> _localizationFiles = new();
	private FileSystemWatcher _translationFileChangeWatcher;

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

		var solutionDirectory = Path.GetDirectoryName(_solutionFilePath);

		TryCreateTranslationFileChangeWatcher(solutionDirectory);

		var localizationFiles = projectFiles
			.Select(TryGetLocalizationFilesFromProjectFile)
			.Where(files => files != null)
			.SelectMany(files => files)
			.ToList();

		foreach (var file in localizationFiles)
			_localizationFiles.Add(file);

		base.Initialize();
	}

	private void TryCreateTranslationFileChangeWatcher(string solutionDirectory)
	{
		try
		{
			_translationFileChangeWatcher = new FileSystemWatcher
			{
				Path = solutionDirectory,
				Filter = $"*.{TranslationFileSuffix}",
				IncludeSubdirectories = true,
				NotifyFilter = NotifyFilters.LastWrite
			};

			_translationFileChangeWatcher.Changed += (_, ea) => HandleLocalizationFileChange(ea.FullPath);
			_translationFileChangeWatcher.EnableRaisingEvents = true;
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Error creating file system watcher at {Path}", solutionDirectory);
		}
	}

	private void HandleLocalizationFileChange(string localizationFile)
	{
		Logger.LogInformation("Handling localization file change: {Path}", localizationFile);
		LoadTranslationFile(localizationFile);
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
		var localizationFiles = document.Descendants()
			.Where(x => x.Name.LocalName == "ItemGroup")
			.SelectMany(itemGroup => itemGroup.Descendants()
				.Where(y =>
					y.Attribute("Include")
						?.Value
						.EndsWith(TranslationFileSuffix, StringComparison.InvariantCultureIgnoreCase)
					?? false)
				.Select(node => node.Attribute("Include").Value)
				.SelectMany(path =>
					path.Contains("?") || path.Contains("*")
					? GetFilesFromWildcard(Path.GetDirectoryName(projectFile), path)
					: new[]{ Path.Combine(
						Path.GetDirectoryName(projectFile), path) }))
				.ToList();

		Logger.LogInformation("Found translation files:");
		foreach (var file in localizationFiles)
			Logger.LogInformation($"- {file}");

		return localizationFiles;
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

	private IReadOnlyList<string> GetProjectFilesFromSolution(string solutionFilePath)
	{
		var projectRelativePaths = SolutionFileParser.GetProjectRelativePaths(solutionFilePath);
		var solutionDirectory = Path.GetDirectoryName(solutionFilePath);
		if (solutionDirectory == null)
			throw new InvalidOperationException($"Couldn't get directory name from {solutionFilePath}");

		return projectRelativePaths
			.Select(PathHelpers.ConvertToUnixPath)
			.Select(projectRelativePath => Path.Combine(solutionDirectory, projectRelativePath))
			.Distinct()
			.ToList();
	}

	protected override IEnumerable<string> GetTranslationFiles() => _localizationFiles;

	public void Dispose()
	{
		_translationFileChangeWatcher?.Dispose();
	}
}