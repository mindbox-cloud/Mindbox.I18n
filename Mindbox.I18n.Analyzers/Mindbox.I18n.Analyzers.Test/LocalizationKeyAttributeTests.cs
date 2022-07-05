using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Mindbox.I18n.Analyzers.Test;

[TestClass]
public class LocalizationKeyAttributeTests : MindboxI18nAnalyzerTests
{
	[TestMethod]
	public void KeyMustHaveCorrectFormat_AttributeConstructor_NamedArgument_WrongKeyFormatAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		[AttributeUsage(AttributeTargets.All)]
		public class TestAttribute : System.Attribute
		{
			public TestAttribute([LocalizationKey]string input)
			{
			}
		}

		[TestAttribute(input:""Кириллическая строка"")]
		public class TestingClass 
		{
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
			Message = BuildExpectedMessage("Кириллическая строка"),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 14, 24)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void KeyMustHaveCorrectFormat_AttributeProperty_WrongKeyFormatAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		[AttributeUsage(AttributeTargets.All)]
		public class TestAttribute : System.Attribute
		{
			[LocalizationKey]
			public string Input {get;set;}

			public TestAttribute()
			{
			}
		}

		[TestAttribute(Input = ""Кириллическая строка"")]
		public class TestingClass 
		{
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
			Message = BuildExpectedMessage("Кириллическая строка"),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 17, 26)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void KeyMustHaveCorrectFormat_AttributeField_WrongKeyFormatAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		[AttributeUsage(AttributeTargets.All)]
		public class TestAttribute : System.Attribute
		{
			[LocalizationKey]
			public string Input

			public TestAttribute()
			{
			}
		}

		[TestAttribute(Input = ""Кириллическая строка"")]
		public class TestingClass 
		{
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
			Message = BuildExpectedMessage("Кириллическая строка"),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 17, 26)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void KeyMustHaveCorrectFormat_AttributeField_CorrectKeyAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		[AttributeUsage(AttributeTargets.All)]
		public class TestAttribute : System.Attribute
		{
			[LocalizationKey]
			public string Input

			public TestAttribute()
			{
			}
		}

		[TestAttribute(Input = ""Namespace:Key"")]
		public class TestingClass 
		{
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		VerifyCSharpDiagnostic(test);
	}

	[TestMethod]
	public void KeyMustHaveCorrectFormat_AttributeConstructor_CorrectKeyAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		[AttributeUsage(AttributeTargets.All)]
		public class TestAttribute : System.Attribute
		{
			public TestAttribute([LocalizationKey]string input)
			{
			}
		}

		[TestAttribute(input:""Namespace:Key"")]
		public class TestingClass 
		{
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		VerifyCSharpDiagnostic(test);
	}

	[TestMethod]
	public void KeyMustHaveCorrectFormat_AttributeConstructor_WrongKeyFormatAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		[AttributeUsage(AttributeTargets.All)]
		public class TestAttribute : System.Attribute
		{
			public TestAttribute([LocalizationKey]string input)
			{
			}
		}

		[TestAttribute(""Кириллическая строка"")]
		public class TestingClass 
		{
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
			Message = BuildExpectedMessage("Кириллическая строка"),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 14, 18)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void KeyMustHaveCorrectFormat_FieldWithAttributeAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			[LocalizationKey]
			string field;
			
			TestingClass()
			{
				field = ""Кириллическая строка"";
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
			Message = BuildExpectedMessage("Кириллическая строка"),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 13, 13)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void KeyMustHaveCorrectFormat_PropertyWithAttributeAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			[LocalizationKey]
			string Field {get; set;}
			
			TestingClass()
			{
				Field = ""Кириллическая строка"";
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
			Message = BuildExpectedMessage("Кириллическая строка"),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 13, 13)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void KeyMustHaveCorrectFormat_PropertyWithAttribute_InitializerAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			[LocalizationKey]
			string Property {get; set;} = ""Кириллическая строка"";
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
			Message = BuildExpectedMessage("Кириллическая строка"),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 9, 34)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void KeyMustHaveCorrectFormat_FieldWithAttribute_InitializerAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			[LocalizationKey]
			string Field = ""Кириллическая строка"";
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
			Message = BuildExpectedMessage("Кириллическая строка"),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 9, 19)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void KeyMustHaveCorrectFormat_ArgumentWithAttributeAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		public class TestingClass 
		{			
			TestingClass([LocalizationKey]string field)
			{
			}

			TestingClass TestMethod() 
			{
				return new TestingClass(""Кириллическая строка"");
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var expected = new DiagnosticResult
		{
			Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
			Message = BuildExpectedMessage("Кириллическая строка"),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 14, 29)
				}
		};
		VerifyCSharpDiagnostic(test, expected);
	}

	[TestMethod]
	public void KeyMustHaveCorrectFormat_ArgumentWithAttribute_ComplexArgumentAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		public class TestingClass 
		{			
			TestingClass([LocalizationKey]string field)
			{
			}

			TestingClass TestMethod() 
			{
				return new TestingClass(true ? ""Кириллическая строка"" : ""12312"");
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var whenTrueBranchDiagnostic = new DiagnosticResult
		{
			Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
			Message = BuildExpectedMessage("Кириллическая строка"),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 14, 36),
				}
		};

		var whenFalseBranchDiagnostic = new DiagnosticResult
		{
			Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
			Message = BuildExpectedMessage("12312"),
			Severity = DiagnosticSeverity.Error,
			Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 14, 61),
				}
		};
		VerifyCSharpDiagnostic(test, whenTrueBranchDiagnostic, whenFalseBranchDiagnostic);
	}

	[TestMethod]
	public void KeyMustHaveCorrectFormat_AssignmentFromAttributedMember_NoDiagnosticsAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		public class TestingClass 
		{		

			[LocalizationKey]
			string Field {get; set;}
			
			TestingClass([LocalizationKey]string field)
			{
				Field = field;
			}
		}
    }";
#pragma warning restore Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		VerifyCSharpDiagnostic(test);
	}

	[TestMethod]
	public void KeyMustHaveCorrectFormat_AttributedMemberAsArgument_NoDiagnosticsAsync()
	{
#pragma warning disable Mindbox1002 // Отступы должны формироваться только с помощью табуляции
		var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		public class TestingClass 
		{		

			[LocalizationKey]
			string Field {get; set;}
			
			TestingClass([LocalizationKey]string field)
			{
				Field = field;
			}

			TestingClass Get()
			{
				return new TestingClass(Field);
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

	private string BuildExpectedMessage(string key)
	{
		return string.Format(Diagnostics.KeyMustHaveCorrectFormat.MessageFormat.ToString(), key);
	}
}