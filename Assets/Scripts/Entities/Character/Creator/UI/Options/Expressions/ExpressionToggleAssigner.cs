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

		CharacterIntId IntId => _intIdReference.LoadSync();

		void Awake()
		{
			_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
		}

		public int Value
		{
			get
			{
				return _dataRepo.GetInt(IntId);

			}
			set
			{
				_dataRepo.SetInt(IntId, value);
			}
		}
	}
}
