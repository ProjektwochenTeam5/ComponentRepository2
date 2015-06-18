using Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class SplitJob
    {
        public static void Split(Component job)
        {
            if (job.IsAtomic)
            {
                // an client schiken (Auftrag)
            }
            else
            {
                ICollection<ExtendedComponentEdge> edges = new List<ExtendedComponentEdge>();
                foreach (var item in job.Edges)
                {
                    edges.Add(new ExtendedComponentEdge(item));
                }

                var outputs = edges.Where(x => x.InputComponentGuid == null);
                foreach (var output in outputs)
                {
                    var compGuid = output.OutputComponentGuid;

                }
                // Job aufteilen

                // alle outputs finden (edges ohne InputComponentGUID)
                // 1. output -> Komponente selbst suchen
                    // von dieser Komponenete die Inputs nehmen
                        // von diesen Inputs die Edges suchen, bis Edge gefunden wurde, die keine OutputComponentGUID hat
                        // Job verschicken an Agent für Input
                            // Warten bis Input (Eingabe) da ist
                            
                        // Job verschicken an Agent......
                    // Komponente berechnen lassen
                    // Ergebnis speichern
                // 2. output ........

            }
        }
    }
}
