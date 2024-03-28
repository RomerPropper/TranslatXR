using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using TMPro;

public class Text : RealtimeComponent<TextModel>
{
    private TMP_Text _TextMesh;

    private void Awake()
    {
        _TextMesh = GetComponent<TMP_Text>();
    }

    protected override void OnRealtimeModelReplaced(TextModel previousModel, TextModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.textDidChange -= TextDidChange;
        }
        if (currentModel != null)
        {
            // If this is a model that has no data set on it, populate it with the current mesh renderer color.
            if (currentModel.isFreshModel)
                currentModel.text = _TextMesh.text;

            // Update the mesh render to match the new model
            UndateTextMesh();

            // Register for events so we'll know if the color changes later
            currentModel.textDidChange += TextDidChange;
        }
    }

    private void TextDidChange(TextModel model, string text)
    {
        UndateTextMesh();
    }

    private void UndateTextMesh ()
    {
        _TextMesh.text = model.text;
    }

    public void SetText(string text)
    {
        // Set the color on the model
        // This will fire the colorChanged event on the model, which will update the renderer for both the local player and all remote players.
        model.text = text;
    }
}
