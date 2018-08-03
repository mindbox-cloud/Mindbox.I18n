using System;
using System.Collections.Generic;

namespace Mindbox.i18next
{
    public class I18Next
    {
        private InitOptions initOptions;
        private LocaleData localeData;

        public void Init(InitOptions initOptions)
        {
            this.initOptions = initOptions;
            localeData = new LocaleData(initOptions.LocalePath);
        }

        public string Translate(string key, Options options)
        {
            var splitKey = key.Split(':');
            return localeData.GetKeyValue(splitKey[0], splitKey[1]);
        }
    }
}