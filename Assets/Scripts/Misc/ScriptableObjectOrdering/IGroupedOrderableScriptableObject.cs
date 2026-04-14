using UnityEngine;


/// <summary>
/// For scriptable objects that want to be ordered between each-other, but don't have a group
/// </summary>
public interface IOrderableScriptableObject
{
	public int OrderIndex { get; }
}

/// <summary>
/// For scriptable objects that both want to be under a group and want to be sorted between each-other
/// </summary>
public interface IGroupedOrderableScriptableObject<TGroup> where TGroup : ScriptableObject
{
	public IGroupedOrderData<TGroup> Order { get; }
}

public interface IGroupedOrderData<TGroup> where TGroup : ScriptableObject
{
	public TGroup Group { get; }

	public int Index { get; }
}