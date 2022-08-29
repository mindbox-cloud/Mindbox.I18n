using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n.Analyzers;
public sealed class AnalyzerFileSystemTranslationSource : FileSystemTranslationSourceBase, IDisposable
{
	private const string ProjectFileSuffix = ".csproj";
	private readonly string _solutionFilePath;

	private FileSystemWatcher? _projectFileWatcher;
	private List<string> _projectFilePaths = null!;

	private FileSystemWatcher? _localizationFileSystemWatcher;
	private List<string> _localizationFilePaths = null!;
	private HashSet<string> _localizationFileNames = null!;

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
		_projectFilePaths = GetProjectFilesFromSolution(_solutionFilePath).ToList();
		LoadProjectFiles(_projectFilePaths);

		_localizationFilePaths = _projectFilePaths
			.AsParallel()
			.Select(TryGetLocalizationFilesFromProjectFile)
			.Where(files => files != null)
			.SelectMany(files => files)
			.Union(GetLocalizationFilesFromProjectTargetDirectories())
			.ToList();
		_localizationFileNames = new HashSet<string>(_localizationFilePaths.Select(Path.GetFileName));
		Console.WriteLine($"i18n: localization files: {string.Join(", ", _localizationFileNames)}");

		LoadLocalizationFiles(_localizationFilePaths);

		base.Initialize();
	}

	private ParallelQuery<string> GetLocalizationFilesFromProjectTargetDirectories()
	{
		return _projectFilePaths
			.AsParallel()
			.Select(path => Path.Combine(Path.GetDirectoryName(path), "bin"))
			.Where(Directory.Exists)
			.Select(path => Directory.GetFiles(path, $"*{TranslationFileSuffix}", SearchOption.AllDirectories))
			.SelectMany(files => files)
			.GroupBy(Path.GetFileName)
			.Select(x => x.First());
	}

	private static string? GetGreatestCommonFilePath(IEnumerable<string> paths)
	{
		var directoryPaths = paths
			.Select(Path.GetDirectoryName)
			.Distinct()
			.ToList();

		if (directoryPaths.Count == 0)
			return null;

		if (directoryPaths.Count == 1)
			return directoryPaths[0];

		var pathsSegments = directoryPaths.Select(path => path.Split(Path.DirectorySeparatorChar))
			.ToList();

		var minimalLength = pathsSegments.Min(x => x.Length);
		var samplePath = pathsSegments[0].Take(minimalLength).ToArray();
		for (var index = minimalLength - 1; index >= 0; index--)
		{
			if (pathsSegments.All(pathSegments => pathSegments[index] == samplePath[index]))
			{
				return string.Join(Path.DirectorySeparatorChar.ToString(), samplePath.Take(index + 1));
			}
		}

		return null;
	}

	private void LoadProjectFiles(ICollection<string> projectFiles)
	{
		Console.WriteLine($"i18n: registering {projectFiles.Count} project files");
		var greatestCommonPath = GetGreatestCommonFilePath(projectFiles);
		Console.WriteLine($"i18n: found greatest common path at {greatestCommonPath}");

		var watcher = new FileSystemWatcher
		{
			Path = greatestCommonPath,
			Filter = $"*{ProjectFileSuffix}",
			IncludeSubdirectories = true,
			NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
		};

		watcher.Changed += (_, eventArgs) => HandleProjectFileChange(eventArgs.FullPath);

		_projectFileWatcher = watcher;
		_projectFileWatcher.EnableRaisingEvents = true;
	}

	private void LoadLocalizationFiles(ICollection<string> localizationFiles)
	{
		Console.WriteLine($"i18n: registering {localizationFiles.Count} localization files");
		var greatestCommonPath = GetGreatestCommonFilePath(localizationFiles);
		Console.WriteLine($"i18n: found greatest common path at {greatestCommonPath}");

		var watcher = new FileSystemWatcher
		{
			Path = greatestCommonPath,
			Filter = $"*{TranslationFileSuffix}",
			IncludeSubdirectories = true,
			NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
		};

		watcher.Changed += (_, eventArgs) => HandleLocalizationFileChange(eventArgs.FullPath, eventArgs.ChangeType);

		_localizationFileSystemWatcher = watcher;
		_localizationFileSystemWatcher.EnableRaisingEvents = true;
	}

	private void HandleLocalizationFileChange(string localizationFile, WatcherChangeTypes changeTypes)
	{
		if (changeTypes.HasFlag(WatcherChangeTypes.Created))
		{
			var fileName = Path.GetFileName(localizationFile);
			// ReSharper disable once InconsistentlySynchronizedField - reading does not require locking
			if (_localizationFileNames.Contains(fileName))
			{
				return;
			}

			lock (_localizationFileNames)
			{
				_localizationFileNames.Add(fileName);
			}
		}

		Console.WriteLine($"i18n: handling change of localization file {localizationFile}");
		LoadTranslationFile(localizationFile);
	}

	private void HandleProjectFileChange(string projectFile)
	{
		Console.WriteLine($"i18n: handling change of project file {projectFile}");
		var localizationFilesFromProject = TryGetLocalizationFilesFromProjectFile(projectFile);
		if (localizationFilesFromProject == null)
			return;

		var localizationFilesToAdd = localizationFilesFromProject.Except(_localizationFilePaths).ToList();

		LoadLocalizationFiles(localizationFilesToAdd);
	}

	private static IEnumerable<string>? TryGetLocalizationFilesFromProjectFile(string projectFile)
	{
		var projectFileDirectory = Path.GetDirectoryName(projectFile);

		if (projectFileDirectory == null)
			return null;

		var projectFileContent = TryGetProjectFileContent(projectFile);
		if (projectFileContent == null)
			return null;

		var document = XDocument.Parse(projectFileContent);

		return document.XPathSelectElements("//ItemGroup//@Include/..")
			.Select(node => node.Attribute("Include")!.Value)
			.Where(include => include.EndsWith(TranslationFileSuffix, StringComparison.InvariantCultureIgnoreCase))
			.SelectMany(
				path => path.IndexOfAny(new[] { '?', '*' }) >= 0
					? GetFilesFromWildcard(projectFileDirectory, path)
					: new[] { Path.Combine(PathHelpers.ConvertToUnixPath(projectFileDirectory), path) }
			);
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

	private static string? TryGetProjectFileContent(string projectFile)
	{
		for (var tryCounter = 0; tryCounter < 3; tryCounter++)
		{
			try
			{
				return File.ReadAllText(projectFile);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);

				// Sometimes VS locks the project files so we can't read from them. Maybe we should wait for a bit so we can read?
				Thread.Sleep(100);
			}
		}

		return null;
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
		return _localizationFilePaths;
	}

	public void Dispose()
	{
		if (_projectFileWatcher != null)
		{
			_projectFileWatcher.EnableRaisingEvents = false;
			_projectFileWatcher.Dispose();
		}

		if (_localizationFileSystemWatcher != null)
		{
			_localizationFileSystemWatcher.EnableRaisingEvents = false;
			_localizationFileSystemWatcher.Dispose();
		}
	}
}