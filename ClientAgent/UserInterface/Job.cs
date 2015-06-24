using Core.Network;
using System.Collections.Generic;

namespace UserInterface
{
    public class Job
    {
        /// <summary>
        /// The input identifiers of the input components along with their input descriptions.
        /// </summary>
        public Dictionary<InputIdentifier, string> InputDescriptions
        {
            get;
            set;
        }

        /// <summary>
        /// The component which represents the job.
        /// </summary>
        public Component JobComponent
        {
            get;
            set;
        }
    }
}
