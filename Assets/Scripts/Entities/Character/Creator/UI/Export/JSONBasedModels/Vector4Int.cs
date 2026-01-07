/// <summary>
/// Why the hell is this missing from Unity? It has Vector2Int and Vector3Int but not Vector4Int.
/// </summary>
public struct Vector4Int
{
	public int x;
	public int y;
	public int z;
	public int w;

	public int this[int index]
	{
		get
		{
			switch (index)
			{
				case 0: return x;
				case 1: return y;
				case 2: return z;
				case 3: return w;
				default:
					throw new System.IndexOutOfRangeException("Invalid Vector4Int index: " + index);
			}
		}
		set
		{
			switch (index)
			{
				case 0: x = value; break;
				case 1: y = value; break;
				case 2: z = value; break;
				case 3: w = value; break;
				default:
					throw new System.IndexOutOfRangeException("Invalid Vector4Int index: " + index);
			}
		}
	}

	public Vector4Int(int x, int y, int z, int w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}
}
