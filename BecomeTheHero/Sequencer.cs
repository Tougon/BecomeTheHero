using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hero.Core;

/// <summary>
/// Plays back a queue of <see cref="Sequence"/>
/// </summary>
public class Sequencer : MonoBehaviour
{
    private Queue<Sequence> sequence = new Queue<Sequence>();

    public bool active { get; private set; }


    /// <summary>
    /// Adds a <see cref="Sequence"/> into the queue.
    /// </summary>
    public void AddSequence(Sequence s)
    {
        sequence.Enqueue(s);
    }


    /// <summary>
    /// Starts the <see cref="Sequence"/> queue
    /// </summary>
    public void StartSequence()
    {
        StartCoroutine(RunSequence());
    }


    /// <summary>
    /// Loops while a <see cref="Sequence"/> is active.
    /// When the <see cref="Sequence"/> has finished playback, runs the next one in the queue.
    /// </summary>
    public IEnumerator RunSequence()
    {
        Sequence seq = sequence.Dequeue();
        seq.SequenceStart();
        StartCoroutine(seq.SequenceLoop());
        active = true;

        while (seq.active)
        {
            yield return null;
        }

        PlayNextSequence();
    }


    /// <summary>
    /// Checks if there are any more Sequences to play and if so, runs the next <see cref="Sequence"/>.
    /// </summary>
    public void PlayNextSequence()
    {
        if (sequence.Count > 0)
            StartCoroutine(RunSequence());
        else
            active = false;
    }
}
