using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class KeywordManager : MonoBehaviour {

    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    public GameObject protocolManager;
    public HandsManager handsManager;

    void Start () {
        setKeywords();

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
    }

    public void startKeywordRecognizer()
    {
        keywordRecognizer.Start();
    }

    public void stopKeywordRecogniser() {
        keywordRecognizer.Stop();
    }

    public bool isRunning() {
        return keywordRecognizer.IsRunning;
    }

    private void setKeywords() {
        keywords.Add("next", () =>
        {
            protocolManager.SendMessage("AdvanceProtocol");
        });

        keywords.Add("restart", () =>
        {
            gameObject.SendMessage("OpenStartPageScene");
        });

        keywords.Add("tap", () =>
        {
            handsManager.selectFocusedObject();
        });
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("command recognised : " + args.text);
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

}
