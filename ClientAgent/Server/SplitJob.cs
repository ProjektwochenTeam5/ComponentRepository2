using ClientServerCommunication;
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
        private static List<ExtendedComponentEdge> edges;

        public static void Split(DoJobRequest jobreq)
        {
            var job = jobreq.Job;

            //var addGuid = Guid.NewGuid();
            //var inpGuid = Guid.NewGuid();
            //var outGuid = Guid.NewGuid();

            //var intaddGuid1 = Guid.NewGuid();
            //var intaddGuid2 = Guid.NewGuid();
            //var intinpGuid1 = Guid.NewGuid();
            //var intinpGuid2 = Guid.NewGuid();
            //var intinpGuid3 = Guid.NewGuid();
            //var intoutGuid = Guid.NewGuid();

            ////////////////// Testjob
            //Component job = new Component();
            //job.ComponentGuid = new Guid();
            //job.FriendlyName = "Addieren von 3 Zahlen";
            //job.InputDescriptions = new List<string>() {"zahl1", "zahl2", "zahl3"};
            //job.OutputDescriptions = new List<string>() { "zahl" };
            //job.InputHints = new List<string>() { typeof(int).ToString(), typeof(int).ToString(), typeof(int).ToString() };
            //job.IsAtomic = false;
            //job.Edges = new List<ComponentEdge>();

            //var edge1 = new ComponentEdge();
            //edge1.InputComponentGuid = addGuid;
            //edge1.OutputComponentGuid = inpGuid;
            //edge1.InternalInputComponentGuid = intaddGuid1;
            //edge1.InternalOutputComponentGuid = intinpGuid1;
            //edge1.InputValueID = 1;
            //edge1.OutputValueID = 1;

            //var edge2 = new ComponentEdge();
            //edge2.InputComponentGuid = addGuid;
            //edge2.OutputComponentGuid = inpGuid;
            //edge2.InternalInputComponentGuid = intaddGuid1;
            //edge2.InternalOutputComponentGuid = intinpGuid2;
            //edge2.InputValueID = 2;
            //edge2.OutputValueID = 1;

            //var edge3 = new ComponentEdge();
            //edge3.InputComponentGuid = addGuid;
            //edge3.OutputComponentGuid = addGuid;
            //edge3.InternalInputComponentGuid = intaddGuid2;
            //edge3.InternalOutputComponentGuid = intaddGuid1;
            //edge3.InputValueID = 1;
            //edge3.OutputValueID = 1;

            //var edge4 = new ComponentEdge();
            //edge4.InputComponentGuid = addGuid;
            //edge4.OutputComponentGuid = inpGuid;
            //edge4.InternalInputComponentGuid = intaddGuid2;
            //edge4.InternalOutputComponentGuid = intinpGuid3;
            //edge4.InputValueID = 2;
            //edge4.OutputValueID = 1;

            //var edge5 = new ComponentEdge();
            //edge5.InputComponentGuid = outGuid;
            //edge5.OutputComponentGuid = addGuid;
            //edge5.InternalInputComponentGuid = intoutGuid;
            //edge5.InternalOutputComponentGuid = intaddGuid2;
            //edge5.InputValueID = 1;
            //edge5.OutputValueID = 1;

            //List<ComponentEdge> myedges = new List<ComponentEdge>();

            //myedges.Add(edge1);
            //myedges.Add(edge2);
            //myedges.Add(edge3);
            //myedges.Add(edge4);
            //myedges.Add(edge5);
            //job.Edges = myedges.AsEnumerable();

            

            ///////////////////// Temporär - gesamte Liste von DLLs usw. 
            Dictionary<Guid, Assembly> components = new Dictionary<Guid, Assembly>();
            //////////////////////

            if (job.IsAtomic)
            {
                // an client schiken (Auftrag)

            }
            else
            {
                edges = new List<ExtendedComponentEdge>();
                foreach (var item in job.Edges)
                {
                    edges.Add(new ExtendedComponentEdge(item));
                }

                //var outputs = edges.Where(x => x.InputComponentGuid == null);

                // get all input and output component guids.
                var edgesList = edges.ToList();
                List<Guid> allInputGuids = new List<Guid>();
                List<Guid> allOutputGuids = new List<Guid>();

                List<Guid> inputGuids = new List<Guid>();
                List<Guid> outputGuids = new List<Guid>();

                foreach(var edge in edgesList)
                {
                    allInputGuids.Add(edge.InternalOutputComponentGuid);
                    allOutputGuids.Add(edge.InternalInputComponentGuid);
                    inputGuids.Add(edge.InternalOutputComponentGuid);
                    outputGuids.Add(edge.InternalInputComponentGuid);
                }

                foreach (var inputGuid in allInputGuids)
                {
                    if (allOutputGuids.Contains(inputGuid))
                    {
                        inputGuids.Remove(inputGuid);
                    }
                }

                foreach (var outputGuid in allOutputGuids)
                {
                    if (allInputGuids.Contains(outputGuid))
                    {
                        outputGuids.Remove(outputGuid);
                    }
                }

                foreach (var outputGuid in outputGuids)
                {
                    //var comp = components.First(x => x.Key == outputGuid).Value;
                    // var comp = components.First(x => x.Key == compGuid);

                    GoToNextEdge(outputGuid, inputGuids, components);

                    // BERECHNUNG VOM USER ODER AUSGABE?? (wenn letzte component ausgabe ist)
                    foreach (var resultEdge in edges.Where(x => x.InternalInputComponentGuid == outputGuid))
                    {
                        Console.WriteLine("Ergebnis: " + ((int)resultEdge.ComponentResult).ToString());
                    }
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

        private static int Add(IEnumerable<object> param)
        {
            int result = 0;

            foreach (var item in param)
            {
                result += (int)item;
            }

            return result;
        }

        private static void GoToNextEdge(Guid outputGuid, List<Guid> inputGuids, Dictionary<Guid, Assembly> components)
        {
            //var compInputEdges = edges.Where(x => x.InternalInputComponentGuid == outputGuid);

            // schaun, ob diese nächste edge eine input edge is oder nicht
            foreach (var compInputEdge in edges.Where(x => x.InternalInputComponentGuid == outputGuid))
            {
                if (compInputEdge.ComponentResult == null)
                {
                    Guid nextCompGuid = compInputEdge.InternalOutputComponentGuid;

                    if (!inputGuids.Contains(nextCompGuid))
                    {
                        // weiter gehen
                        GoToNextEdge(nextCompGuid, inputGuids, components);

                        // Component an Client mit den ganzen Inputs schicken.
                        // var inputEdges = edges.Where(x => x.InternalInputComponentGuid == nextCompGuid);
                        List<object> inputData = new List<object>();

                        foreach (var inputEdge in edges.Where(x => x.InternalInputComponentGuid == nextCompGuid))
                        {
                            inputData.Add(inputEdge.ComponentResult);
                        }

                        // input Data mittels JobRequest an client schicken.

                        /////////// TEST METHODENAUFRUF
                        int result = Add(inputData);


                        //var outputEdges = edges.Where(x => x.InternalInputComponentGuid == nextCompGuid);
                        // TODO: result-IEnumerable durchgehen und results in outputEdges schreiben!

                        foreach (var outputEdge in edges.Where(x => x.InternalOutputComponentGuid == nextCompGuid))
                        {
                            ////////// TEST ZUWEISUNG
                            outputEdge.ComponentResult = result;
                        }
                    }
                    else
                    {
                        // job aufgeben
                        //var comp = components.First(x => x.Key == nextCompGuid).Value;
                        // Component an Client schicken und auf result warten.
                        // ...
                        // Result von client ist angekommen:
                        //clientResultEdge == compInputEdge
                        //var clientResultEdges = edges.Where(x => x.InternalOutputComponentGuid == nextCompGuid);
                        
                        
                        /////////////////// TEST INPUT
                        foreach (var resultEdge in edges.Where(x => x.InternalOutputComponentGuid == nextCompGuid))
                        {
                            resultEdge.ComponentResult = 5;
                        }
                    }

                    //compInputEdge.ComponentResult = clientJobResult.ToList();

                    /* TODO: BERECHNUNG VOM USER
                    var clientResultEdges = edges.Where(x => x.InternalInputComponentGuid == nextCompGuid);

                    for (int i = 0; i < clientResultEdges.Count; i++)
                    {
                        //clientResultEdges[i].ComponentResult = clientJobResult[i];
                    }*/
                }
            }
        }
    }
}
