using EuroRepository.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace europarser.interfaces
{
    public interface ILogger
    {
        void Log(ErrorFrom from, IEnumerable<string> problems, Uri uri);
    }
}
