using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VomitRPC {
	struct RPCCall {
		public string Name {
			get; set;
		}

		public object[] Args {
			get; set;
		}
	}

	struct RPCResponse {
		public object Response {
			get; set;
		}
	}

	public static class RPCSerializer {
		public static string SerializeProcedureCall(string Name, object[] Args) {
			RPCCall Call = new RPCCall();
			Call.Name = Name;
			Call.Args = Args;

			return Utils.Serialize(Call);
		}

		public static void DeserializeProcedureCall(string RPC, out string Name, out object[] Args) {
			RPCCall Call = Utils.Deserialize<RPCCall>(RPC);
			Name = Call.Name;
			Args = Call.Args;
		}

		public static string SerializeProcedureResponse(object Ret) {
			RPCResponse Response = new RPCResponse();
			Response.Response = Ret;

			return Utils.Serialize(Response);
		}

		public static object DeserializeProcedureResponse(string Resp, Type ReturnType) {
			RPCResponse Response = Utils.Deserialize<RPCResponse>(Resp);

			if (Response.Response is JsonElement E)
				Response.Response = Utils.ChangeType(E, ReturnType);

			return Response.Response;
		}
	}
}
