using System;
using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI currentTimeText; // Drag your TextMeshPro object for current time here in the inspector
    public TextMeshProUGUI timerText;       // Drag your TextMeshPro object for timer here in the inspector

    private DateTime startTime;

    private void Start()
    {
        // Store the time when the app started
        startTime = DateTime.UtcNow;

        // Update the display every second
        InvokeRepeating("UpdateDisplay", 0, 1.0f);
    }

private void UpdateDisplay()
{
    // Get current time in EST
    DateTime estTime = DateTime.UtcNow.AddHours(-4); // UTC to EST conversion (this doesn't account for daylight saving time)
    currentTimeText.text = estTime.ToString("hh:mm:ss tt"); // 12-hour format with AM/PM

    // Calculate elapsed time
    TimeSpan elapsedTime = DateTime.UtcNow - startTime;
    timerText.text = string.Format("{0:00}:{1:00}:{2:00}", elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds);
}

}
