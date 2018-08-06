using System;
using System.Collections.Generic;

namespace Mindbox.i18next
{
    public class I18Next
    {
        private Options options;
        private LocaleData localeData;

        public void Init(Options options)
        {
            this.options = options;
            localeData = new LocaleData(options.LanguageDirectoryPath);
        }

        public string Translate(string key) => Translate(key, new TranslationOptions());

        public string Translate(string key, TranslationOptions translationOptions)
        {
            var splitKey = key.Split(':');
            return localeData.GetKeyValue(splitKey[0], splitKey[1]);
        }
    }
}