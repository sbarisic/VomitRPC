using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using VomitRPC;

namespace Test {
	public interface ITest {
		object[] SomeMethod(int A, int B, string[] Strings);

		void SomeOtherMethod();
	}

	public class Program {
		static void Main(string[] args) {
			ITest TestCaller = RPCCaller.CreateInterfaceWrapper<ITest>((This, Name, Args) => {
				Console.WriteLine("You are calling '{0}' with {1} arguments", Name, Args?.Length ?? 0);
				return null;
			});

			object[] Ret = TestCaller.SomeMethod(2, 3, new[] { "String1", "String2" });
			TestCaller.SomeOtherMethod();

		}
	}
}
