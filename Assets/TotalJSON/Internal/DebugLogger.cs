//  DebugLogger


using UnityEngine;

namespace Leguar.TotalJSON.Internal {

	internal static class DebugLogger {

		internal static void LogUserWarning(string str) {
			Debug.LogWarning("TotalJSON: " + str);
		}

		internal static void LogInternalError(string str) {
			Debug.LogError("Leguar.TotalJSON: Internal error: " + str);
		}

	}

}
