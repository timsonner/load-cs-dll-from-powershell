using System;
using System.Diagnostics.Eventing.Reader;
using System.Text;

namespace SecurityTools
{
    public class DefenderHistory
    {
        public string GetProtectionHistory() // Remove 'static' keyword
        {
            StringBuilder history = new StringBuilder();
            try
            {
                string logPath = "Microsoft-Windows-Windows Defender/Operational";
                EventLogQuery eventsQuery = new EventLogQuery(logPath, PathType.LogName);

                using (EventLogReader logReader = new EventLogReader(eventsQuery))
                {
                    EventRecord eventInstance;
                    while ((eventInstance = logReader.ReadEvent()) != null)
                    {
                        history.AppendLine($"Time Created: {eventInstance.TimeCreated}");
                        history.AppendLine($"Event ID: {eventInstance.Id}");
                        history.AppendLine($"Message: {eventInstance.FormatDescription()}");
                        history.AppendLine(new string('-', 50));
                    }
                }
            }
            catch (Exception ex)
            {
                history.AppendLine($"Error: {ex.Message}");
            }

            return history.ToString();
        }
    }
}
