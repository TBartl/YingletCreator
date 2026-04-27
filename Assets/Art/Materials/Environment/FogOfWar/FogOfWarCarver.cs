using UnityEngine;

public interface IFogOfWarCarver
{
	DoubleBufferedRenderTexture GenerateFogOfWarTexture(Vector2Int numRooms);
	void CarveRoom(DoubleBufferedRenderTexture renderTextures, Vector2Int roomPos);
}

public class FogOfWarCarver : MonoBehaviour, IFogOfWarCarver, IInitializable
{
	static readonly int ROOM_SCALE_PROPERTY_ID = Shader.PropertyToID("_RoomScale");
	static readonly int ROOM_OFFSET_PROPERTY_ID = Shader.PropertyToID("_RoomOffset");

	[SerializeField] int _pixelsPerRoomEdge = 256;
	[SerializeField] Material _carveMaterialSource;
	Material _carveMaterial;

	public void Initialize()
	{
		// Create copies of the materials so we don't modify the orginal
		_carveMaterial = new Material(_carveMaterialSource);
	}

	public DoubleBufferedRenderTexture GenerateFogOfWarTexture(Vector2Int numRooms)
	{
		var textureSize = numRooms * _pixelsPerRoomEdge;

		var renderTextures = new DoubleBufferedRenderTexture(textureSize, rt =>
		{
			rt.wrapMode = TextureWrapMode.Clamp;
			rt.filterMode = FilterMode.Bilinear;
			rt.format = RenderTextureFormat.R8;
			rt.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8_UNorm;
			rt.depth = 0;
		});
		return renderTextures;
	}

	public void CarveRoom(DoubleBufferedRenderTexture renderTextures, Vector2Int roomPos)
	{
		var numRooms = new Vector2Int(renderTextures.GetCurrent().width, renderTextures.GetCurrent().height) / _pixelsPerRoomEdge;

		_carveMaterial.SetVector(ROOM_SCALE_PROPERTY_ID, new Vector2(numRooms.x, numRooms.y));
		_carveMaterial.SetVector(ROOM_OFFSET_PROPERTY_ID, -new Vector2(roomPos.x / (float)numRooms.x, roomPos.y / (float)numRooms.y));

		// Make the room area white
		renderTextures.Blit(_carveMaterial);
	}
}
