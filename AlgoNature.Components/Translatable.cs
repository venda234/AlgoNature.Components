using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO;

namespace AlgoNature.Components
{
    public abstract class Translatable : ITranslatable
    {
        // ITranslatable
        private bool _translatable = true;
        private bool _translatableForThsCulture = true;
        public string TryTranslate(string translateKey)
        {
            if (_translatable)
            {
                string culture = _translatableForThsCulture ? Thread.CurrentThread.CurrentCulture.Name : Generals.DEFAULT_LOCALE_KEY;
                if (_translationDictionaries.Count == 0)
                {
                    if (_translatableForThsCulture/* && _translationDictionaries == null*/) // trying current culture if not previously restricted
                    {
                        _translatable = tryInitializeTranslationDictionary(culture);
                    }
                    if (_translationDictionaries.Count == 0) // trying default culture
                    {
                        _translatableForThsCulture = false;
                        culture = Generals.DEFAULT_LOCALE_KEY;
                        _translatable = tryInitializeTranslationDictionary(culture);
                    }
                }

                /*else if (_translationDictionaries[culture] == null)
                {
                    if (!tryInitializeTranslationDictionary(culture))
                    {
                        _translatable = tryInitializeTranslationDictionary(culture);
                    }
                }*/
                if (_translatable)
                {
                    string res = _translationDictionaries[culture][translateKey];
                    return (res != null) ? res : translateKey;
                }
            }
            return translateKey;
        }
        private Dictionary<string, Dictionary<string, string>> _translationDictionaries = new Dictionary<string, Dictionary<string, string>>();
        private bool tryInitializeTranslationDictionary(string locale)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Type thisType = this.GetType();
            var resourceName = thisType.Name;
            if (resourceName.Contains(thisType.Namespace))
                resourceName.Remove(0, thisType.Namespace.Length + 1);
            resourceName += ".PropertiesToTranslate." + locale + ".txt";

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    _translationDictionaries[locale] = new Dictionary<string, string>();

                    string line;
                    string[] splitLine;
                    while ((line = reader.ReadLine()) != null)
                    {
                        splitLine = line.Split(new char[2] { '=', '"' }, StringSplitOptions.RemoveEmptyEntries);
                        if (splitLine.Length == 2)
                        {
                            _translationDictionaries[locale].Add(splitLine[0], splitLine[1]);
                        }
                        else if (splitLine.Length == 1)
                        {
                            _translationDictionaries[locale].Add(splitLine[0], splitLine[0]);
                        }
                    }
                }
                return true;
            }
            catch { return false; }
        }
    }
}
