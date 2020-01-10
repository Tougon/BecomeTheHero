using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hero.Core;

/// <summary>
/// Represents a <see cref="Sequence"/> of dialogue.
/// </summary>
public class DialogueSequence : Sequence
{
    // The dialogue manager this text will be displayed in.
    private DialogueManager manager;
    private IEnumerator printAnimation;

    private bool running;

    private string text;

    public DialogueSequence(string s, DialogueManager dm)
    {
        manager = dm;
        text = s;
    }


    /// <summary>
    /// Clears displayed text and begins the sequence
    /// </summary>
    public override void SequenceStart()
    {
        active = true;
        manager.ClearText();
    }


    public override IEnumerator SequenceLoop()
    {
        // Waits until the text box is fully displayed
        while (!VariableManager.Instance.GetBoolVariableValue(VariableConstants.TEXT_BOX_IS_ACTIVE))
        {
            yield return null;
        }

        // Begin text print animation
        running = true;
        printAnimation = manager.BeginTextAnimation(text);

        while (running)
        {
            // If the player attempts to advance
            if (Input.GetMouseButtonDown(0)) // Replace this with touch when we build to platform
            {
                // Stop printing and fully display text if the animation is still going
                if (manager.isPrinting)
                    manager.EndTextAnimation(printAnimation, text);
                // Otherwise end sequence
                else
                    running = false;
            }

            yield return null;
        }

        SequenceEnd();
    }


    public override void SequenceEnd()
    {
        active = false;
    }
}
