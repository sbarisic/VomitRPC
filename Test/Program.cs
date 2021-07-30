using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VomitRPC;

namespace Test {
	public interface ITest {
		void DoSomething();

		/*void Print(string Str);

		void Print(string Fmt, params object[] Args);

		int Add(int A, int B);

		string ToString(object Obj);*/
	}

	class TestImpl : ITest {
		public int Add(int A, int B) {
			return A + B;
		}

		public void DoSomething() {
			Console.WriteLine("Doing something!");
		}

		public void Print(string Str) {
			Console.WriteLine("Print: {0}", Str);
		}

		public void Print(string Fmt, params object[] Args) {
			Print(string.Format(Fmt, Args));
		}

		public string ToString(object Obj) {
			return Obj.ToString();
		}
	}

	class Program {
		static void Main(string[] args) {
			TestImpl RemoteObject = new TestImpl();

			object TestCaller = RPCCaller.CreateCaller<ITest>();
			IRPCCallerImpl Internal_Test = (IRPCCallerImpl)TestCaller;

			Internal_Test.PerformRPC("AAA", "BBB");

			//TestCaller.DoSomething();

			Console.WriteLine("Done!");
			Console.ReadLine();
		}
	}
}
