using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using VomitRPC;

namespace Test {
	public interface ITest {
		void DoSomething();

		int Add(int A, int B);

		string AppendStrings(string A, string B);

		void Print(string Str);
	}

	class TestImpl : ITest {
		public int Add(int A, int B) {
			return A + B;
		}

		public string AppendStrings(string A, string B) {
			return A + B;
		}

		public void DoSomething() {
			Console.WriteLine("Doing something!");
		}

		public void Print(string Str) {
			Console.WriteLine("Print: {0}", Str);
		}
	}

	public class Program {
		static void Main(string[] args) {
			TestImpl RemoteObject = new TestImpl();

			ITest TestCaller = RPCCaller.CreateCaller<ITest>((This, Name, Args) => {
				MethodInfo Method = RemoteObject.GetType().GetMethod(Name);
				return Method.Invoke(RemoteObject, Args);
			});

			TestCaller.DoSomething();
			Console.WriteLine("Result = {0}", TestCaller.Add(2, 3));
			TestCaller.Print("Print this string!");
			Console.WriteLine("Appended string = {0}", TestCaller.AppendStrings("Hello", "World!"));

		}
	}
}
