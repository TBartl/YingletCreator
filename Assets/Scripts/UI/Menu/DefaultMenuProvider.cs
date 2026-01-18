using UnityEngine;

public interface IDefaultMenuProvider
{
	MenuType DefaultMenu { get; }
}

public class DefaultMenuProvider : MonoBehaviour, IDefaultMenuProvider
{
	[SerializeField] MenuType _defaultMenu;
	public MenuType DefaultMenu => _defaultMenu;
}
