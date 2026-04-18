using UnityEngine;


namespace Character.Creator.UI
{

	internal class PortraitExpressionValueAssigner : MonoBehaviour, IExpressionToggleAssigner
	{
		[SerializeField] bool _isMouth;

		private ICustomizationSelectedDataRepository _dataRepo;
		private ICharacterCreatorUndoManager _undoManager;

		void Awake()
		{
			_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
			_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();
		}

		public int Value
		{
			get
			{
				return _isMouth
					? _dataRepo.CustomizationData.PortraitData.OverrideMouthExpression.Val
					: _dataRepo.CustomizationData.PortraitData.OverrideEyeExpression.Val;

			}
			set
			{
				_undoManager.RecordState($"Changed portrait override expression");

				if (_isMouth)
				{
					_dataRepo.CustomizationData.PortraitData.OverrideMouthExpression.Val = value;
				}
				else
				{
					_dataRepo.CustomizationData.PortraitData.OverrideEyeExpression.Val = value;
				}
			}
		}
	}
}
