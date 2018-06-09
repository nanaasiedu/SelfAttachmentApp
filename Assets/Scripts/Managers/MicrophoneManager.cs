// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class MicrophoneManager : MonoBehaviour
{
    [Tooltip("A text area for the recognizer to display the recognized strings.")]
    [SerializeField]
    private Text dictationDisplay;

    private DictationRecognizer dictationRecognizer;

    // Use this string to cache the text currently displayed in the text box.
    private StringBuilder textSoFar;

    // Using an empty string specifies the default microphone. 
    private static string deviceName = string.Empty;
    private int samplingRate;
    private const int messageLength = 10;

    // Use this to reset the UI once the Microphone is done recording after it was started.
    private bool hasRecordingStarted;

    private int numberOfNewWords = 0;
    private string previousRecognisedText = "";

    private Hashtable phraseIdPhraseMap = new Hashtable();
    private Hashtable phraseIdDetectedMap = new Hashtable();

    void Awake()
    {
        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;

        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;

        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;

        dictationRecognizer.DictationError += DictationRecognizer_DictationError;

        // Query the maximum frequency of the default microphone. Use 'unused' to ignore the minimum frequency.
        int unused;
        Microphone.GetDeviceCaps(deviceName, out unused, out samplingRate);

        // Use this string to cache the text currently displayed in the text box.
        textSoFar = new StringBuilder();

        // Use this to reset the UI once the Microphone is done recording after it was started.
        hasRecordingStarted = false;
    }

    void Update()
    {
        if (hasRecordingStarted && !Microphone.IsRecording(deviceName) && dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            hasRecordingStarted = false;
            SendMessage("RecordStop");
        }
    }

    /// <summary>
    /// Turns on the dictation recognizer and begins recording audio from the default microphone.
    /// </summary>
    /// <returns>The audio clip recorded from the microphone.</returns>
    public AudioClip StartRecording()
    {
        PhraseRecognitionSystem.Shutdown();

        dictationRecognizer.Start();

        dictationDisplay.text = "Dictation is starting. It may take time to display your text the first time, but begin speaking now...";

        hasRecordingStarted = true;
        return Microphone.Start(deviceName, true, messageLength, samplingRate);
    }

    /// <summary>
    /// Ends the recording session.
    /// </summary>
    public void StopRecording()
    {
        if (dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            dictationRecognizer.Stop();
        }

        Debug.Log("Ending microphone");
        Microphone.End(deviceName);
        phraseIdDetectedMap.Clear();
        phraseIdPhraseMap.Clear();
    }

    public void detectPhrase(int phraseId, string[] words) {
        phraseIdPhraseMap.Add(phraseId, words);
        phraseIdDetectedMap.Add(phraseId, false);
    }

    public void deletePhrase(int phraseId) {
        phraseIdPhraseMap.Remove(phraseId);
        phraseIdDetectedMap.Remove(phraseId);
    }

    public void resetPhrase(int phraseId) {
        if (!phraseIdPhraseMap.Contains(phraseId)) return;

        phraseIdDetectedMap[phraseId] = true;
    }

    public bool isPhraseDetected(int phraseId) {
        if (!phraseIdDetectedMap.Contains(phraseId)) return false;

        return (bool)phraseIdDetectedMap[phraseId];
    }

    public void checkPhraseDetection(int phraseId, string[] words) {
        if (!phraseIdPhraseMap.Contains(phraseId)) return;
        string[] phrase = (string[])phraseIdPhraseMap[phraseId];

        int phraseIndex = 0;
        int detectedWords = 0;

        for (int wordsIndex = 0; wordsIndex < words.Length; wordsIndex++) {
            for (int aheadIndex = 0; aheadIndex < ScenesData.phraseDetectionLookAhead; aheadIndex++)
            {
                if (phrase[phraseIndex + aheadIndex].Equals(words[wordsIndex])) {
                    phraseIndex += 1 + aheadIndex;
                    detectedWords++;
                    break;
                }
            }

            if (detectedWords / (float)phrase.Length > ScenesData.phraseDetectionPercentage) {
                phraseIdDetectedMap[phraseId] = true;
                break;
            }
        }

        Debug.Log("Phrase detection finished : " + detectedWords + " detected: " + phraseIdDetectedMap[phraseId]);
    }

    /// <summary>
    /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
    /// </summary>
    /// <param name="text">The currently hypothesized recognition.</param>
    private void DictationRecognizer_DictationHypothesis(string text)
    {
        Debug.Log("hypo text: " + text);

        numberOfNewWords += numberOfWords(text) - numberOfWords(previousRecognisedText);

        previousRecognisedText = text;
        dictationDisplay.text = textSoFar.ToString() + " " + text + "...";
    }

    private int numberOfWords(string text) {
        int wordCount = 0, index = 0;

        while (index < text.Length)
        {
            while (index < text.Length && !char.IsWhiteSpace(text[index]))
                index++;

            wordCount++;

            while (index < text.Length && char.IsWhiteSpace(text[index]))
                index++;
        }

        return wordCount;
    }

    /// <summary>
    /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
    /// </summary>
    /// <param name="text">The text that was heard by the recognizer.</param>
    /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        char[] delimiter = { ' ', '.' };
        string[] words = text.Split(delimiter);

        foreach (int phraseId in phraseIdPhraseMap.Keys) {
            checkPhraseDetection(phraseId, words);
        }

        textSoFar.Append(text + ". ");

        previousRecognisedText = "";
        dictationDisplay.text = textSoFar.ToString();
    }

    /// <summary>
    /// This event is fired when the recognizer stops, whether from Stop() being called, a timeout occurring, or some other error.
    /// Typically, this will simply return "Complete". In this case, we check to see if the recognizer timed out.
    /// </summary>
    /// <param name="cause">An enumerated reason for the session completing.</param>
    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        Debug.Log("MICROPHONE COMPLETE: " + cause);
        if (cause == DictationCompletionCause.TimeoutExceeded) {
            dictationRecognizer.Start();
            return;
        };
        PhraseRecognitionSystem.Restart();
    }

    /// <summary>
    /// This event is fired when an error occurs.
    /// </summary>
    /// <param name="error">The string representation of the error reason.</param>
    /// <param name="hresult">The int representation of the hresult.</param>
    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        dictationDisplay.text = error + "\nHRESULT: " + hresult;
    }

    /// <summary>
    /// The dictation recognizer may not turn off immediately, so this call blocks on
    /// the recognizer reporting that it has actually stopped.
    /// </summary>
    public IEnumerator WaitForDictationToStop()
    {
        while (dictationRecognizer != null && dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            yield return null;
        }
    }

    public int numberOfRecentWords() {
        int result = numberOfNewWords;
        numberOfNewWords = 0;

        return result;
    }
}
