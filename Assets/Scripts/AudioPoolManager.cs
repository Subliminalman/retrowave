using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPoolManager : MonoBehaviour {

	// The amount of AudioSource we will initialize the pool with
	public int poolSize = 10;

	// We use a queue since it will remove the instance at the same time that we are asking for one
	private Queue<AudioSource> pool;

	void Awake()
	{
		pool = new Queue<AudioSource>();

		// Here we create initialize our pool with the specified amount of instance,
		for (int i = 0; i < poolSize; i++)  {
			pool.Enqueue(CreateNewInstance());
		}
	}

	private AudioSource CreateNewInstance() {
		GameObject go = new GameObject("AudioSourceInstance");
		// Let's group in our instance under the pool manager
		go.transform.parent = this.transform;
		return go.AddComponent<AudioSource>();
	}

	// When we are asking for an AudioSource, we will first check if we still have one in our
	// pool, if not create a new instance
	public AudioSource GetAudioSource() {
		if (pool.Count < 1) {
			return CreateNewInstance();
		} else {
			return pool.Dequeue();
		}
	}

	public IEnumerator WaitAndReturnAudioSource (float _waitTime, AudioSource _source) {
		if (_source == null) {
			yield break;
		}
		yield return new WaitForSeconds (_waitTime);
		ReturnAudioSource (_source);
	}

	// Always return the AudioSource instance once you are done with it
	public void ReturnAudioSource(AudioSource instance) {
		pool.Enqueue(instance);
	}
}
