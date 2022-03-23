using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Mindbox.I18n.Analyzers.Test
{
	[TestClass]
	public class LocalizationKeyAttributeTests : MindboxI18nAnalyzerTests
	{
		[TestMethod]
		public void KeyMustHaveCorrectFormat_AttributeConstructor_NamedArgument_WrongKeyFormat()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		public class TestingClass 
		{
			private void TrackedEntitySetState()
			{
				LocalizableString.LocaleIndependent(""ex.Message"");
			}
		}
    }";
			
			var test2 = @"
	using System;

using Mindbox.I18n;

namespace ConsoleApplication1
{
	public class TestingClass 
	{
		private void TrackedEntitySetState()
		{
			try
			{
				throw new ArgumentException(""Message"");
		}
		catch (Exception ex)
		{
			var name = LocalizableString.LocaleIndependent(ex.Message);
		}
	}
}
}";
			
			
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.OnlyStringsCanBeUsedInLocaleIndependent.Id,
				Message = BuildExpectedMessage("Кириллическая строка"),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 14, 24)
					}
			};

			VerifyCSharpDiagnostic(test2, expected);
		}

		[TestMethod]
		public void KeyMustHaveCorrectFormat_AttributeProperty_WrongKeyFormat()
		{
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
		public void KeyMustHaveCorrectFormat_AttributeField_WrongKeyFormat()
		{
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
		public void KeyMustHaveCorrectFormat_AttributeField_CorrectKey()
		{
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
			VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void KeyMustHaveCorrectFormat_AttributeConstructor_CorrectKey()
		{
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

			VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void KeyMustHaveCorrectFormat_AttributeConstructor_WrongKeyFormat()
		{
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
		public void KeyMustHaveCorrectFormat_FieldWithAttribute()
		{
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
		public void KeyMustHaveCorrectFormat_PropertyWithAttribute()
		{
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
		public void KeyMustHaveCorrectFormat_PropertyWithAttribute_Initializer()
		{
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
		public void KeyMustHaveCorrectFormat_FieldWithAttribute_Initializer()
		{
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
		public void KeyMustHaveCorrectFormat_ArgumentWithAttribute()
		{
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
		public void KeyMustHaveCorrectFormat_ArgumentWithAttribute_ComplexArgument()
		{
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
		public void KeyMustHaveCorrectFormat_AssignmentFromAttributedMember_NoDiagnostics()
		{
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
			VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void KeyMustHaveCorrectFormat_AttributedMemberAsArgument_NoDiagnostics()
		{
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
}