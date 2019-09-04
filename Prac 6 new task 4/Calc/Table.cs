// Handle cross reference table for Parva
// P.D. Terry, Rhodes University, 2016

using Library;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Calc {

  class Entry {                                             // Class for symbol record
    public string name;                                     // Name of the symbol 
    public bool status;                                     // status of the status 
    public int value;               // values of the symbol 
    public Entry(string name, bool status, int value) {
      this.name = name;
      this.status = status;
      this.value = value;
    }
  } // Entry

  class Table {
    static List<Entry> list = new List<Entry>();

    public static void ClearTable() {
            list.Clear();
    } // Table.ClearTable

    public static void AddRef(string name, bool status, int value) {
        int stop = 0;
        for(stop = 0; stop< list.Count; stop++){
            Entry symbol = list[stop];
            if(name == symbol.name){
                symbol.value = value;
                list[stop]= symbol;
            }
        }
        if( stop== list.Count){
            Entry symbol = new Entry(name,status,value);
            list.Add(symbol);
        }
    } // Table.AddRef
    
    public static int Retrieve (string name)
    {
        for(int stop = 0; stop<list.Count; stop++){
            Entry indexInfo = list[stop];
            if(name == indexInfo.name){
                return indexInfo.value;
            }
        }
        return(0);
    }// retrieve the value of the given symbol

    
    public static void PrintTable() {
            // Prints out all references in the table (eliminate duplicates line numbers)
            //...
            string output = "";
        for(int stop =0; stop<list.Count; stop++)
            {
                Entry currIndex = list[stop];
                output += currIndex.name + " " + currIndex.value+ "\t \t";                 
                output += "\n";
            }
           StreamWriter outputFile = System.IO.File.CreateText("outputCalc.txt");
            outputFile.Write(output);
            outputFile.Close();
    } // Table.PrintTable
     
  } // Table

} // namespace
