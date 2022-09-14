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
public class OnlyStringLiteralsCanBeUsedAsKeysTests : MindboxI18nAnalyzerTests
{
	[TestMethod]
	public void OnlyStringLiteralsCanBeUsed_CorrectKey_NoDiagnostics()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = ""Namespace:Key_Key"";
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		VerifyCSharpDiagnostic(test);
	}

	[TestMethod]
	public void OnlyStringLiteralsCanBeUsed_CorrectKeyUsingConditionalExpression_NoDiagnostics()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = true ? ""Namespace:Key_Key1"" : ""Namespace:Key_Key2"";
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		VerifyCSharpDiagnostic(test);
	}

	[TestMethod]
	public void OnlyStringLiteralsCanBeUsed_StringInterpolation_Error()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = $""{DateTime.Now}"";
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
			Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 10, 27)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void OnlyStringLiteralsCanBeUsed_StringFormat_Error()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = string.Format(""{0}"", ""text"");
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
			Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 10, 27)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void OnlyStringLiteralsCanBeUsed_ExplicitConversion_Error()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;
    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				string key = ""text"";
				var str = (LocalizableString)key;
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
			Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 10, 34)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void OnlyStringLiteralsCanBeUsed_LocalStringVariable_Error()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;
    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				string key = ""text"";
				LocalizableString s = key;
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
			Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 10, 27)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void OnlyStringLiteralsCanBeUsed_MethodArgumentIsLocalizableString_Error()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod2(LocalizableString s) 
			{
				
			}

			void TestMethod() 
			{
				string key = ""text"";
				TestMethod2(key);
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
			Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 16, 17)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void OnlyStringLiteralsCanBeUsed_StringMemberAccess_Error()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = TestingClass.Key;
			}

			private const string Key = ""text"";
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
			Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 10, 27)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void OnlyStringLiteralsCanBeUsed_StringConditionalMemberAccess_Error()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				(string, string)? tuple = (""a"", ""b"");
				LocalizableString s = tuple?.Item1;
			}

			private object object;
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
			Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 11, 27)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void OnlyStringLiteralsCanBeUsed_StringConcatenation_Error()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = ""pupa"" + ""lupa"";
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
			Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 10, 27)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void OnlyStringLiteralsCanBeUsed_LocalizableStringAssignedToLocalizableString_NoDiagnostics()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{				
				LocalizableString s = ""Namespace:Key_Key"";
				LocalizableString s2 = s;
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		VerifyCSharpDiagnostic(test);
	}

	protected override MindboxI18nAnalyzer CreateAnalyzer()
	{
		return new MindboxI18nAnalyzer();
	}
}