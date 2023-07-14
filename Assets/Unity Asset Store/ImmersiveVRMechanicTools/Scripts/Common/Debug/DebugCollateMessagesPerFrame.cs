using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugCollateMessagesPerFrame
{
    private int _currentFrame = -1;
    private List<string> _messagesForCurentFrame = new List<string>();
        
    public void Log(string message)
    {
        if (_currentFrame != Time.frameCount)
        {
            UnityEngine.Debug.Log($"All logged messages for frame: {Time.frameCount}: \r\n{string.Join(Environment.NewLine, _messagesForCurentFrame)}");
            _messagesForCurentFrame.Clear();

            _currentFrame = Time.frameCount;
        }
            
        _messagesForCurentFrame.Add(message);
    }
}