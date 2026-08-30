using Reactivity;
using Snapshotter;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ReflectMandibleOpen : ReactiveBehaviour, ISnapshottableComponent
{
	[SerializeField] Transform _mandibleBone;

	Computed<Quaternion> _angle;

	IMouthExpressions _expressions;
	private Quaternion _originalRotation;

	static Dictionary<MouthOpenAmount, float> _defaultRotationMap = new()
	{
		{ MouthOpenAmount.Closed, 0 },
		{ MouthOpenAmount.Ajar, 7.7f },
		{ MouthOpenAmount.Open, 15 },
		{ MouthOpenAmount.WideOpen, 30 }
	};


	static Dictionary<MouthOpenAmount, float> _museRotationMap = new()
	{
		{ MouthOpenAmount.Closed, 0 },
		{ MouthOpenAmount.Ajar, 5.4f },
		{ MouthOpenAmount.Open, 10.6f },
		{ MouthOpenAmount.WideOpen, 19.3f }
	};


	private void Awake()
	{
		_expressions = this.GetComponent<IMouthExpressions>();
		_originalRotation = _mandibleBone.localRotation;
		_angle = this.CreateComputed(Reflect);
	}

	private void LateUpdate()
	{
		// Some animations are (accidentally) overriding this
		// Rather than hunt them all down, let's just set this here
		// In the future, it might be better to instead have the blender script strip those nodes on export
		_mandibleBone.localRotation = _angle.Val;
	}

	private Quaternion Reflect()
	{
		var expression = _expressions.Expression;
		var openAmount = _expressions.OpenAmount;

		var rotationMap = expression == MouthExpression.Muse ? _museRotationMap : _defaultRotationMap;
		var angle = _originalRotation * Quaternion.Euler(rotationMap[openAmount], 0, 0);

		// Also reflect this since the LateUpdate won't run in time if there's an animation
		_mandibleBone.localRotation = angle;
		return angle;
	}

	public SnapshotOrder SnapshotOrder => SnapshotOrder.AfterAnimate;
	public Action PrepareForSnapshot(ISnapshotterReferences references)
	{
		// I tried to mask the mandible of the snapshot animator controller, but it didn't seem to work
		// Doing it here seems like the only place to reliably set it
		LateUpdate();
		return null;
	}
}
