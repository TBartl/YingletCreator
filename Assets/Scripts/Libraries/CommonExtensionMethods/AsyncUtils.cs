using System;
using System.Threading.Tasks;
using UnityEngine;

public static class AsyncUtils
{
	public static void FireAndForgetWithLogging(this Task task)
	{
		_ = FireAndForgetAsync();

		async Task FireAndForgetAsync()
		{
			try
			{
				await task;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
	}
}