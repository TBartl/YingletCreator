using UnityEngine;

public interface IStatReference
{
	public StatId Stat { get; set; }
}

public class StatReference : MonoBehaviour, IStatReference
{
	public StatId Stat { get; set; }
}
