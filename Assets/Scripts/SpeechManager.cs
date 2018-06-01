using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    // Use this for initialization
    void Start()
    {
        keywords.Add("Reset world", () =>
        {
            // Call the OnReset method on every descendant object.
            this.BroadcastMessage("OnReset");
        });

        // EZ
        keywords.Add("Freeze", () =>
        {
            // Call the OnFreeze method on every descendant object.
            this.BroadcastMessage("OnFreeze");
        });

        // EZ
        keywords.Add("Go", () =>
        {
            // Call the OnGo method on every descendant object.
            this.BroadcastMessage("OnGo");
        });

        // EZ
        keywords.Add("Show Mesh", () =>
        {
            SpatialMapping.Instance.DrawVisualMeshes = true;
        });

        // EZ
        keywords.Add("Hide Mesh", () =>
        {
            SpatialMapping.Instance.DrawVisualMeshes = false;
        });

        // EZ
        keywords.Add("Quit", () =>
        {
            Application.Quit();
        });

        keywords.Add("Drop Sphere", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null)
            {
                // Call the OnDrop method on just the focused object.
                focusObject.SendMessage("OnDrop", SendMessageOptions.DontRequireReceiver);
            }
        });

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}
