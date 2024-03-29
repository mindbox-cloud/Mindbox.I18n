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
using Microsoft.CodeAnalysis;

namespace TestHelper;

/// <summary>
/// Location where the diagnostic appears, as determined by path, line number, and column number.
/// </summary>
public readonly struct DiagnosticResultLocation
{
	public DiagnosticResultLocation(string path, int line, int column)
	{
		if (line < -1)
		{
			throw new ArgumentOutOfRangeException(nameof(line), "line must be >= -1");
		}

		if (column < -1)
		{
			throw new ArgumentOutOfRangeException(nameof(column), "column must be >= -1");
		}

		Path = path;
		Line = line;
		Column = column;
	}

	public string Path { get; }
	public int Line { get; }
	public int Column { get; }
}

/// <summary>
/// Struct that stores information about a Diagnostic appearing in a source
/// </summary>
public struct DiagnosticResult
{
	private DiagnosticResultLocation[] _locations;

	public DiagnosticResultLocation[] Locations
	{
		get
		{
			if (_locations == null)
			{
				_locations = new DiagnosticResultLocation[] { };
			}
			return _locations;
		}

		set
		{
			_locations = value;
		}
	}

	public DiagnosticSeverity Severity { get; set; }

	public string Id { get; set; }

	public string Message { get; set; }

	public string Path
	{
		get
		{
			return Locations.Length > 0 ? Locations[0].Path : "";
		}
	}

	public int Line
	{
		get
		{
			return Locations.Length > 0 ? Locations[0].Line : -1;
		}
	}

	public int Column
	{
		get
		{
			return Locations.Length > 0 ? Locations[0].Column : -1;
		}
	}
}