using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using System.Reflection.Emit;
using System.Linq.Expressions;

namespace VomitRPC {
	public interface IRPCCallerImpl {
		void PerformRPC(string Name, string Func);
	}

	delegate void PerformRPCAction(string Name, string Func);

	public class RPCCaller {
		public static object CreateCaller<T>() {
			MethodInfo PerformRPCMethod = typeof(RPCCaller).GetMethod("PerformRPC", BindingFlags.Public | BindingFlags.Static);

			AssemblyName AName = new AssemblyName("Assembly_" + typeof(T).Name);
			AppDomain Domain = Thread.GetDomain();
			AssemblyBuilder ABuilder = Domain.DefineDynamicAssembly(AName, AssemblyBuilderAccess.Run);
			ModuleBuilder MBuilder = ABuilder.DefineDynamicModule(AName.Name);
			TypeBuilder TBuilder = MBuilder.DefineType("Caller_" + typeof(T).Name, TypeAttributes.Public | TypeAttributes.Class);

			//TBuilder.AddInterfaceImplementation(typeof(T));
			TBuilder.AddInterfaceImplementation(typeof(IRPCCallerImpl));

			MethodBuilder PerformRPCBuilder = TBuilder.DefineMethod(nameof(PerformRPC), MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual);
			PerformRPCBuilder.SetParameters(new Type[] { typeof(string), typeof(string) });
			PerformRPCBuilder.SetReturnType(typeof(void));
			GenerateStaticMethodCall(PerformRPCBuilder, PerformRPCMethod);

			/*MethodInfo[] RequiredMethods = typeof(T).GetMethods();
			for (int i = 0; i < RequiredMethods.Length; i++) {
				MethodBuilder MB = TBuilder.DefineMethod(RequiredMethods[i].Name, MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual);


			}*/

			Type TType = TBuilder.CreateType();
			return Activator.CreateInstance(TType);
		}

		static void GenerateStaticMethodCall(MethodBuilder MB, MethodInfo StaticMethod) {
			ParameterInfo[] Params = StaticMethod.GetParameters();
			ILGenerator IL = MB.GetILGenerator();

			for (int i = 0; i < Params.Length; i++) {
				if (i == 1) {
					IL.Emit(OpCodes.Ldstr, "Hello World!");
				} else
					IL.Emit(OpCodes.Ldarg, i);
			}

			IL.EmitCall(OpCodes.Call, StaticMethod, null);
			IL.Emit(OpCodes.Ret);
		}

		public static void PerformRPC(object This, string Name, string Func) {
		}
	}
}
