using Microsoft.Data.Sqlite;
using Dapper;
using CodingTracker.Model;

namespace CodingTracker.DAL
{
    public class CodingTrackerRepository
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["code-tracker"].ConnectionString;
        public void CreateTables()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS codingSession (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        StartDateTime TEXT,
                        EndDateTime TEXT,
                        Duration REAL
                    )";

                tableCmd.ExecuteNonQuery();

                tableCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS goal (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        StartDateTime TEXT,
                        EndDateTime TEXT,
                        TargetDuration REAL
                    )";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }

            PopulateTablesIfEmpty();
        }

        private void PopulateTablesIfEmpty()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var sql = "SELECT COUNT(*) FROM codingSession";
                var codingSessionCount = connection.ExecuteScalar<int>(sql);

                if (codingSessionCount == 0)
                {

                    string populateTabesSql = $@"
                    INSERT INTO
                        codingSession (StartDateTime, EndDateTime, Duration)
                    VALUES
                        (@StartDateTime, @EndDateTime, @Duration)";

                    CodingSession codingSession = new CodingSession();
                    for (int i = 1; i < 31; i++)
                    {
                        codingSession.StartDateTime = new DateTime(2025, 1, i, 0, i, i);
                        codingSession.EndDateTime = new DateTime(2025, 1, i, 12, i, i);
                        connection.Execute(populateTabesSql, codingSession);

                        codingSession.StartDateTime = codingSession.StartDateTime.AddHours(12);
                        codingSession.EndDateTime = codingSession.EndDateTime.AddHours(12);
                        connection.Execute(populateTabesSql, codingSession);

                        codingSession.StartDateTime = codingSession.StartDateTime.AddYears(1);
                        codingSession.EndDateTime = codingSession.EndDateTime.AddYears(1);
                        connection.Execute(populateTabesSql, codingSession);

                        codingSession.StartDateTime = codingSession.StartDateTime.AddHours(-12);
                        codingSession.EndDateTime = codingSession.EndDateTime.AddHours(-12);
                        connection.Execute(populateTabesSql, codingSession);
                    }

                    connection.Close();
                }
            }
        }

        public bool Insert(CodingRecord record, string tableName)
        {
            string durationColumn = tableName == "codingSession" ? "Duration" : "TargetDuration";
            string sql = $@"
                    INSERT INTO
                        {tableName} (StartDateTime, EndDateTime, {durationColumn})
                    VALUES
                        (@StartDateTime, @EndDateTime, @{durationColumn})";

            return ExecuteQuery(sql, record);
        }

        public bool Delete(CodingRecord codingSession, string tableName)
        {
            string sql = $@"
                DELETE FROM
                    {tableName}
                WHERE
                    ID = @Id";

            return ExecuteQuery(sql, codingSession);
        }

        public bool Update(CodingRecord record, string tableName)
        {
            string durationColumn = tableName == "codingSession" ? "Duration" : "TargetDuration";
            string sql = $@"
                UPDATE
                    {tableName}
                SET
                    StartDateTime = @StartDateTime, EndDateTime = @EndDateTime, {durationColumn} = @{durationColumn}
                WHERE
                    Id = @Id";

            return ExecuteQuery(sql, record);
        }

        public bool ExecuteQuery(string sql, CodingRecord codingSession)
        {
            int success;
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                success = connection.Execute(sql, codingSession);
                connection.Close();
            }

            return success != 0;
        }

        public T GetById<T>(int id, string tableName)
        {
            T result;
            string sql = $@"
                SELECT
                    *
                FROM
                    {tableName}
                WHERE
                    Id = @Id";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                result = connection.QuerySingleOrDefault<T>(sql, new { Id = id });
                connection.Close();
            }

            return result;
        }

        public List<T> GetAll<T>(string tableName)
        {
            List<T> result = new List<T>();
            string sql = $@"
                SELECT
                    *
                FROM
                    {tableName}";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                result = connection.Query<T>(sql).ToList();
                connection.Close();
            }

            return result;
        }

        public List<CodingSession> GetByDateRange(DateTime start, DateTime end, bool ascending)
        {
            string order = ascending ? "ASC" : "DESC";
            string sql = $@"
                SELECT 
                    *
                FROM 
                    codingSession
                WHERE 
                    StartDateTime BETWEEN @Start AND @End
                ORDER BY 
                    StartDateTime {order}";

            List<CodingSession> codingSessions = new List<CodingSession>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                codingSessions = connection.Query<CodingSession>(sql, new { Start = start, End = end}).ToList();
                connection.Close();
            }

             return codingSessions;
        }

        public double GetTotalDurationByDateRange(DateTime start, DateTime end)
        {
            double durationSum;
            string sql = $@"
                SELECT 
                    SUM(Duration)
                FROM 
                    codingSession
                WHERE 
                    StartDateTime BETWEEN @Start AND @End";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                durationSum = connection.ExecuteScalar<double>(sql, new { Start = start, End = end });
                connection.Close();
            }

            return durationSum;
        }

        public double GetAverageDurationByDateRange(DateTime start, DateTime end)
        {
            double durationSum;
            string sql = $@"
                SELECT 
                    SUM(Duration) / COUNT(*) AS Average
                FROM 
                    codingSession
                WHERE 
                    StartDateTime BETWEEN @Start AND @End";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                durationSum = connection.ExecuteScalar<double>(sql, new { Start = start, End = end });
                connection.Close();
            }

            return durationSum;
        }

        public double GetHoursLeftForGoalCompletion(Goal goal)
        {
            double actualDuration;
            string sql = $@"
                SELECT 
                    SUM(Duration)
                FROM 
                    codingSession
                WHERE 
                    StartDateTime >= @Start AND
                    EndDateTime <= @End";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                actualDuration = connection.ExecuteScalar<double>(sql, new { Start = goal.StartDateTime, End = goal.EndDateTime });
                connection.Close();
            }

            return goal.TargetDuration - actualDuration;
        }
    }
}