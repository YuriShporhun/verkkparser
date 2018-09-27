using europarser.global.statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace europarser.models
{
    public class UploaderState
    {
        UploaderState() { }
        object statusSync = new object();

        public string uploaderState { get; private set; }
        public UploaderStatus Status { get; private set; } = UploaderStatus.Ready;
        static UploaderState instance = new UploaderState();
        public static UploaderState Instance => instance;

        public bool TryStart()
        {
            lock(statusSync)
            {
                if(Status == UploaderStatus.Ready || Status == UploaderStatus.Done)
                {
                    Status = UploaderStatus.Started;
                    return true;
                }
                return false;
            }
        }

        public void Reset()
        {
            Status = UploaderStatus.Ready;
        }

        public void Success()
        {
            Status = UploaderStatus.Done;
        }

        public void Update()
        {
            Status = UploaderStatus.Update;
        }

        public void Excel()
        {
            Status = UploaderStatus.Excel;
        }

        public void Download()
        {
            Status = UploaderStatus.Download;
        }
    }
}
