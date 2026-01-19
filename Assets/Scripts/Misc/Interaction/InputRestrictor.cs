using Reactivity;
using System;

/// <summary>
/// Returns if normal, keyboard input and mouse controls should be allowed
/// Restrictions will occur if the user is viewing a confirmation, settings, or about screen
/// </summary>
public interface IInputRestrictor
{
	bool InputAllowed { get; }

	/// <summary>
	/// Returns an object. While it is alive, input is restricted.
	/// </summary>
	IDisposable RestrictInput();
}
public class InputRestrictor : ReactiveBehaviour, IInputRestrictor
{
	Observable<int> _restrictionCount = new Observable<int>(0);
	private Computed<bool> _inputAllowed;

	public bool InputAllowed => _inputAllowed.Val;

	public IDisposable RestrictInput()
	{
		using var suspender = new ReactivityTrackingSuspender();
		_restrictionCount.Val++;

		return new BasicActionDisposable(() =>
		{
			using var suspender = new ReactivityTrackingSuspender();
			_restrictionCount.Val--;
		});
	}

	private void Awake()
	{
		_inputAllowed = CreateComputed(ComputeInputAllowed);
	}

	private bool ComputeInputAllowed()
	{
		return _restrictionCount.Val == 0;
	}
}
