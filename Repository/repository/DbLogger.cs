using europarser.interfaces;
using EuroRepository.models;
using System;
using System.Collections.Generic;

namespace europarser.repository
{
    public class DbLogger : ILogger
    {
        DbLogger() { }
        static ILogger instance = new DbLogger();

        public static ILogger GetInstance() => instance;

        public void Log(ErrorFrom from, IEnumerable<string> problems, Uri uri)
        {
            
        }
    }
}
