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

using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis.Diagnostics;
using Mindbox.I18n.Abstractions;
using TestHelper;

namespace Mindbox.I18n.Analyzers.Test;

public abstract class MindboxI18nAnalyzerTests : DiagnosticVerifier
{
	protected override IEnumerable<Assembly> GetAdditionalAssemblies()
	{
		yield return typeof(LocalizableString).Assembly;
		yield return typeof(LocalizationKey).Assembly;
	}

	protected sealed override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
	{
		return CreateAnalyzer();
	}

	protected abstract MindboxI18nAnalyzer CreateAnalyzer();
}