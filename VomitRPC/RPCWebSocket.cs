using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Text.Json;

namespace VomitRPC {
	class RPCWebSocketServerBehavior : WebSocketBehavior {
		RPCWebSocketServer Server;

		public RPCWebSocketServerBehavior(RPCWebSocketServer Server) {
			this.Server = Server;
		}

		protected override void OnMessage(MessageEventArgs e) {
			string RPC = e.Data;
			RPCSerializer.DeserializeProcedureCall(RPC, out string Name, out object[] Args);

			BoundMethod BM = Server.GetBoundMethod(Name);
			ParameterInfo[] ParamInfo = BM.Method.GetParameters();

			Utils.ChangeTypes(ref Args, ParamInfo.Select(P => P.ParameterType).ToArray());

			object Ret = BM.Method.Invoke(BM.Object, Args);

			string Response = RPCSerializer.SerializeProcedureResponse(Ret);
			Send(Response);
		}
	}

	public class BoundMethod {
		public object Object;
		public MethodInfo Method;

		public BoundMethod(object Object, MethodInfo Method) {
			this.Object = Object;
			this.Method = Method;
		}
	}

	public class RPCWebSocketServer {
		WebSocketServer Server;
		Dictionary<string, BoundMethod> Methods;

		public RPCWebSocketServer(string URL) {
			Server = new WebSocketServer(URL);
			Server.Log.Output = (LogData, Str) => { };
			Server.AddWebSocketService("/rpc", () => new RPCWebSocketServerBehavior(this));

			Methods = new Dictionary<string, BoundMethod>();
		}

		public void Start() {
			Server.Start();
		}

		public void Bind<T>(T Obj) {
			MethodInfo[] TypeMethods = typeof(T).GetMethods();

			foreach (MethodInfo M in TypeMethods) {
				Methods.Add(M.Name, new BoundMethod(Obj, M));
			}
		}

		public BoundMethod GetBoundMethod(string Name) {
			return Methods[Name];
		}
	}

	public class RPCWebSocketClient {
		WebSocket Client;

		bool Awaiting;
		string Response;

		public RPCWebSocketClient(string URL) {
			Client = new WebSocket(URL);
			Client.OnMessage += Client_OnMessage;
		}

		public T BindInterface<T>() {
			return RPCCaller.CreateInterfaceWrapper<T>(PerformRPCFunc);
		}

		private void Client_OnMessage(object sender, MessageEventArgs e) {
			Response = e.Data;
			Awaiting = false;
		}

		object PerformRPCFunc(object This, string Name, MethodInfo MethInfo, object[] Args) {
			string RPC = RPCSerializer.SerializeProcedureCall(Name, Args);

			Awaiting = true;
			Client.Send(RPC);

			while (Awaiting)
				;

			object Ret = RPCSerializer.DeserializeProcedureResponse(Response, MethInfo.ReturnType);
			return Ret;
		}

		public void Connect() {
			Client.Connect();
		}
	}
}
