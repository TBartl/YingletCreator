using Character.Data;
using UnityEngine;


namespace Character.Creator.UI
{
	internal interface IExpressionToggleAssigner
	{
		public int Value { get; set; }
	}

	internal class ExpressionToggleAssigner : MonoBehaviour, IExpressionToggleAssigner
	{
		[SerializeField] AssetReferenceT<CharacterIntId> _intIdReference;
		private ICustomizationSelectedDataRepository _dataRepo;
		private ICharacterCreatorUndoManager _undoManager;

		CharacterIntId IntId => _intIdReference.LoadSync();

		void Awake()
		{
			_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
			_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();
		}

		public int Value
		{
			get
			{
				return _dataRepo.GetInt(IntId);

			}
			set
			{
				_undoManager.RecordState($"Changed override expression");
				_dataRepo.SetInt(IntId, value);
			}
		}
	}
}
