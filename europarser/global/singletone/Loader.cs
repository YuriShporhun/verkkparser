using DataTransferObjects;
using europarser.global.statuses;
using europarser.repository;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace europarser.global.singletone
{
    public class Loader
    {
        Loader() { }
        static Loader instance = new Loader();
        public static Loader Instance => instance;

        LoaderStatus status = LoaderStatus.Ready;
        public LoaderStatus Status => status;

        public string Progress { get; private set; } = string.Empty;

        public void ResetState()
        {
            status = LoaderStatus.Ready;
            Progress = string.Empty;
        }

        public void Run()
        {
            if (Status != LoaderStatus.Ready)
                return;

            List<UmiItem> euroMadeData = null;

            status = LoaderStatus.Download;

            try
            {
                euroMadeData = Repository.EuroMade.Download((state) => Progress = state);
            }
            catch(MySqlException e)
            {
                status = LoaderStatus.Disconnected;
                return;
            }

            status = LoaderStatus.Upload;
            Repository.EuroMade.Upload(euroMadeData, (state) => Progress = state);

            status = LoaderStatus.Done;
        }
    }
}
