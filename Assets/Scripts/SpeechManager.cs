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
        keywords.Add("Forward", () =>
        {
            this.BroadcastMessage("OnStepForward");
        });

        keywords.Add("Next", () =>
        {
            this.BroadcastMessage("OnStepForward");
        });

        keywords.Add("Back", () =>
        {
            this.BroadcastMessage("OnStepBack");
        });

        keywords.Add("Previous", () =>
        {
            this.BroadcastMessage("OnStepBack");
        });

        keywords.Add("Reset", () =>
        {
            this.BroadcastMessage("OnRestart");
        });

        keywords.Add("Restart", () =>
        {
            this.BroadcastMessage("OnRestart");
        });

        keywords.Add("Start", () =>
        {
            this.BroadcastMessage("OnRestart");
        });

        keywords.Add("Quit", () =>
        {
            this.BroadcastMessage("OnQuitRequested");
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