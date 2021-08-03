using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VomitRPC;

namespace Test2 {
	public interface ITestInterface {
		int Add(int A, int B);

		float Mul(float A, float B);

		int ArrayLen(string[] Arr);
	}

	class Program {
		static void Main(string[] args) {
			// Connect to RPC provider
			RPCWebSocketClient Client = new RPCWebSocketClient("ws://127.0.0.1/rpc");
			ITestInterface Test = Client.BindInterface<ITestInterface>();
			Client.Connect();


			Console.WriteLine("Add = {0}", Test.Add(10, 3));

			Console.WriteLine("Mul = {0}", Test.Mul(6, 6));

			Console.WriteLine("ArrayLen = {0}", Test.ArrayLen(new string[] { "Hello", "World!" }));

			while (true)
				;
		}
	}
}
