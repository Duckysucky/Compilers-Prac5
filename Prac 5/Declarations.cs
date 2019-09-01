  // Do learn to insert your names and a brief description of what the program is supposed to do!

  // This is a skeleton program for developing a parser for Modula-2 declarations
  // P.D. Terry, Rhodes University
  //g16V4332, g16M4204, g16T3531, g16N3498
  using Library;
  using System;
  using System.Text;

  class Token {
    public int kind;
    public string val;

    public Token(int kind, string val) {
      this.kind = kind;
      this.val = val;

    }

  } // Token

  class Declarations {

    // +++++++++++++++++++++++++ File Handling and Error handlers ++++++++++++++++++++

    static InFile input;
    static OutFile output;

    static string NewFileName(string oldFileName, string ext) {
    // Creates new file name by changing extension of oldFileName to ext
      int i = oldFileName.LastIndexOf('.');
      if (i < 0) return oldFileName + ext; else return oldFileName.Substring(0, i) + ext;
    } // NewFileName

    static void ReportError(string errorMessage) {
    // Displays errorMessage on standard output and on reflected output
      Console.WriteLine(errorMessage);
      output.WriteLine(errorMessage);
    } // ReportError

    static void Abort(string errorMessage) {
    // Abandons parsing after issuing error message
      ReportError(errorMessage);
      output.Close();
      System.Environment.Exit(1);
    } // Abort

    // +++++++++++++++++++++++  token kinds enumeration +++++++++++++++++++++++++

    const int
      noSym        =  0,
      EOFSym       =  1,
      typeSym      =  2,
      semicolSym   =  3,
      varSym       =  4,
      stopSym      =  5,
      commaSym     =  6,
      coloSym      =  7,
      LsquabraSym  =  8,
      RsquabraSym  =  9,
      equSym       = 10,
      RbraSym      = 11,
      LbraSym      = 12,  
      doublestopSym= 13,
      arraySym     = 14,
      ofSym        = 15,
      recordSym    = 16,
      endSym       = 17,
      setSym       = 18,
      pointerSym   = 19,
      toSym        = 20,
      indenSym     = 21,
      numSym       = 22,
      commentSym   = 23;

      // and others like this

    // +++++++++++++++++++++++++++++ Character Handler ++++++++++++++++++++++++++

    const char EOF = '\0';
    static bool atEndOfFile = false;

    // Declaring ch as a global variable is done for expediency - global variables
    // are not always a good thing

    static char ch;    // look ahead character for scanner

    static void GetChar() {
    // Obtains next character ch from input, or CHR(0) if EOF reached
    // Reflect ch to output
      if (atEndOfFile) ch = EOF;
      else {
        ch = input.ReadChar();
        atEndOfFile = ch == EOF;
        if (!atEndOfFile) output.Write(ch);
      }
    } // GetChar

    // +++++++++++++++++++++++++++++++ Scanner ++++++++++++++++++++++++++++++++++

    // Declaring sym as a global variable is done for expediency - global variables
    // are not always a good thing

    static Token sym;

    static void GetSym() {
    // Scans for next sym from input
      while (ch > EOF && ch <= ' ') GetChar();
      StringBuilder symLex = new StringBuilder();
      int symKind = noSym;

      // over to you!

      // check numbers first
      if(Char.IsDigit(ch)){
        while(Char.IsDigit(ch))
        {
          symLex.Append(ch);
          GetChar();
        }
        symKind = numSym;
      }
      else if((Char.IsLetter(ch)))
      {
        while((Char.IsDigit(ch)) || (Char.IsLetter(ch)))
        {
          symLex.Append(ch);
          switch(symLex.ToString()){
            case ("TYPE"):
                GetChar();
                if( ch=='\n' || ch==' ')
                {
                  symKind = typeSym;
                  break;
                }
                else if(Char.IsLetter(ch))
                {
                  symLex.Append(ch);
                }
                else{
                  symKind = typeSym;
                }
                break;
            case ("VAR"):
                 GetChar();
                 if( ch=='\n' || ch==' ')
                {
                  symKind=varSym;
                  break;
                }
                else if(Char.IsLetter(ch))
                {
                  symLex.Append(ch);
                }
                else{
                  symKind = varSym ;
                }
                break;
            case ("ARRAY"):
                GetChar();
                if( ch=='\n' || ch==' ')
                {
                  symKind = arraySym;
                  break;
                }
                else if(Char.IsLetter(ch))
                {
                  symLex.Append(ch);
                }
                else{
                  symKind = arraySym ;
                }
                break;
            case ("RECORD"):
               GetChar();
               if( ch=='\n' || ch==' ')
               {
                 symKind = recordSym;
                 break;
               }
                else if(Char.IsLetter(ch))
                {
                  symLex.Append(ch);
                }
                else{
                  symKind = recordSym ;
                }
                break;
            case ("END"):
                GetChar();
                if( ch=='\n' || ch==' ')
                {
                  symKind = endSym;
                  break;
                }
                else if(Char.IsLetter(ch))
                {
                  symLex.Append(ch);
                }
                else{
                  symKind = endSym ;
                }
                break;
            case ("SET"):
                GetChar();
                if( ch=='\n' || ch==' ')
                {
                  symKind = setSym;
                  break;
                }
                else if(Char.IsLetter(ch))
                {
                  symLex.Append(ch);                  
                }
                else{
                  symKind = setSym ;
                }
                break;
            case ("OF"):
               GetChar();
               if( ch=='\n' || ch==' ')
               {
                  symKind = ofSym ;
                  break;                 
               }
                else if(Char.IsLetter(ch) || (Char.IsLetter(ch)))
                {
                  symLex.Append(ch);
                }
                else{
                  symKind = ofSym ;
                }
                break;
            case ("POINTER"):
                GetChar();
                if( ch=='\n' || ch==' ')
                {
                  symKind = pointerSym;
                  break;
                }
                else if((Char.IsDigit(ch)) || (Char.IsLetter(ch)) )
                {
                    symLex.Append(ch);
                }
                else{
                  symKind = pointerSym ;
                }
                break;
            case ("TO"):
                GetChar();
                if( ch=='\n' || ch==' ')
                {
                  symKind = toSym;
                  break;
                }
                else if(Char.IsLetter(ch))
                {
                  symLex.Append(ch);
                }
                else{
                  symKind = toSym ;
                }
                break;
            default:
                symKind = indenSym;
                GetChar();
                break;
            
          }
        }
      }
      else
      {
        symLex.Append(ch);
        switch (symLex.ToString())
            {
              case (";"):
                symKind=semicolSym;
                GetChar();
                break;
              case ("."):
                GetChar();
                if(ch=='.')
                {
                  symLex.Append(ch);
                  GetChar();
                  symKind = doublestopSym;
                }
                else{
                  symKind = stopSym;
                }
                break;
              case (","):
                symKind = commaSym ;
                GetChar();
                break;
              case (":"):
                symKind = coloSym;
                GetChar();
                break;
              case ("["):
                symKind =LsquabraSym;
                GetChar();
                break;
              case ("]"):
                  symKind= RsquabraSym;
                  GetChar();
                  break;
              case ("="):
                symKind = equSym;
                GetChar();
                break;
              case ("("):
                GetChar();
                if(ch == '*')
                {
                  GetChar();
                  while(ch != '\0')
                  {
                    GetChar();
                    if(ch =='*')
                    { 
                      GetChar();
                      if(ch == ')')
                      {
                        GetChar();
                        GetSym(); 
                        break;
                      }
                    } 
                  }    
                  return;
                }
                else
                {
                symKind= LbraSym;
                }
                break;
              case (")"):
                  symKind= RbraSym;
                  GetChar();
                  break;
              default:
                symKind = EOFSym;
                GetChar();
                  break;                
            }
        } 
        // check for string literals and specials keywords

        // check for special chars

        // 
      sym = new Token(symKind, symLex.ToString());
     
    } // GetSym

    // +++++++++++++++++++++++++++++++ Parser +++++++++++++++++++++++++++++++++++

    static void Accept(int wantedSym, string errorMessage) {
    // Checks that lookahead token is wantedSym
      if (sym.kind == wantedSym) 
      {
        GetSym();
        Console.WriteLine(sym.val);
      } 
      else Abort(errorMessage);
    } // Accept

    static void Accept(IntSet allowedSet, string errorMessage) {
    // Checks that lookahead token is in allowedSet
      if (allowedSet.Contains(sym.kind)) GetSym(); else Abort(errorMessage);
    } // Accept

   static void Mod2Decl()           
      {
        //Mod2Dec1 = {Declaration}
        while(sym.kind==typeSym || sym.kind==varSym)
        {
          Declaration();
        }

      }
      static void Declaration()
      { 
        //"TYPE" {TypeDec1 SYNC ";"} | "VAR" {VarDec1 SYNC ";"}
        switch(sym.kind)
        {
          case (typeSym):
           Accept(typeSym, "TYPE expected");
           while(sym.kind == indenSym)
           {
             TypeDec1();
             Accept(semicolSym, "; expected");
           }
           break;
          case (varSym):
            Accept(varSym, "VAR expected");
            while(sym.kind == indenSym)
            {
              VarDec1();
              Accept(semicolSym, "; expected");
            }
            break;
        }
      }
      static void TypeDec1()
      { //TypeDec1 = identifier "=" Type .
        Accept(indenSym, "Identifier expected");
        Accept(equSym, "= expected");
        Type();
      }
      static void VarDec1()
      {
        // VARDec1 = identList ":" Type
        IdentList();
        Accept(coloSym,": expected");
        Type();
      }
      static void Type()
      {
        // Type =  SimpleType | ArrayType | RecordType | SetType | PointerType . 
        switch(sym.kind)
        {
            case (indenSym):
              SimpleType();
              break;
            case (LbraSym):
              SimpleType();
              break;
            case (LsquabraSym):
              SimpleType();
              break;
            case (arraySym):
              ArrayType();
              break;
            case (recordSym):
              RecordType();
              break;
            case (setSym):
              SetType();
              break;
            case (pointerSym):
              PointerType();
              break;
        }
      } 
      static void SimpleType()
      {
          //  SimpleType  = QualIdent [ Subrange ] | Enumeration | Subrange 
          switch(sym.kind){
            case (indenSym):
              QualIdent();
              if(sym.kind == LsquabraSym)
              {
                SubRange();
              }
              break;
            case (LbraSym):
              Enumeration();
              break;
            case (LsquabraSym):
              SubRange();
              break;
          }
      }
      static void QualIdent()
      // QualIdent   = identifier { "." identifier } 
      {
        Accept(indenSym, "Identifier expected");
        while(sym.kind == stopSym)
        {
          Accept(stopSym, ". expected");
          Accept(indenSym, "Identifier expected");
        }
      }
      static void SubRange()
      // Subrange    = "[" Constant ".." Constant "]"  
      {
        
        Accept(LsquabraSym, "[ expected");
        Constant();
        Accept(doublestopSym, ".. expected");
        Constant();
        Accept(RsquabraSym, "] expected");
      }      
      static void Constant()
      // Constant    = number | identifier .
      {
        if(sym.kind == numSym){
          Accept(numSym, "number expected");
        }
        else if(sym.kind == indenSym){
          Accept(indenSym, "Identifier expected");
        }
      }
      static void Enumeration()
      // Enumeration = "(" IdentList ")" .
      {
          Accept(LbraSym, "( expected");
          IdentList();
          Accept(RbraSym, ") expected");
      }

      static void IdentList()
      // IdentList   = identifier { "," identifier } .
      {
        Accept(indenSym, "Identifier expected");
        while(sym.kind == commaSym)
        {
          Accept(commaSym, "Comma expected");
          Accept(indenSym, "Identifier expected");
        }
      }

      static void ArrayType()
      // ArrayType   = "ARRAY" SimpleType { "," SimpleType } "OF" Type.
      {
        Accept(arraySym, "ARRAY expected");
        SimpleType();
        while(sym.kind == commaSym)
        {
          SimpleType();
        }
        Accept(ofSym, "OF expected");
        
        Type();
      }
      static void RecordType()
      // RecordType  = "RECORD" FieldLists "END" .
      {
        Accept(recordSym, "RECORD expected");
        FieldLists();
        Accept(endSym, "END expected");
      }
      static void FieldLists()
      // FieldLists  = FieldList { ";" FieldList } 
      {
        FieldList();
        while(sym.kind == semicolSym)
        {
          Accept(semicolSym, "; Expected");
          FieldList();
        }
      }
      static void FieldList()
      // FieldList   = [ IdentList ":" Type ] 
      {
        if(sym.kind == indenSym){
          IdentList();
          Accept(coloSym, ": expected");
          Type();
        }
      }
      static void SetType()
      // SetType     = "SET" "OF" SimpleType 
      {
        Accept(setSym, "SET expected");
        Accept(ofSym, "OF expected");
        SimpleType();
      }
      static void PointerType()
      // PointerType = "POINTER" "TO" Type .
      {
        Accept(pointerSym, "POINTER expected");
        Accept(toSym, "TO expected");
        Type();
      }



    // +++++++++++++++++++++ Main driver function +++++++++++++++++++++++++++++++

    public static void Main(string[] args) {
      // Open input and output files from command line arguments
      if (args.Length == 0) {
        Console.WriteLine("Usage: Declarations FileName");
        System.Environment.Exit(1);
      }
      input = new InFile(args[0]);
      output = new OutFile(NewFileName(args[0], ".out"));

      GetChar();                                  // Lookahead character

  //  To test the scanner we can use a loop like the following:

    /*  do {
        GetSym();                                 // Lookahead symbol
        OutFile.StdOut.Write(sym.kind, 3);
        OutFile.StdOut.WriteLine(" " + sym.val);  // See what we got
      } while (sym.kind != EOFSym); */

  //  After the scanner is debugged we shall substitute this code:

      GetSym();                                   // Lookahead symbol
      Mod2Decl();                                 // Start to parse from the goal symbol
            // if we get back here everything must have been satisfactory
      Console.WriteLine("Parsed correctly");
      output.Close();
    } // Main

  } // Declarations
