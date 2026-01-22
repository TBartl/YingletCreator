using Reactivity;
using UnityEngine;


/// <summary>
/// Useful for juice elements that 
/// </summary>
public class NeverSelected : MonoBehaviour, ISelectable
{
	public IReadOnlyObservable<bool> Selected { get; } = new Observable<bool>(false);

}
