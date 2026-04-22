using Character.Creator;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Entities.Character.Creator.Data
{
	public static class SerializableCustomizationDataExtensionMethods
	{
		// The native interface implementation doesn't play nicely with nullable types (aka non-struts)
		// So we have to add our own extension method
		public static void SerializeCustomizationData<T>(this BufferSerializer<T> serializer, ref SerializableCustomizationData data) where T : IReaderWriter
		{
			// This could be done in a significantly more consolidated way, but this is fine for now
			string jsonBlob = null;
			if (serializer.IsWriter)
			{
				jsonBlob = JsonUtility.ToJson(data);
			}
			serializer.SerializeValue(ref jsonBlob);
			if (serializer.IsReader)
			{
				if (data == null)
				{
					data = JsonUtility.FromJson<SerializableCustomizationData>(jsonBlob);
				}
			}
		}
	}
}
