using Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class SplitJob
    {
        public static void Split(Component job)
        {
            ///////////////////// Temporär - gesamte Liste von DLLs usw. 
            Dictionary<Guid, Assembly> components = new Dictionary<Guid, Assembly>();
            //////////////////////

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

                //var outputs = edges.Where(x => x.InputComponentGuid == null);

                // get all input and output component guids.
                var edgesList = edges.ToList();
                List<Guid> inputGuids = new List<Guid>();
                List<Guid> outputGuids = new List<Guid>();

                foreach(var edge in edgesList)
                {
                    inputGuids.Add(edge.InternalOutputComponentGuid);
                    outputGuids.Add(edge.InternalInputComponentGuid);
                }

                foreach (var inputGuid in inputGuids)
                {
                    if (outputGuids.Contains(inputGuid))
                    {
                        outputGuids.Remove(inputGuid);
                        inputGuids.Remove(inputGuid);
                    }
                }

                foreach (var outputGuid in outputGuids)
                {
                    //var comp = components.First(x => x.Key == outputGuid).Value;
                    // var comp = components.First(x => x.Key == compGuid);

                    GoToNextEdge(edges, outputGuid, inputGuids, components);

                    var clientResultEdges = edges.Where(x => x.InternalInputComponentGuid == outputGuid).ToList();

                    // BERECHNUNG VOM USER ODER AUSGABE?? (wenn letzte component ausgabe ist)
                }

                // jetzt haben wir alle Ergebnisse abgearbeitet --> fertig!!!


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

        private static void GoToNextEdge(ICollection<ExtendedComponentEdge> edges, Guid outputGuid, List<Guid> inputGuids, Dictionary<Guid, Assembly> components)
        {
            var compInputEdges = edges.Where(x => x.InternalOutputComponentGuid == outputGuid).ToList();

            // schaun, ob diese nächste edge eine input edge is oder nicht
            foreach (var compInputEdge in compInputEdges)
            {
                if (compInputEdge.ComponentResult != null)
                {
                    Guid nextCompGuid = compInputEdge.InternalOutputComponentGuid;

                    if (!inputGuids.Contains(nextCompGuid))
                    {
                        // weiter gehen
                        GoToNextEdge(edges, nextCompGuid, inputGuids, components);

                        // Component an Client mit den ganzen Inputs schicken.
                        var inputEdges = edges.Where(x => x.InternalOutputComponentGuid == nextCompGuid).ToList();
                        List<object> inputData = new List<object>();

                        foreach (var inputEdge in compInputEdges)
                        {
                            inputData.Add(inputEdge.ComponentResult);
                        }

                        // input Data mittels JobRequest an client schicken.
                    }
                    else
                    {
                        // job aufgeben
                        var comp = components.First(x => x.Key == nextCompGuid).Value;
                        // Component an Client schicken und auf result warten.
                        // ...
                        // Result von client ist angekommen:
                        
                        
                    }

                    //compInputEdge.ComponentResult = clientJobResult.ToList();

                    // BERECHNUNG VOM USER
                    var clientResultEdges = edges.Where(x => x.InternalInputComponentGuid == nextCompGuid).ToList();

                    for (int i = 0; i < clientResultEdges.Count; i++)
                    {
                        //clientResultEdges[i].ComponentResult = clientJobResult[i];
                    }
                }
            }
        }
    }
}
