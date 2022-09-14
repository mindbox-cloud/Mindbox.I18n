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

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Mindbox.I18n.Analyzers.Test;

[TestClass]
public class TranslationDiagnosticTests : MindboxI18nAnalyzerTests
{
	private TestTranslationSource _translationSource;

	[TestInitialize]
	public void TestInitialize()
	{
		_translationSource = new TestTranslationSource();

		_translationSource.AddTranslation(
			"Marvin:HumanHate",
			"Marvin was humming ironically because he hated humans so much.");
	}

	[TestMethod]
	public void TranslationAnalysis_TranslationExists_Info()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			public LocalizableString Name => ""Marvin:HumanHate"";
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.TranslationHint.Id,
			Message = "\"Marvin was humming ironically because he hated humans so much.\"",
			Severity = DiagnosticSeverity.Info,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 8, 37)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void TranslationAnalysis_TranslationNotFound_Error()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			public LocalizableString Name => ""Marvin:HumanLove"";
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.TranslationMustExistForLocalizationKey.Id,
			Message = "No translation found for key \"Marvin:HumanLove\"",
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 8, 37)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	protected override MindboxI18nAnalyzer CreateAnalyzer()
	{
		return new MindboxI18nAnalyzer(_translationSource);
	}
}