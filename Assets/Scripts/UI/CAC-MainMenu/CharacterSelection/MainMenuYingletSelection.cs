using Character.Creator;
using Reactivity;
using UnityEngine;

public interface IMainMenuYingletSelection
{
	public CachedYingletReference Selected { get; set; }
}

public class MainMenuYingletSelection : MonoBehaviour, IMainMenuYingletSelection
{
	Observable<CachedYingletReference> _selected = new();
	public CachedYingletReference Selected
	{
		get => _selected.Val;
		set => _selected.Val = value;
	}
}
