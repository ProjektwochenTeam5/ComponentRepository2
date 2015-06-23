using Core.Network;
using System;
using System.Collections.Generic;

namespace UserInterface
{
    public class Job
    {
        /// <summary>
        /// The internal GUIDs of the input components along with their input descriptions.
        /// </summary>
        public Dictionary<Guid, string> InputDescriptions
        {
            get;
            set;
        }

        public Component JobComponent
        {
            get;
            set;
        }
    }
}
