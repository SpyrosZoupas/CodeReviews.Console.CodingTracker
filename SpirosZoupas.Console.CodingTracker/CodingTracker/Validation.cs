using CodingTracker.DAL;
using CodingTracker.Model;
using Spectre.Console;
using System.Globalization;

namespace CodingTracker
{
    public class Validation
    {
        private readonly CodingSessionController _controller;
        public Validation(CodingSessionController controller)
        {
            _controller = controller;
        }

        public CodingSession GetExistingCodingSession()
        {
            int id = GetValidatedInteger();
            CodingSession existingCodingSession = _controller.GetById<CodingSession>(id, "codingSession");
            while (existingCodingSession == null)
            {
                AnsiConsole.MarkupLine($"[white on red]Coding session with ID of {id} does not exist.[/]");
                id = GetValidatedInteger();
                existingCodingSession = _controller.GetById<CodingSession>(id, "codingSession");
            }

            return existingCodingSession;
        }

        public Goal GetExistingGoal()
        {
            int id = GetValidatedInteger();
            Goal existingCodingSession = _controller.GetById<Goal>(id, "goal");
            while (existingCodingSession == null)
            {
                AnsiConsole.MarkupLine($"[white on red]Goal with ID of {id} does not exist.[/]");
                id = GetValidatedInteger();
                existingCodingSession = _controller.GetById<Goal>(id, "goal");
            }

            return existingCodingSession;
        }

        public int GetValidatedInteger()
        {
            int number = AnsiConsole.Prompt(
                new TextPrompt<int>(string.Empty));

            return number;
        }

        public double GetValidatedDouble()
        {
            double number = AnsiConsole.Prompt(
                new TextPrompt<double>(string.Empty));

            return number;
        }

        public DateTime GetValidatedDateTimeValue()
        {
            string dateTime = AnsiConsole.Prompt(
                new TextPrompt<string>(string.Empty)
                .Validate(input =>
                {
                    return DateTime.TryParseExact(input, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                        ? ValidationResult.Success()
                        : DateTime.TryParseExact(input, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _) 
                        ? ValidationResult.Success() 
                        : ValidationResult.Error($"[white on red]Invalid format. Please enter any DateTime values in dd-MM-yyyy HH:mm:ss foramt. Example: 20-01-2025 13:00:00.[/]");
                }));

            if (DateTime.TryParseExact(dateTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            result = DateTime.ParseExact(dateTime, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
            AnsiConsole.MarkupLine("[yellow3_1]Time of DateTime value not set; Defaulting to 00:00:00.[/]");
            return result.Date;
        }

        public int GetValidYear()
        {
            int year = AnsiConsole.Prompt(
                new TextPrompt<int>(string.Empty)
                .Validate(input =>
                {
                    return input > 999 && input < 9999;
                }));

            return year;
        }
    }
}
