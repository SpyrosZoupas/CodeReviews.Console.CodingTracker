
using CodingTracker.DAL;
using CodingTracker.Model;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Spectre.Console;

namespace CodingTracker
{
    public class UserInput
    {
        private readonly CodingSessionController _controller;
        private readonly Validation _validation;

        public UserInput(CodingSessionController controller, Validation validaton)
        {
            _controller = controller;
            _validation = validaton;
        }

        public void GetUserInput()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold purple on black]Welcome to the Coding Tracker application![/]");

            bool closeApp = false;
            do
            {
                AnsiConsole.MarkupLine("[bold purple on black]MAIN MENU[/]");
                AnsiConsole.MarkupLine("[italic hotpink3_1 on black]Please choose a sub menu:[/]");
                AnsiConsole.MarkupLine("[italic hotpink3_1 on black]0) Coding Sessions[/]");
                AnsiConsole.MarkupLine("[italic hotpink3_1 on black]1) Goals[/]");
                string input = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("[italic hotpink3_1 on black]Please type one of the following values only:[/]")
                    .AddChoices([
                        "0",
                        "1"
                    ]));

                if (input == "0")
                {
                    AnsiConsole.MarkupLine("[bold purple on black]CODING SESSION MENU[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]Please choose an action:[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]0) Close Application[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]1) Create Coding Session[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]2) Delete Coding Session[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]3) Update Coding Session[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]4) Get a specific Coding Session[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]5) Get all Coding Sessions[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]6) Get all Coding Sessions for a specific period[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]7) Get total duration of Coding Sessions for a specific period[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]8) Get average duration of Coding Sessions for a specific period[/]");

                    input = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                        .Title("[italic hotpink3_1 on black]Please type one of the following values only:[/]")
                        .AddChoices([
                            "0",
                        "1",
                        "2",
                        "3",
                        "4",
                        "5",
                        "6",
                        "7",
                        "8"
                        ]));

                    switch (input)
                    {
                        case "0":
                            AnsiConsole.MarkupLine("[red on black]Bye![/]");
                            closeApp = true;
                            Environment.Exit(0);
                            break;
                        case "1":
                            CreateCodingSession();
                            break;
                        case "2":
                            DeleteCodingSession();
                            break;
                        case "3":
                            UpdateCodingSession();
                            break;
                        case "4":
                            GetCodingSessionById();
                            break;
                        case "5":
                            GetAllCodingSessions();
                            break;
                        case "6":
                            GetAllCodingSessionsByDateRange();
                            break;
                        case "7":
                            GetTotalDurationByDateRange();
                            break;
                        case "8":
                            GetAverageDurationByDateRange();
                            break;
                        default:
                            Console.WriteLine("Invalid command!");
                            break;
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[bold purple on black]GOAL MENU[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]Please choose an action:[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]0) Close Application[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]1) Create Goal[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]2) Delete Goal[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]3) Update Goal[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]4) Get a specific Goal[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]5) Get all Goals[/]");
                    AnsiConsole.MarkupLine("[italic hotpink3_1 on black]6) Find out how many hours do you need to complete a goal.[/]");

                    input = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                        .Title("[italic hotpink3_1 on black]Please type one of the following values only:[/]")
                        .AddChoices([
                            "0",
                        "1",
                        "2",
                        "3",
                        "4",
                        "5",
                        "6"
                        ]));

                    switch (input)
                    {
                        case "0":
                            AnsiConsole.MarkupLine("[red on black]Bye![/]");
                            closeApp = true;
                            Environment.Exit(0);
                            break;
                        case "1":
                            CreateGoal();
                            break;
                        case "2":
                            DeleteGoal();
                            break;
                        case "3":
                            UpdateGoal();
                            break;
                        case "4":
                            GetGoalById();
                            break;
                        case "5":
                            GetAllGoals();
                            break;
                        case "6":
                            GetHoursLeftForGoalCompletion();
                            break;
                        default:
                            Console.WriteLine("Invalid command!");
                            break;
                    }
                }
            } while (closeApp == false);
        }

        // Should I merge XCodingSession / XGoal methods into one and just have a string sessionOrGoal parameter and use that to check with
        // ifs whether I should do this or the other?
        private void GetHoursLeftForGoalCompletion()
        {
            AnsiConsole.MarkupLine("[darkcyan]Please enter the ID of the goal you would like to check.[/]");
            int id = _validation.GetValidatedInteger();

            Goal goal = _controller.GetById<Goal>(id, "goal");
            if (goal == null)
            {
                AnsiConsole.MarkupLine($"[white on red]Goal with ID of {id} does not exist.[/]");
                return;
            }

            double hoursLeft = _controller.GetHoursLeftForGoalCompletion(goal);
            if (hoursLeft > 0)
            {
                AnsiConsole.MarkupLine($"[purple_1]You have to do {hoursLeft} hours until you complete the goal with ID of {goal.Id}[/]");
                double daysRemaining = (goal.EndDateTime - DateTime.Now).TotalDays;
                if (daysRemaining > 0) AnsiConsole.MarkupLine($"[yellow3_1]You have to do {hoursLeft / (goal.EndDateTime - DateTime.Now).TotalDays:0.###} hours per day if you would like to complete this goal on time.[/]");
                else AnsiConsole.MarkupLine($"[bold black on darkred_1] But you were too late, the end date of this goal has long since passed... You will never be able to complete it.[/]");            
            }
            else
            {
                AnsiConsole.MarkupLine($"You have already completed this goal! So far you have done {Math.Abs(hoursLeft)} extra hours.");
            }
        }

        private void CreateCodingSession()
        {
            AnsiConsole.MarkupLine("[darkcyan]Please enter the Start DateTime of your session in dd-MM-yyyy HH:mm:ss format:[/]");
            DateTime startDateTime = _validation.GetValidatedDateTimeValue();
            AnsiConsole.MarkupLine("[darkcyan]Please enter the End DateTime of your session in dd-MM-yyyy HH:mm:ss format:[/]");
            DateTime endDateTime = _validation.GetValidatedDateTimeValue();

            if (_controller.CreateCodingSession(startDateTime, endDateTime))
                AnsiConsole.MarkupLine("[white on green]Coding session created.[/]");
            else
                AnsiConsole.MarkupLine("[white on red]Something went wrong, unable to create coding session.[/]");
        }

        private void CreateGoal()
        {
            AnsiConsole.MarkupLine("[darkcyan]Please enter the Start DateTime of your goal in dd-MM-yyyy HH:mm:ss format:[/]");
            DateTime startDateTime = _validation.GetValidatedDateTimeValue();
            AnsiConsole.MarkupLine("[darkcyan]Please enter the End DateTime of your goal in dd-MM-yyyy HH:mm:ss format:[/]");
            DateTime endDateTime = _validation.GetValidatedDateTimeValue();
            AnsiConsole.MarkupLine("[darkcyan]Please enter the Target Duration of your goal in dd-MM-yyyy HH:mm:ss format:[/]");
            double goal = _validation.GetValidatedDouble();

            if (_controller.CreateGoal(startDateTime, endDateTime, goal))
                AnsiConsole.MarkupLine("[white on green]Goal created.[/]");
            else
                AnsiConsole.MarkupLine("[white on red]Something went wrong, unable to create goal.[/]");
        }

        private void DeleteCodingSession()
        {
            AnsiConsole.MarkupLine("[darkcyan]Please enter the ID of the coding session you would like to delete[/]");
            CodingSession existingCodingSession = _validation.GetExistingCodingSession();

            if (_controller.Delete(existingCodingSession))
                AnsiConsole.MarkupLine("[white on green]Coding session deleted.[/]");
            else
                AnsiConsole.MarkupLine("[white on red]Something went wrong, unable to delete coding session.[/]");
        }

        private void DeleteGoal()
        {
            AnsiConsole.MarkupLine("[darkcyan]Please enter the ID of the goal you would like to delete[/]");
            Goal existingGoal = _validation.GetExistingGoal();

            if (_controller.Delete(existingGoal))
                AnsiConsole.MarkupLine("[white on green]Goal deleted.[/]");
            else
                AnsiConsole.MarkupLine("[white on red]Something went wrong, unable to delete goal.[/]");
        }

        private void UpdateCodingSession()
        {
            AnsiConsole.MarkupLine("[darkcyan]Please enter the ID of the coding session you would like to update[/]");
            CodingSession existingCodingSession = _validation.GetExistingCodingSession();

            AnsiConsole.MarkupLine("[darkcyan]Please enter updated StartDateTime[/]");
            DateTime startDateTime = _validation.GetValidatedDateTimeValue();
            AnsiConsole.MarkupLine("[darkcyan]Please enter updated EndDateTime[/]");
            DateTime endDateTime = _validation.GetValidatedDateTimeValue();

            if (_controller.UpdateCodingSession(existingCodingSession, startDateTime, endDateTime))
                AnsiConsole.MarkupLine("[white on green]Coding session updated.[/]");
            else
                AnsiConsole.MarkupLine("[white on red]Something went wrong, unable to update coding session.[/]");
        }


        private void UpdateGoal()
        {
            AnsiConsole.MarkupLine("[darkcyan]Please enter the ID of the goal you would like to update[/]");
            Goal existingGoal = _validation.GetExistingGoal();

            AnsiConsole.MarkupLine("[darkcyan]Please enter updated StartDateTime[/]");
            DateTime startDateTime = _validation.GetValidatedDateTimeValue();
            AnsiConsole.MarkupLine("[darkcyan]Please enter updated EndDateTime[/]");
            DateTime endDateTime = _validation.GetValidatedDateTimeValue();
            AnsiConsole.MarkupLine("[darkcyan]Please enter updated Target Duration[/]");
            double targetDuration = _validation.GetValidatedDouble();

            if (_controller.UpdateGoal(existingGoal, startDateTime, endDateTime, targetDuration))
                AnsiConsole.MarkupLine("[white on green]Goal updated.[/]");
            else
                AnsiConsole.MarkupLine("[white on red]Something went wrong, unable to update goal.[/]");
        }

        private void GetCodingSessionById()
        {
            AnsiConsole.MarkupLine("[darkcyan]Please enter the ID of the coding session you would like to find.[/]");
            int id = _validation.GetValidatedInteger();

            CodingSession codingSession = _controller.GetById<CodingSession>(id, "codingSession");
            if (codingSession != null)
                AnsiConsole.MarkupLine($"[white on green]Coding session with ID of {codingSession.Id} started at: {codingSession.StartDateTime} and ended at: {codingSession.EndDateTime}. The total duration of the session is {codingSession.Duration} hours.[/]");
            else
                AnsiConsole.MarkupLine($"[white on red]Coding session with ID of {id} does not exist.[/]");
        }

        private void GetGoalById()
        {
            AnsiConsole.MarkupLine("[darkcyan]Please enter the ID of the goal you would like to find.[/]");
            int id = _validation.GetValidatedInteger();

            Goal goal = _controller.GetById<Goal>(id, "goal");
            if (goal != null)
                AnsiConsole.MarkupLine($"[white on green]Goal with ID of {goal.Id} begins at: {goal.StartDateTime} and ends at: {goal.EndDateTime}. The target duration of the goal is {goal.TargetDuration} hours.[/]");
            else
                AnsiConsole.MarkupLine($"[white on red]Coding session with ID of {id} does not exist.[/]");
        }

        private void GetAllCodingSessions()
        {
            AnsiConsole.MarkupLine("[bold springgreen2]Please find below a list of all your coding sessions.[/]");
            List<CodingSession> codingSessions = _controller.GetAll<CodingSession>("codingSession");
            if (codingSessions != null)
            {
                foreach (CodingSession c in codingSessions)
                {
                    AnsiConsole.MarkupLine($"[springgreen2]ID: {c.Id} - You had a coding session of {c.Duration} hours from {c.StartDateTime} to {c.EndDateTime}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[white on red]No coding sessions found![/]");
            }
        }

        private void GetAllGoals()
        {
            AnsiConsole.MarkupLine("[bold springgreen2]Please find below a list of all your goals.[/]");
            List<Goal> goals = _controller.GetAll<Goal>("goal");
            if (goals != null)
            {
                foreach (Goal g in goals)
                {
                    AnsiConsole.MarkupLine($"[springgreen2]ID: {g.Id} - You have a goal of a target duration of {g.TargetDuration} hours from {g.StartDateTime} to {g.EndDateTime}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[white on red]No coding sessions found![/]");
            }
        }

        private void GetAllCodingSessionsByDateRange()
        {
            var filterType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("How would you like to filter your coding sessions?")
                    .AddChoices(["Day", "Week", "Year" ]));

            AnsiConsole.MarkupLine(
                filterType switch
                {
                    "Day" => "[darkcyan]Enter the date of the day by which you want to filter [/][green](dd-MM-yyyy)[/]:",
                    "Week" => "[darkcyan]Enter any date of the week by which you want to filter [/][green](dd-MM-yyyy)[/][darkcyan] you'd like to see:[/]",
                    "Year" => "[darkcyan]Enter the year by which you want to filter [/][green](yyyy)[/]:"
                });

            DateTime inputDate;
            if (filterType != "Year")
            {
                inputDate = _validation.GetValidatedDateTimeValue();
            }
            else
            {
                int year = _validation.GetValidYear();
                inputDate = new DateTime(year, 1, 1, 0, 0, 0);
            }

            string order = AnsiConsole.Prompt(
                new TextPrompt<string>("Would you like to order the results in ascending or descending order?")
                .AddChoices(["asc", "desc"]));

            bool ascending = order == "asc";

            var sessions = _controller.GetAllCodingSessionsByDateRange(filterType, inputDate, ascending);

            if (!sessions.Any())
            {
                AnsiConsole.MarkupLine("[yellow3_1]No sessions found for the selected period.[/]");
                return;
            }

            foreach (var session in sessions)
            {
                AnsiConsole.MarkupLine($"[blue]ID:[/] {session.Id}, [green]Start:[/] {session.StartDateTime}, [green]End:[/] {session.EndDateTime}, [green]Duration:[/] {session.Duration} hours");
            }
        }

        private void GetTotalDurationByDateRange()
        {
            var filterType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("How would you like to filter your coding sessions?")
                    .AddChoices(["Day", "Week", "Year"]));

            AnsiConsole.MarkupLine(
                filterType switch
                {
                    "Day" => "[darkcyan]Enter the date of the day by which you want to filter [/][green](dd-MM-yyyy)[/]:",
                    "Week" => "[darkcyan]Enter any date of the week by which you want to filter [/][green](dd-MM-yyyy)[/][darkcyan] you'd like to see:[/]",
                    "Year" => "[darkcyan]Enter the year by which you want to filter [/][green](yyyy)[/]:"
                });

            DateTime inputDate;
            if (filterType != "Year")
            {
                inputDate = _validation.GetValidatedDateTimeValue();
            }
            else
            {
                int year = _validation.GetValidYear();
                inputDate = new DateTime(year, 1, 1, 0, 0, 0);
            }

            int duration = _controller.GetTotalDurationByDateRange(filterType, inputDate);

            AnsiConsole.MarkupLine($"[white on green]You have spent a total of {duration} hours in coding sessions between the selected date range.[/]");
        }

        private void GetAverageDurationByDateRange()
        {
            var filterType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("How would you like to filter your coding sessions?")
                    .AddChoices(["Day", "Week", "Year"]));

            AnsiConsole.MarkupLine(
                filterType switch
                {
                    "Day" => "[darkcyan]Enter the date of the day by which you want to filter [/][green](dd-MM-yyyy)[/]:",
                    "Week" => "[darkcyan]Enter any date of the week by which you want to filter [/][green](dd-MM-yyyy)[/][darkcyan] you'd like to see:[/]",
                    "Year" => "[darkcyan]Enter the year by which you want to filter [/][green](yyyy)[/]:"
                });

            DateTime inputDate;
            if (filterType != "Year")
            {
                inputDate = _validation.GetValidatedDateTimeValue();
            }
            else
            {
                int year = _validation.GetValidYear();
                inputDate = new DateTime(year, 1, 1, 0, 0, 0);
            }

            double duration = _controller.GetAverageDurationByDateRange(filterType, inputDate);

            AnsiConsole.MarkupLine($"[white on green]You have spent an average of {duration} hours in coding sessions between the selected date range.[/]");
        }
    }
}
