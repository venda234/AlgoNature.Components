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
        private bool _translatableForThisCulture = true;
        public string TryTranslate(string translateKey)
        {
            if (_translatable)
            {
                string culture = _translatableForThisCulture ? Thread.CurrentThread.CurrentCulture.Name : Generals.DEFAULT_LOCALE_KEY;
                if (_translationDictionaries.Count == 0)
                {
                    if (_translatableForThisCulture/* && _translationDictionaries == null*/) // trying current culture if not previously restricted
                    {
                        _translatable = tryInitializeTranslationDictionary(culture);
                    }
                    if (_translationDictionaries.Count == 0) // trying default culture
                    {
                        _translatableForThisCulture = false;
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

            try
            {
                //ResourceManager resmgr = new ResourceManager(thisType.Namespace + ".resources", Assembly.GetExecutingAssembly());
                //var strs = new ResourceReader()
                //var strs = assembly.GetManifestResourceNames();
                using (Stream stream = assembly.GetManifestResourceStream(thisType.FullName + ".PropertiesToTranslate.resources"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    _translationDictionaries[locale] = new Dictionary<string, string>();

                    string line;
                    string[] splitLine;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("System.Resources.ResourceReader")) continue;
                        splitLine = line.Split(new char[6] { '=', '"', '\\', '\t', '\u0002', '\u0004' }); //cleaning firstrow mess
                        /*if (splitLine.Length >= 6) //cleaning firstrow mess
                        {
                            var _splln = new string[splitLine.Length == 7 ? 2 : 1];
                            _splln[0] = splitLine[5];
                            if (splitLine.Length > 6) _splln[1] = splitLine[6];
                            splitLine = _splln;
                        }
                        if (splitLine.Length <= 2)
                        {
                            // cleaning mess
                            if (splitLine[0].Contains('\u0002')) _translationDictionaries[locale].Add(splitLine[1], splitLine[2]);
                            else _translationDictionaries[locale].Add(splitLine[0], splitLine[1]);
                        }
                        else if (splitLine.Length == 1)
                        {
                            _translationDictionaries[locale].Add(splitLine[0], splitLine[0]);
                        }*/
                        if (splitLine[splitLine.Length - 2] == "") // empty
                            _translationDictionaries[locale].Add(splitLine[splitLine.Length - 4], splitLine[splitLine.Length - 4]);
                        else
                            _translationDictionaries[locale].Add(splitLine[splitLine.Length - 4], splitLine[splitLine.Length - 2]);
                    }
                }
                return true;
            }
            catch { return false; }
        }
    }
}
