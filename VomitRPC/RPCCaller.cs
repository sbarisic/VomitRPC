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
	public delegate object PerformRPCFunc(object This, string Name, MethodInfo MethInfo, object[] Args);

	public class RPCCaller {
		const bool DumpGeneratedDll = false;

		public static T CreateInterfaceWrapper<T>(PerformRPCFunc PerformRPC) {
			AssemblyName AName = new AssemblyName("Assembly_" + typeof(T).Name);
			AppDomain Domain = Thread.GetDomain();

			AssemblyBuilder ABuilder = Domain.DefineDynamicAssembly(AName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder MBuilder = ABuilder.DefineDynamicModule("Module_" + typeof(T).Name, AName + ".dll");
			TypeBuilder TBuilder = MBuilder.DefineType("Type_" + typeof(T).Name, TypeAttributes.Public | TypeAttributes.Class);

			FieldBuilder PerformRPCField = TBuilder.DefineField("PerformRPC", typeof(PerformRPCFunc), FieldAttributes.Public);

			TBuilder.AddInterfaceImplementation(typeof(T));

			MethodInfo[] RequiredMethods = typeof(T).GetMethods();
			for (int i = 0; i < RequiredMethods.Length; i++) {
				ParameterInfo[] Params = RequiredMethods[i].GetParameters();
				Type[] ParamTypes = Params.Select(P => P.ParameterType).ToArray();

				MethodBuilder MB = TBuilder.DefineMethod(RequiredMethods[i].Name, MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual);
				MB.SetParameters(ParamTypes);
				MB.SetReturnType(RequiredMethods[i].ReturnType);

				GeneratePerformRPCCall(MB, Params, PerformRPCField);
			}


			Type TType = TBuilder.CreateType();

			if (DumpGeneratedDll)
				ABuilder.Save(AName + ".dll");

			object TypeInstance = Activator.CreateInstance(TType);
			FieldInfo PerformRPCFieldInstance = TypeInstance.GetType().GetField(PerformRPCField.Name);
			PerformRPCFieldInstance.SetValue(TypeInstance, PerformRPC);

			return (T)TypeInstance;
		}

		static void GeneratePerformRPCCall(MethodBuilder MB, ParameterInfo[] MethodParams, FieldBuilder LocalFunc) {
			ILGenerator IL = MB.GetILGenerator();

			// Get local class field which contains function to call
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, LocalFunc);

			// this
			IL.Emit(OpCodes.Ldarg_0);

			// Name
			IL.Emit(OpCodes.Ldstr, MB.Name);

			// Current method info
			MethodInfo GetCurrentMethod = typeof(MethodBase).GetMethod("GetCurrentMethod");
			IL.Emit(OpCodes.Call, GetCurrentMethod);
			IL.Emit(OpCodes.Castclass, typeof(MethodInfo));

			// Push all arguments in an array OR null if no arguments
			if (MethodParams.Length > 0) {
				IL.Emit(OpCodes.Ldc_I4_S, MethodParams.Length);
				IL.Emit(OpCodes.Newarr, typeof(object));

				for (int i = 1; i < MethodParams.Length + 1; i++) {
					int ArrayIdx = i - 1;

					IL.Emit(OpCodes.Dup);
					IL.Emit(OpCodes.Ldc_I4, ArrayIdx);
					IL.Emit(OpCodes.Ldarg, i);

					if (MethodParams[ArrayIdx].ParameterType.IsValueType)
						IL.Emit(OpCodes.Box, MethodParams[ArrayIdx].ParameterType);

					IL.Emit(OpCodes.Stelem_Ref);
				}
			} else {
				IL.Emit(OpCodes.Ldnull);
			}

			// Invoke local class field function
			MethodInfo InvokeMethod = LocalFunc.FieldType.GetMethod("Invoke");
			IL.Emit(OpCodes.Callvirt, InvokeMethod);

			// Return bullshits
			if (MB.ReturnType == typeof(void)) {
				IL.Emit(OpCodes.Pop);
			} else if (MB.ReturnType.IsValueType) {
				IL.Emit(OpCodes.Unbox_Any, MB.ReturnType);
			}

			IL.Emit(OpCodes.Ret);
		}
	}
}
