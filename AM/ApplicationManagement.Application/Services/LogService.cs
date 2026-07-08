using AMS.Application.DTOs;
using AMS.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Application.Services
{
    public class LogService : ILogService
    {
        public async Task LogAsync(LogEntryDto log)
        {
            string logsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Logs");

            if (!Directory.Exists(logsFolder))
            {
                Directory.CreateDirectory(logsFolder);
            }

            string fileName =
                $"log-{DateTime.Now:yyyy-MM-dd}.txt";

            string filePath =
                Path.Combine(logsFolder, fileName);

            StringBuilder builder = new();

            builder.AppendLine("------------------------------------------------");

            builder.AppendLine(
                $"Date: {DateTime.Now}");

            builder.AppendLine(
                $"Level: {log.Level}");

            builder.AppendLine(
                $"Message: {log.Message}");

            if (!string.IsNullOrWhiteSpace(log.UserEmail))
            {
                builder.AppendLine(
                    $"User: {log.UserEmail}");
            }

            if (!string.IsNullOrWhiteSpace(log.Exception))
            {
                builder.AppendLine(
                    $"Exception: {log.Exception}");
            }

            builder.AppendLine("------------------------------------------------");

            await File.AppendAllTextAsync(filePath,builder.ToString());
        }
    }
}
