using Reactivity;
using UnityEngine;

public class CharacterClassReference : MonoBehaviour, IWriteableClassReference
{
	Observable<ClassId> _classId = new Observable<ClassId>();
	public ClassId Class => _classId.Val;

	public void SetClass(ClassId classId)
	{
		_classId.Val = classId;
	}
}
