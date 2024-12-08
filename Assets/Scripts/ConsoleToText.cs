using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ConsoleToText : MonoBehaviour
{
    private Queue<string> messages = new Queue<string>();
    private int maxMessages = 15;

    void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (messages.Count >= maxMessages)
        {
            messages.Dequeue();
        }
        messages.Enqueue(logString);
    }
}
