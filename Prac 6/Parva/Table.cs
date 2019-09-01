// Handle cross reference table for Parva
// P.D. Terry, Rhodes University, 2016

using Library;
using System.Collections.Generic;
using System.IO;

namespace Parva {

  class Entry {                      // Cross reference table entries
    public string name;              // The identifier itself
    public List<int> refs;           // Line numbers where it appears
    public Entry(string name) {
      this.name = name;
      this.refs = new List<int>();
    }
  } // Entry

  class Table {
    static List<Entry> list = new List<Entry>();

    public static void ClearTable() {
            list.Clear();
    } // Table.ClearTable

    public static void AddRef(string name, bool declared, int lineRef) {
            Entry AddingValues = new Entry(name);
            if (declared)
            {
                int stop;
                for (stop=0; stop<list.Count; stop++)
                {
                    string idenName = list[stop].name;
                    if (name == idenName)
                    {
                        AddingValues.refs[0] = -1 * lineRef;
                        list[stop] = (AddingValues);
                        break;
                    }
                }
                if (stop == list.Count - 1)
                {
                    AddingValues.refs.Add(-1 * lineRef);
                    list.Add(AddingValues);
                }
            }
            else
            {
                int stop;
                for (stop = 0; stop < list.Count; stop++)
                {
                    string idenName = list[stop].name;
                    if (name == idenName)
                    {
                       AddingValues.refs.Add(lineRef);
                        list[stop]=(AddingValues);
                        break;
                    }
                }
                if (stop == list.Count - 1)
                {
                    AddingValues.refs.Add(0);
                    AddingValues.refs.Add(lineRef);
                    list.Add(AddingValues);
                }
            }
    } // Table.AddRef

    public static void PrintTable() {
            // Prints out all references in the table (eliminate duplicates line numbers)
            //...
            string output = "";
        for(int stop =0; stop<list.Count; stop++)
            {
                Entry currIndex = list[stop];
                output += currIndex.name + "\t \t"; 
                for (int stop2=0; stop2< currIndex.refs.Count; stop2++)
                {
                    output += currIndex.refs[stop2] + "\t";
                }
                output += "\n";
            }
           StreamWriter outputFile = System.IO.File.CreateText("output.txt");
            outputFile.Write(output);
            outputFile.Close();
    } // Table.PrintTable

  } // Table

} // namespace
