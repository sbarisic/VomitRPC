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

			PerformRPCAction AAA = (A, B) => PerformRPC(A, B);
			MethodBody PerformRPCImplBody = AAA.GetMethodInfo().GetMethodBody();
			PerformRPCBuilder.SetMethodBody(PerformRPCImplBody.GetILAsByteArray(), PerformRPCImplBody.MaxStackSize, null, null, null);

			/*ParameterExpression ParamThis = Expression.Parameter(typeof(object), "This");
			ParameterExpression ParamName = Expression.Parameter(typeof(string), "Name");
			ParameterExpression ParamFunc = Expression.Parameter(typeof(string), "Func");
			MethodCallExpression PerformRPCCallExp = Expression.Call(null, typeof(RPCCaller).GetMethod("PerformRPC", BindingFlags.Public | BindingFlags.Static));

			Expression.Lambda<Action<string, string>>(PerformRPCCallExp, null);

			Expression.Lambda(PerformRPCCallExp, ParamThis, ParamName, ParamFunc).CompileToMethod(PerformRPCBuilder);*/

			Type TType = TBuilder.CreateType();
			return Activator.CreateInstance(TType);
		}

		public static void PerformRPC(string Name, string Func) {
		}
	}
}
