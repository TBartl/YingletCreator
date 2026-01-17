using Reactivity;
using System;
using UnityEngine;

public sealed class ConfirmationData
{
	public ConfirmationData(string displayText, string confirmText, string id, Action confirmAction)
	{
		DisplayText = displayText;
		ConfirmText = confirmText;
		Id = id;
		ConfirmAction = confirmAction;
	}

	public string DisplayText { get; }
	public string ConfirmText { get; }
	public string Id { get; }
	public Action ConfirmAction { get; }
	public Observable<bool> DontShowAgain { get; } = new Observable<bool>(false);
}

public interface IConfirmationManager
{
	IReadOnlyObservable<ConfirmationData> Current { get; }

	void OpenConfirmation(ConfirmationData data);

	void Execute();
	void Cancel();
}

public class ConfirmationManager : MonoBehaviour, IConfirmationManager
{
	Observable<ConfirmationData> _current = new Observable<ConfirmationData>(null);
	private ISettingsManager _settingsManager;

	public IReadOnlyObservable<ConfirmationData> Current => _current;

	private void Awake()
	{
		_settingsManager = Singletons.GetSingleton<ISettingsManager>();
	}

	public void Cancel()
	{
		_current.Val = null;
	}

	public void Execute()
	{
		if (_current.Val == null) return;

		if (_current.Val.DontShowAgain.Val == true)
		{
			_settingsManager.Settings.DontShowConfirmationIdsAgain.Add(_current.Val.Id);
			_settingsManager.SaveChangesToDisk();
		}

		_current.Val.ConfirmAction();
		_current.Val = null;
	}

	public void OpenConfirmation(ConfirmationData data)
	{
		if (_settingsManager.Settings.DontShowConfirmationIdsAgain.Contains(data.Id))
		{
			data.ConfirmAction();
			return;
		}

		_current.Val = data;
	}
}
