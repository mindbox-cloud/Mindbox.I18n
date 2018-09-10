using System;

namespace Mindbox.I18n.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
			// Incorrect key
			LocalizableString sss = "Abacaba";

			// Correct key, should be found
	        LocalizableString s = "Cars:Bntl";

			// Correct key, should be found
			LocalizableString s2 = "Bands:DeathCab";

			// Correct key but doesn't have a translation
			LocalizableString s3 = "Bands:SomeUnknownGuys"; 

			// Correct key but doesn't have a translation
			LocalizableString s4 = "Bands:UnknownAtTheBeginning";

			// Should be an error because the translation file is ignored by the configuration
	        LocalizableString coke = "Drinks:Coke";

			LocalizableString strange = $"Try {"that"}";
        }
    }
}
