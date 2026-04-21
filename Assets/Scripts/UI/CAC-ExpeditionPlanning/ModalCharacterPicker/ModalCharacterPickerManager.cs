using Character.Creator;
using Reactivity;
using System;
using UnityEngine;

public sealed class ModalCharacterPickerData
{
	public ModalCharacterPickerData(Action<CachedYingletReference> onPick)
	{
		OnPick = onPick;
	}

	public Action<CachedYingletReference> OnPick { get; }
}

public interface IModalCharacterPickerManager
{
	IReadOnlyObservable<ModalCharacterPickerData> Current { get; }
	void OpenModalCharacterPickerData(ModalCharacterPickerData data);
	void PickForCurrent(CachedYingletReference reference);
}

public class ModalCharacterPickerManager : MonoBehaviour, IModalCharacterPickerManager, IEscapeInputConsumer
{
	Observable<ModalCharacterPickerData> _current = new Observable<ModalCharacterPickerData>(null);
	private IEscapeInputManager _escapeInputManager;

	private void Start()
	{
		_escapeInputManager = Singletons.GetSingleton<IEscapeInputManager>();
		_escapeInputManager.Register(this);
	}

	private void OnDestroy()
	{
		_escapeInputManager.Unregister(this);
	}

	public IReadOnlyObservable<ModalCharacterPickerData> Current => _current;


	public void OpenModalCharacterPickerData(ModalCharacterPickerData data)
	{
		if (_current.Val != null)
		{
			Debug.LogError("Attempting to open a ModalCharacterSelection when one is already open.");
			return;
		}
		_current.Val = data;
	}

	public EscapeInputPriority EscapeInputPriority => EscapeInputPriority.ClosePopupModal;
	public bool OnEscape()
	{
		if (_current.Val != null)
		{
			_current.Val = null;
			return true;
		}
		return false;
	}

	public void PickForCurrent(CachedYingletReference reference)
	{
		if (_current.Val == null)
		{
			Debug.LogError("Attempting to pick a character when no ModalCharacterSelection is open.");
			return;
		}
		var onPick = _current.Val.OnPick;
		_current.Val = null;
		onPick(reference);
	}
}
