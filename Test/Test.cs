using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using VomitRPC;

namespace Test {
	class Test {
		public int Add(int A, int B) {
			Console.WriteLine("Add({0}, {1})", A, B);
			return A + B;
		}

		public int ArrayLen(string[] Arr) {
			Console.WriteLine("ArrayLen({0})", string.Join(", ", Arr.Select(E => string.Format("\"{0}\"", E)).ToArray()));
			return Arr.Length;
		}

		public float Mul(float A, float B) {
			Console.WriteLine("Mul({0}, {1})", A, B);
			return A * B;
		}
	}

	public class Program {
		static void Main(string[] args) {
			Test TestObj = new Test();

			// Register RPC provider
			RPCWebSocketServer Server = new RPCWebSocketServer("ws://127.0.0.1");
			Server.Bind(TestObj);
			Server.Start();


			while (true)
				;
		}
	}
}
