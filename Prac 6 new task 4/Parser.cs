using Library;



using System;
using System.IO;
using System.Text;

namespace Calc {

public class Parser {
	public const int _EOF = 0;
	public const int _number = 1;
	public const int _identifier = 2;
	// terminals
	public const int EOF_SYM = 0;
	public const int number_Sym = 1;
	public const int identifier_Sym = 2;
	public const int quit_Sym = 3;
	public const int equal_Sym = 4;
	public const int semicolon_Sym = 5;
	public const int print_Sym = 6;
	public const int comma_Sym = 7;
	public const int barbar_Sym = 8;
	public const int andand_Sym = 9;
	public const int plus_Sym = 10;
	public const int minus_Sym = 11;
	public const int bang_Sym = 12;
	public const int true_Sym = 13;
	public const int false_Sym = 14;
	public const int lparen_Sym = 15;
	public const int rparen_Sym = 16;
	public const int star_Sym = 17;
	public const int slash_Sym = 18;
	public const int percent_Sym = 19;
	public const int less_Sym = 20;
	public const int lessequal_Sym = 21;
	public const int greater_Sym = 22;
	public const int greaterequal_Sym = 23;
	public const int equalequal_Sym = 24;
	public const int bangequal_Sym = 25;
	public const int NOT_SYM = 26;
	// pragmas

	public const int maxT = 26;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;

	public static Token token;    // last recognized token   /* pdt */
	public static Token la;       // lookahead token
	static int errDist = minErrDist;

	static int ToInt(bool b) {
// return 0 or 1 according as b is false or true
  return b ? 1 : 0;
} // ToInt

static string Variable_Val = "";

static bool ToBool(int i) {
// return false or true according as i is 0 or 1
  return i == 0 ? false : true;
} // ToBool

const int
   noType   = 0,
   intType  = 1,
   boolType = 2;

static int checkType= 0;
static int save =  0;
// List keeps track of the symbol table




	static void SynErr (int n) {
		if (errDist >= minErrDist) Errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public static void SemErr (string msg) {
		if (errDist >= minErrDist) Errors.Error(token.line, token.col, msg); /* pdt */
		errDist = 0;
	}

	public static void SemError (string msg) {
		if (errDist >= minErrDist) Errors.Error(token.line, token.col, msg); /* pdt */
		errDist = 0;
	}

	public static void Warning (string msg) { /* pdt */
		if (errDist > minErrDist) Errors.Warn(token.line, token.col, msg);
		errDist = 2; //++ 2009/11/04
	}

	public static bool Successful() { /* pdt */
		return Errors.count == 0;
	}

	public static string LexString() { /* pdt */
		return token.val;
	}

	public static string LookAheadString() { /* pdt */
		return la.val;
	}

	static void Get () {
		for (;;) {
			token = la; /* pdt */
			la = Scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = token; /* pdt */
		}
	}

	static void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}

	static bool StartOf (int s) {
		return set[s, la.kind];
	}

	static void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}

	static bool WeakSeparator (int n, int syFol, int repFol) {
		bool[] s = new bool[maxT+1];
		if (la.kind == n) { Get(); return true; }
		else if (StartOf(repFol)) return false;
		else {
			for (int i=0; i <= maxT; i++) {
				s[i] = set[syFol, i] || set[repFol, i] || set[0, i];
			}
			SynErr(n);
			while (!s[la.kind]) Get();
			return StartOf(syFol);
		}
	}

	static void Calc() {
		int calConstValue;
		Table.ClearTable();
		while (la.kind == identifier_Sym || la.kind == print_Sym) {
			if (la.kind == print_Sym) {
				Print(out calConstValue);
			} else {
				Assignment(out calConstValue);
			}
		}
		Expect(quit_Sym);
		Table.PrintTable();
	}

	static void Print(out int printConst) {
		int printAlsoConst;
		Expect(print_Sym);
		Expression(out printConst);
		while (WeakSeparator(comma_Sym, 1, 2)) {
			Expression(out printAlsoConst);
		}
		while (!(la.kind == EOF_SYM || la.kind == semicolon_Sym)) {SynErr(27); Get();}
		Expect(semicolon_Sym);
	}

	static void Assignment(out int AssignConst) {
		Variable();
		Variable_Val = token.val;
		Expect(equal_Sym);
		Expression(out AssignConst);
		Table.AddRef(Variable_Val, true, AssignConst);
		while (!(la.kind == EOF_SYM || la.kind == semicolon_Sym)) {SynErr(28); Get();}
		Expect(semicolon_Sym);
	}

	static void Variable() {
		Expect(identifier_Sym);
	}

	static void Expression(out int ExprValue) {
		int ExprAlsoValue; ExprValue = 0;
		AndExp(out ExprValue);
		while (la.kind == barbar_Sym) {
			Get();
			save = checkType;
			AndExp(out ExprAlsoValue);
			if(checkType == noType && save ==noType) { }
			else if(checkType == boolType && save == boolType){
			  if(ToBool(ExprValue)) ExprValue = 1;
			  else{
			    ExprValue = ExprAlsoValue;
			  }
			}
			else{
			  IO.WriteLine("Error !! Mixed Type 1");
			}
			
		}
	}

	static void AndExp(out int andExpConst) {
		int andExpAlsoConst; andExpConst = 0;
		EqlExp(out andExpConst);
		while (la.kind == andand_Sym) {
			save = checkType;
			Get();
			EqlExp(out andExpAlsoConst);
			if(checkType == noType && save == noType) { }
			else if(checkType == boolType && save == boolType){
			  if(!ToBool(andExpConst)) andExpConst = 0;
			  else if (ToBool(andExpAlsoConst)){
			    andExpConst = andExpAlsoConst;
			  }
			  else{
			    andExpConst = 0;
			  }
			}
			else{
			    IO.WriteLine("Error !! Mixed Type 1");
			}
			
		}
	}

	static void EqlExp(out int equalExpConst) {
		int equalExpAlsoConst; string combool ;
		RelExp(out equalExpConst);
		save = checkType;
		while (la.kind == equalequal_Sym || la.kind == bangequal_Sym) {
			EqlOp(out combool);
			RelExp(out equalExpAlsoConst);
			if((checkType == boolType && save == boolType) || (checkType == noType && save == noType) ) {
			   if(combool =="Eq"){equalExpConst = ToInt(equalExpConst ==equalExpAlsoConst);}
			   if(combool =="NotEq"){equalExpConst = ToInt(equalExpAlsoConst != equalExpAlsoConst) ;}
			   }else
			   {
			     IO.WriteLine("Error !! Mixed Type 1");
			   }
			
		}
	}

	static void RelExp(out int relExpConst) {
		int relExpAlsoConst; string Comp;
		AddExp(out relExpConst);
		if (StartOf(3)) {
			RelOp(out Comp);
			AddExp(out relExpAlsoConst);
			if(checkType == boolType)
			 {
			   if(Comp=="LT"){relExpConst =ToInt(relExpConst < relExpAlsoConst) ;}
			   if(Comp=="LE"){relExpConst = ToInt(relExpConst <= relExpAlsoConst) ;}
			   if(Comp=="GT"){relExpConst = ToInt(relExpConst > relExpAlsoConst) ;}
			   if(Comp=="GE"){relExpConst = ToInt(relExpConst >= relExpAlsoConst) ;}
			 }
			 else{
			   IO.WriteLine("Error!! Type Mix ");
			 }
			
		}
	}

	static void EqlOp(out string bools) {
		bools = "";
		if (la.kind == equalequal_Sym) {
			Get();
			bools = "Eq";
		} else if (la.kind == bangequal_Sym) {
			Get();
			bools = "NotEq";
		} else SynErr(29);
	}

	static void AddExp(out int addExpConst) {
		int addExpAlsoConst; string Ad_Mn;
		MultExp(out addExpConst);
		while (la.kind == plus_Sym || la.kind == minus_Sym) {
			AddOp(out Ad_Mn);
			MultExp(out addExpAlsoConst);
			if(checkType == noType){
			if(checkType == noType)
			if(Ad_Mn == "ADD"){addExpConst = addExpConst + addExpAlsoConst; }
			if(Ad_Mn == "MIN"){addExpConst = addExpConst - addExpAlsoConst; }
			 }
			 else{
			   IO.WriteLine("Error!! Type Mix");
			 }
			
		}
	}

	static void RelOp(out string compare) {
		compare = "";
		if (la.kind == less_Sym) {
			Get();
			compare = "LT";
		} else if (la.kind == lessequal_Sym) {
			Get();
			compare = "LE";
		} else if (la.kind == greater_Sym) {
			Get();
			compare = "GT";
		} else if (la.kind == greaterequal_Sym) {
			Get();
			compare = "GE";
		} else SynErr(30);
	}

	static void MultExp(out int multValue) {
		UnaryExp(out multValue);
		int multAlsoValue; string Opt;
		while (la.kind == star_Sym || la.kind == slash_Sym || la.kind == percent_Sym) {
			MulOp(out Opt);
			UnaryExp(out multAlsoValue);
			if(checkType ==noType){
			   if(Opt == "mul"){ multValue = multValue * multAlsoValue;}
			   if(Opt == "div") {
			     if(multAlsoValue != 0) multValue = multValue / multAlsoValue;
			     else IO.WriteLine("Error !! Tried to divide by zero!");
			   }
			   if(Opt == "mod") {multValue = multValue % multAlsoValue;}
			   }
			   else {
			     IO.WriteLine("Error!! Type Mix");
			   }
			
		}
	}

	static void AddOp(out string opet) {
		opet = "";
		if (la.kind == plus_Sym) {
			Get();
			opet = "ADD";
		} else if (la.kind == minus_Sym) {
			Get();
			opet = "MIN";
		} else SynErr(31);
	}

	static void UnaryExp(out int UnaryValue) {
		UnaryValue = 0;
		if (StartOf(4)) {
			Factor(out UnaryValue);
		} else if (la.kind == plus_Sym) {
			Get();
			UnaryExp(out UnaryValue);
		} else if (la.kind == minus_Sym) {
			Get();
			UnaryExp(out UnaryValue);
		} else if (la.kind == bang_Sym) {
			Get();
			UnaryExp(out UnaryValue);
			if(checkType == boolType)
			   {
			     if (UnaryValue == 1)
			       {UnaryValue =0;}
			     else{ UnaryValue = 1; }
			   }
			   else {
			       IO.WriteLine("Error!! Type Mix");
			     }
			
		} else SynErr(32);
	}

	static void MulOp(out string op) {
		op = "";
		if (la.kind == star_Sym) {
			Get();
			op = "mul";
		} else if (la.kind == slash_Sym) {
			Get();
			op = "div";
		} else if (la.kind == percent_Sym) {
			Get();
			op = "mod";
		} else SynErr(33);
	}

	static void Factor(out int factorNum) {
		factorNum = 0;
		if (la.kind == identifier_Sym) {
			Variable();
			factorNum = Table.Retrieve(token.val);
		} else if (la.kind == number_Sym) {
			Number();
			factorNum = Convert.ToInt32(token.val); checkType =noType;
		} else if (la.kind == true_Sym) {
			Get();
			factorNum = ToInt(true); checkType = boolType ;
		} else if (la.kind == false_Sym) {
			Get();
			factorNum = ToInt(false); checkType = boolType ;
		} else if (la.kind == lparen_Sym) {
			Get();
			Expression(out factorNum);
			Expect(rparen_Sym);
		} else SynErr(34);
	}

	static void Number() {
		Expect(number_Sym);
	}



	public static void Parse() {
		la = new Token();
		la.val = "";
		Get();
		Calc();
		Expect(EOF_SYM);

	}

	static bool[,] set = {
		{T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,T,x, x,x,x,x, x,x,T,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, x,x,x,x},
		{x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x}

	};

} // end Parser

/* pdt - considerable extension from here on */

public class ErrorRec {
	public int line, col, num;
	public string str;
	public ErrorRec next;

	public ErrorRec(int l, int c, string s) {
		line = l; col = c; str = s; next = null;
	}

} // end ErrorRec

public class Errors {

	public static int count = 0;                                     // number of errors detected
	public static int warns = 0;                                     // number of warnings detected
	public static string errMsgFormat = "file {0} : ({1}, {2}) {3}"; // 0=file 1=line, 2=column, 3=text
	static string fileName = "";
	static string listName = "";
	static bool mergeErrors = false;
	static StreamWriter mergedList;

	static ErrorRec first = null, last;
	static bool eof = false;

	static string GetLine() {
		char ch, CR = '\r', LF = '\n';
		int l = 0;
		StringBuilder s = new StringBuilder();
		ch = (char) Buffer.Read();
		while (ch != Buffer.EOF && ch != CR && ch != LF) {
			s.Append(ch); l++; ch = (char) Buffer.Read();
		}
		eof = (l == 0 && ch == Buffer.EOF);
		if (ch == CR) {  // check for MS-DOS
			ch = (char) Buffer.Read();
			if (ch != LF && ch != Buffer.EOF) Buffer.Pos--;
		}
		return s.ToString();
	}

	static void Display (string s, ErrorRec e) {
		mergedList.Write("**** ");
		for (int c = 1; c < e.col; c++)
			if (s[c-1] == '\t') mergedList.Write("\t"); else mergedList.Write(" ");
		mergedList.WriteLine("^ " + e.str);
	}

	public static void Init (string fn, string dir, bool merge) {
		fileName = fn;
		listName = dir + "listing.txt";
		mergeErrors = merge;
		if (mergeErrors)
			try {
				mergedList = new StreamWriter(new FileStream(listName, FileMode.Create));
			} catch (IOException) {
				Errors.Exception("-- could not open " + listName);
			}
	}

	public static void Summarize () {
		if (mergeErrors) {
			mergedList.WriteLine();
			ErrorRec cur = first;
			Buffer.Pos = 0;
			int lnr = 1;
			string s = GetLine();
			while (!eof) {
				mergedList.WriteLine("{0,4} {1}", lnr, s);
				while (cur != null && cur.line == lnr) {
					Display(s, cur); cur = cur.next;
				}
				lnr++; s = GetLine();
			}
			if (cur != null) {
				mergedList.WriteLine("{0,4}", lnr);
				while (cur != null) {
					Display(s, cur); cur = cur.next;
				}
			}
			mergedList.WriteLine();
			mergedList.WriteLine(count + " errors detected");
			if (warns > 0) mergedList.WriteLine(warns + " warnings detected");
			mergedList.Close();
		}
		switch (count) {
			case 0 : Console.WriteLine("Parsed correctly"); break;
			case 1 : Console.WriteLine("1 error detected"); break;
			default: Console.WriteLine(count + " errors detected"); break;
		}
		if (warns > 0) Console.WriteLine(warns + " warnings detected");
		if ((count > 0 || warns > 0) && mergeErrors) Console.WriteLine("see " + listName);
	}

	public static void StoreError (int line, int col, string s) {
		if (mergeErrors) {
			ErrorRec latest = new ErrorRec(line, col, s);
			if (first == null) first = latest; else last.next = latest;
			last = latest;
		} else Console.WriteLine(errMsgFormat, fileName, line, col, s);
	}

	public static void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "number expected"; break;
			case 2: s = "identifier expected"; break;
			case 3: s = "\"quit\" expected"; break;
			case 4: s = "\"=\" expected"; break;
			case 5: s = "\";\" expected"; break;
			case 6: s = "\"print\" expected"; break;
			case 7: s = "\",\" expected"; break;
			case 8: s = "\"||\" expected"; break;
			case 9: s = "\"&&\" expected"; break;
			case 10: s = "\"+\" expected"; break;
			case 11: s = "\"-\" expected"; break;
			case 12: s = "\"!\" expected"; break;
			case 13: s = "\"true\" expected"; break;
			case 14: s = "\"false\" expected"; break;
			case 15: s = "\"(\" expected"; break;
			case 16: s = "\")\" expected"; break;
			case 17: s = "\"*\" expected"; break;
			case 18: s = "\"/\" expected"; break;
			case 19: s = "\"%\" expected"; break;
			case 20: s = "\"<\" expected"; break;
			case 21: s = "\"<=\" expected"; break;
			case 22: s = "\">\" expected"; break;
			case 23: s = "\">=\" expected"; break;
			case 24: s = "\"==\" expected"; break;
			case 25: s = "\"!=\" expected"; break;
			case 26: s = "??? expected"; break;
			case 27: s = "this symbol not expected in Print"; break;
			case 28: s = "this symbol not expected in Assignment"; break;
			case 29: s = "invalid EqlOp"; break;
			case 30: s = "invalid RelOp"; break;
			case 31: s = "invalid AddOp"; break;
			case 32: s = "invalid UnaryExp"; break;
			case 33: s = "invalid MulOp"; break;
			case 34: s = "invalid Factor"; break;

			default: s = "error " + n; break;
		}
		StoreError(line, col, s);
		count++;
	}

	public static void SemErr (int line, int col, int n) {
		StoreError(line, col, ("error " + n));
		count++;
	}

	public static void Error (int line, int col, string s) {
		StoreError(line, col, s);
		count++;
	}

	public static void Error (string s) {
		if (mergeErrors) mergedList.WriteLine(s); else Console.WriteLine(s);
		count++;
	}

	public static void Warn (int line, int col, string s) {
		StoreError(line, col, s);
		warns++;
	}

	public static void Warn (string s) {
		if (mergeErrors) mergedList.WriteLine(s); else Console.WriteLine(s);
		warns++;
	}

	public static void Exception (string s) {
		Console.WriteLine(s);
		System.Environment.Exit(1);
	}

} // end Errors

} // end namespace
