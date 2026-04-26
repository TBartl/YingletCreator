public interface IClassReference
{
	ClassId Class { get; }
}

public interface IWriteableClassReference : IClassReference
{
	void SetClass(ClassId classId);
}