using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using VomitRPC;

namespace Test {
	public interface ILuaClass {
		int Add(int A, int B);

		float Mul(float A, float B);
	}

	public class Program {
		static object LuaInvoker(object This, string Name, object[] Args) {
			// Push arguments to stack here

			// Call lua function Name

			// Pop return value from stack and return
			return null;
		}

		static void Main(string[] args) {
			ILuaClass LuaClass = RPCCaller.CreateInterfaceWrapper<ILuaClass>(LuaInvoker);

			LuaClass.Add(2, 3);
			LuaClass.Mul(4, 6);
		}
	}
}
