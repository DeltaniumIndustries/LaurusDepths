using UnityEngine;

public class L
{
    public static void Info(string s)
    {
        if (!string.IsNullOrEmpty(s))
        {
            XRL.Messages.MessageQueue.AddPlayerMessage(s);
            Debug.LogError(s);
        }
        else
        {
            Debug.LogWarning("[Laurus] Attempted to log an empty or null message.");
        }
    }
}
