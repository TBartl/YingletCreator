
using Unity.GraphToolkit.Editor;

public static class EditorNodeUtils
{
	public const string EXECUTION_PORT_NAME = "ExecutionPort";

	public static T GetPortValue<T>(this Node node, string portName)
	{
		var port = node.GetInputPortByName(portName);

		if (port == null)
		{
			throw new System.ArgumentException($"Port with name {portName} not found in node of type {node.GetType().Name}");
		}

		T value = default;

		// If port is connected to another node, get value from connection
		if (port.IsConnected)
		{
			switch (port.FirstConnectedPort.GetNode())
			{
				case IVariableNode variableNode:
					variableNode.Variable.TryGetDefaultValue<T>(out value);
					return value;
				case IConstantNode constantNode:
					constantNode.TryGetValue<T>(out value);
					return value;
				default:
					break;
			}
		}
		else
		{
			// If port has embedded value, return it.
			// Otherwise, return the default value of the port
			port.TryGetValue(out value);
		}

		return value;
	}
}
