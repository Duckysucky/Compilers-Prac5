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

static bool ToBool(int i) {
// return false or true according as i is 0 or 1
  return i == 0 ? false : true;
} // ToBool



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
		bool calConst;
		while (la.kind == identifier_Sym || la.kind == print_Sym) {
			if (la.kind == print_Sym) {
				Print(out calConst);
			} else {
				Assignment(out calConst);
			}
		}
		Expect(quit_Sym);
	}

	static void Print(out bool printConst) {
		bool printAlsoConst;
		Expect(print_Sym);
		Expression(out printConst);
		while (WeakSeparator(comma_Sym, 1, 2)) {
			Expression(out printAlsoConst);
			printConst = printConst && printAlsoConst;
		}
		while (!(la.kind == EOF_SYM || la.kind == semicolon_Sym)) {SynErr(27); Get();}
		Expect(semicolon_Sym);
	}

	static void Assignment(out bool AssignConst) {
		bool AssignAlsoConst;
		Variable();
		AssignConst = false;
		Expect(equal_Sym);
		Expression(out AssignAlsoConst);
		AssignConst = AssignConst && AssignAlsoConst;
		while (!(la.kind == EOF_SYM || la.kind == semicolon_Sym)) {SynErr(28); Get();}
		Expect(semicolon_Sym);
	}

	static void Variable() {
		Expect(identifier_Sym);
	}

	static void Expression(out bool ExprConst) {
		bool ExprAlsoConst;
		AndExp(out ExprConst);
		while (la.kind == barbar_Sym) {
			Get();
			AndExp(out ExprAlsoConst);
			ExprConst = ExprConst && ExprAlsoConst;
		}
	}

	static void AndExp(out bool andExpConst) {
		bool andExpAlsoConst;
		EqlExp(out andExpConst);
		while (la.kind == andand_Sym) {
			Get();
			EqlExp(out andExpAlsoConst);
			andExpConst = andExpConst && andExpAlsoConst;
		}
	}

	static void EqlExp(out bool equalExpConst) {
		bool equalExpAlsoConst;
		RelExp(out equalExpConst);
		while (la.kind == equalequal_Sym || la.kind == bangequal_Sym) {
			EqlOp();
			RelExp(out equalExpAlsoConst);
			equalExpConst = equalExpConst && equalExpAlsoConst;
		}
	}

	static void RelExp(out bool relExpConst) {
		bool relExpAlsoConst;
		AddExp(out relExpConst);
		if (StartOf(3)) {
			RelOp();
			AddExp(out relExpAlsoConst);
			relExpConst = relExpConst && relExpAlsoConst;
		}
	}

	static void EqlOp() {
		if (la.kind == equalequal_Sym) {
			Get();
		} else if (la.kind == bangequal_Sym) {
			Get();
		} else SynErr(29);
	}

	static void AddExp(out bool addExpConst) {
		bool addExpAlsoConst;
		MultExp(out addExpConst);
		while (la.kind == plus_Sym || la.kind == minus_Sym) {
			AddOp();
			MultExp(out addExpAlsoConst);
			addExpConst = addExpConst && addExpAlsoConst;
		}
	}

	static void RelOp() {
		if (la.kind == less_Sym) {
			Get();
		} else if (la.kind == lessequal_Sym) {
			Get();
		} else if (la.kind == greater_Sym) {
			Get();
		} else if (la.kind == greaterequal_Sym) {
			Get();
		} else SynErr(30);
	}

	static void MultExp(out bool multConst) {
		bool multAlsoConst;
		UnaryExp(out multConst);
		while (la.kind == star_Sym || la.kind == slash_Sym || la.kind == percent_Sym) {
			MulOp();
			UnaryExp(out multAlsoConst);
			multConst = multConst && multAlsoConst;
		}
	}

	static void AddOp() {
		if (la.kind == plus_Sym) {
			Get();
		} else if (la.kind == minus_Sym) {
			Get();
		} else SynErr(31);
	}

	static void UnaryExp(out bool UnaryConst) {
		if (StartOf(4)) {
			Factor(out UnaryConst);
		} else if (la.kind == plus_Sym) {
			Get();
			UnaryExp(out UnaryConst);
		} else if (la.kind == minus_Sym) {
			Get();
			UnaryExp(out UnaryConst);
		} else if (la.kind == bang_Sym) {
			Get();
			UnaryExp(out UnaryConst);
		} else SynErr(32);
	}

	static void MulOp() {
		if (la.kind == star_Sym) {
			Get();
		} else if (la.kind == slash_Sym) {
			Get();
		} else if (la.kind == percent_Sym) {
			Get();
		} else SynErr(33);
	}

	static void Factor(out bool factoConst) {
		if (la.kind == identifier_Sym) {
			Variable();
			factoConst = false;
		} else if (la.kind == number_Sym) {
			Number();
			factoConst = true;
		} else if (la.kind == true_Sym) {
			Get();
		} else if (la.kind == false_Sym) {
			Get();
		} else if (la.kind == lparen_Sym) {
			Get();
			Expression(out factoConst);
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
