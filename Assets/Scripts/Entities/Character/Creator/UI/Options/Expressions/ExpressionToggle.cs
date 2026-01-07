using Reactivity;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public interface IExpressionToggle
	{
		void Setup(int index, Sprite sprite);
	}

	public class ExpressionToggle : ReactiveBehaviour, IUserToggleEvents, IExpressionToggle, ISelectable
	{
		[SerializeField] Image _icon;

		private IExpressionToggleAssigner _expressionAssigner;
		private Button _button;

		private int _index = 0;
		private Computed<bool> _selected;

		public IReadOnlyObservable<bool> Selected => _selected;

		public event Action<bool> UserToggled;

		void Awake()
		{
			_expressionAssigner = this.GetComponentInParent<IExpressionToggleAssigner>();
			_button = this.GetComponent<Button>();
			_button.onClick.AddListener(OnToggle);
		}

		void Start()
		{
			_selected = CreateComputed(ComputeSelected);
		}

		private new void OnDestroy()
		{
			base.OnDestroy();
			_button.onClick.RemoveListener(OnToggle);
		}

		private bool ComputeSelected()
		{
			return _expressionAssigner.Value == _index;
		}

		public void Setup(int index, Sprite sprite)
		{
			_index = index;
			_icon.sprite = sprite;
		}

		private void OnToggle()
		{
			_expressionAssigner.Value = _index;
			UserToggled?.Invoke(true);
		}
	}
}
