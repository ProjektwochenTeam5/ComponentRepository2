using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommunication;

namespace Server
{
    public class JobResponseRecievedEventArgs : EventArgs
    {
        public JobResponseRecievedEventArgs(Guid belongToJob, TransferJobResponse data)
        {
            this.BelongsToJob = belongToJob;
            this.Data = data;
        }

        public Guid BelongsToJob { get; set; }

        public TransferJobResponse Data { get; set; }
    }
}
