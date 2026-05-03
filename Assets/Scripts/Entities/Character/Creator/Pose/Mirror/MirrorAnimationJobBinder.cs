using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;

public interface IMirrorAnimationJobBinder
{
	void SetMirror(bool mirror);
}

public class MirrorAnimationJobBinder : MonoBehaviour, IMirrorAnimationJobBinder, IInitializable
{
	private PlayableGraph _graph;
	private MirrorAnimationJob _job;
	private AnimationScriptPlayable _playable;

	public void Initialize()
	{

		var animator = GetComponent<Animator>();
		_graph = PlayableGraph.Create("MirrorAnimationGraph");
		var output = AnimationPlayableOutput.Create(_graph, "Animation", animator);

		_job = new MirrorAnimationJob(animator);
		_playable = AnimationScriptPlayable.Create(_graph, _job);
		output.SetSourcePlayable(_playable);
		output.SetAnimationStreamSource(AnimationStreamSource.PreviousInputs);

		SetMirror(false);
	}

	public void SetMirror(bool mirror)
	{
		if (mirror)
		{
			_graph.Play();
		}
		else
		{
			_graph.Stop();
		}
	}

	void OnDestroy()
	{
		_job.Dispose();
		_graph.Destroy();
	}
}
