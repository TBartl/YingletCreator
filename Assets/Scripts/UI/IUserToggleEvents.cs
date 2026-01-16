using System;

internal interface IUserToggleEvents
{
	event Action<bool> UserToggled;
}
