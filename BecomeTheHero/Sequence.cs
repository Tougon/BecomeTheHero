using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hero.Core
{
    /// <summary>
    /// Base class for an object that can be queued into a <see cref="Sequencer"/> for playback
    /// </summary>
    public class Sequence
    {
        public bool active { get; protected set; }

        public virtual void SequenceStart()
        {
            active = true;
        }


        public virtual IEnumerator SequenceLoop()
        {
            yield return null;
        }


        public virtual void SequenceEnd()
        {
            active = false;
        }
    }
}
