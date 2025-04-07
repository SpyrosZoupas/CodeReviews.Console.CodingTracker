namespace CodingTracker.Model
{
    public class CodingSession : CodingRecord
    {
        public double Duration
        {
            get
            {
                return CalculateDuration();
            }
        }  

        public double CalculateDuration()
        {
            return (EndDateTime - StartDateTime).TotalHours;
        }
    }
}
