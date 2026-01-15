using Reactivity;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Compositor
{
	/// <summary>
	/// We can't easily export a good mouth for certain formats,
	/// so we add a simple mouth mesh for export purposes only.
	/// </summary>
	public class MeshGatherer_AddMouthForExport : ReactiveBehaviour, IMeshGathererMutator
	{
		[SerializeField] MenuType _menuType;
		[SerializeField] AssetReferenceT<MeshWithMaterial> _slapOnMouth;
		private IMenuManager _menuManager;
		private Computed<bool> _onExportMenu;

		private void Awake()
		{
			_menuManager = Singletons.GetSingleton<IMenuManager>();
			_onExportMenu = CreateComputed(ComputeOnExportMenu);
		}

		bool ComputeOnExportMenu()
		{
			return _menuManager.OpenMenu.Val == _menuType;
		}

		public void Mutate(ref ISet<MeshWithMaterial> meshes)
		{
			if (!enabled) return;
			if (_onExportMenu.Val)
			{
				meshes.Add(_slapOnMouth.LoadSync());
			}
		}
	}
}
