using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoNature.Components
{
    public interface ITranslatable
    {
        string TryTranslate(string translateKey);
        string TranslatedItselfName { get; }
    }
}
