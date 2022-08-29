namespace Mindbox.I18n.Sandbox;

internal class Program
{
	private static void Main(string[] _1)
	{
		// Incorrect key
		_ = LocalizableString.ForKey("Abacaba");

		// Correct key, should be found
		_ = LocalizableString.ForKey("Cars:Bntl");

		// Correct key, should be found
		_ = LocalizableString.ForKey("Bands:DeathCab");

		// Correct key but doesn't have a translation
		_ = LocalizableString.ForKey("Bands:SomeUnknownGuys");

		// Correct key but doesn't have a translation
		_ = LocalizableString.ForKey("Bands:UnknownAtTheBeginning");

		// Should be an error because the translation file is ignored by the configuration
		_ = LocalizableString.ForKey("Drinks:Coke");
		_ = LocalizableString.ForKey($"Try {"that"}");
	}
}