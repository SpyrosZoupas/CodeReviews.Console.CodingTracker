using CodingTracker.Model;
using System.Globalization;

namespace CodingTracker.DAL
{
    public class CodingSessionController
    {
        private readonly CodingTrackerRepository _codingTrackerRepository;

        public CodingSessionController(CodingTrackerRepository codingTrackerRepository)
        {
            _codingTrackerRepository = codingTrackerRepository;
        }

        public void CreateTables()
        {
            _codingTrackerRepository.CreateTables();
        }

        public bool CreateCodingSession(DateTime startDateTime, DateTime endDateTime)
        {
            CodingSession codingSession = new CodingSession()
            {
                StartDateTime = startDateTime,
                EndDateTime = endDateTime
            };

            return _codingTrackerRepository.Insert(codingSession, "codingSession");
        }

        public bool CreateGoal(DateTime startDateTime, DateTime endDateTime, double targetDuration)
        {
            Goal goal = new Goal()
            {
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                TargetDuration = targetDuration
            };

            return _codingTrackerRepository.Insert(goal, "goal");
        }

        public bool Delete(CodingRecord codingSession)
        {
            string tableName = codingSession.GetType() == typeof(CodingSession) ? "codingSession" : "goal";
            return _codingTrackerRepository.Delete(codingSession, tableName);
        }

        public bool UpdateCodingSession(CodingSession codingSession, DateTime startDateTime, DateTime endDateTime)
        {
            codingSession.StartDateTime = startDateTime;
            codingSession.EndDateTime = endDateTime;

            return _codingTrackerRepository.Update(codingSession, "codingSession");
        }

        public bool UpdateGoal(Goal goal, DateTime startDateTime, DateTime endDateTime, double targetDuration)
        {
            goal.StartDateTime = startDateTime;
            goal.EndDateTime = endDateTime;
            goal.TargetDuration = targetDuration;

            return _codingTrackerRepository.Update(goal, "goal");
        }

        public T GetById<T>(int id, string tableName)
        {
            T result = _codingTrackerRepository.GetById<T>(id, tableName);

            return result;
        }

        public List<T> GetAll<T>(string tableName)
        {
            List<T> result = _codingTrackerRepository.GetAll<T>(tableName);

            return result;
        }

        public List<CodingSession> GetAllCodingSessionsByDateRange(string filterType, DateTime baseDate, bool ascending)
        {
            DateTime start;
            DateTime end;
            switch (filterType.ToLower())
            {
                case "day":
                    start = baseDate;
                    end = baseDate.AddDays(1);
                    break;
                case "week":
                    int diff = (7 + (baseDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                    start = baseDate.AddDays(-1 * diff);
                    end = start.AddDays(7);
                    break;
                case "year":
                    start = new DateTime(baseDate.Year, 1, 1, 12, 0, 0);
                    end = start.AddYears(1).AddDays(-1);
                    break;
                default:
                    throw new ArgumentException("Invalid period type");
            }


            List<CodingSession> codingSessions = _codingTrackerRepository.GetByDateRange(start, end, ascending);

            return codingSessions;
        }

        public double GetTotalDurationByDateRange(string filterType, DateTime baseDate)
        {
            DateTime start;
            DateTime end;
            switch (filterType.ToLower())
            {
                case "day":
                    start = baseDate;
                    end = baseDate.AddDays(1);
                    break;
                case "week":
                    int diff = (7 + (baseDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                    start = baseDate.AddDays(-1 * diff);
                    end = start.AddDays(7);
                    break;
                case "year":
                    start = new DateTime(baseDate.Year, 1, 1, 12, 0, 0);
                    end = start.AddYears(1).AddDays(-1);
                    break;
                default:
                    throw new ArgumentException("Invalid period type");
            }


            double duration = _codingTrackerRepository.GetTotalDurationByDateRange(start, end);

            return duration;
        }

        public double GetAverageDurationByDateRange(string filterType, DateTime baseDate)
        {
            DateTime start;
            DateTime end;
            switch (filterType.ToLower())
            {
                case "day":
                    start = baseDate;
                    end = baseDate.AddDays(1);
                    break;
                case "week":
                    int diff = (7 + (baseDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                    start = baseDate.AddDays(-1 * diff);
                    end = start.AddDays(7);
                    break;
                case "year":
                    start = new DateTime(baseDate.Year, 1, 1, 12, 0, 0);
                    end = start.AddYears(1).AddDays(-1);
                    break;
                default:
                    throw new ArgumentException("Invalid period type");
            }


            double duration = _codingTrackerRepository.GetAverageDurationByDateRange(start, end);

            return duration;
        }

        public double GetHoursLeftForGoalCompletion(Goal goal)
        {
            return _codingTrackerRepository.GetHoursLeftForGoalCompletion(goal);
        }
    }
}
