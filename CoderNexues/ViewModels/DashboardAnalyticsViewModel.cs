namespace CoderNexues.ViewModels
{
    public class DashboardAnalyticsViewModel
    {
        public int? SelectedCampId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<string> CampNames { get; set; }
        public List<double> CampAvgScores { get; set; }
        public List<Models.Camp> AllCamps { get; set; }

        public int TotalSubmissions { get; set; }
        public int ActiveCampsCount { get; set; }
        public int TotalStudents { get; set; }
        public int LateStudentsCount { get; set; }
        public int CommittedStudentsCount { get; set; }
        public int DropoutCount { get; set; } 
        public List<TopStudentDto> TopPerformers { get; set; }
        public List<string> LateStudentsNames { get; set; } 

        public List<TrainerStatsDto> TrainerStats { get; set; }

        public int LateTasksCount { get; set; }
        public int UngradedTasksCount { get; set; }
        public string MostFailedTask { get; set; }

        public List<SmartAlert> Alerts { get; set; }
    }

    public class TrainerStatsDto
    {
        public string Name { get; set; }
        public int StudentCount { get; set; }
        public int CompletedEvaluations { get; set; }
        public double AvgResponseTimeHours { get; set; } 
    }

    public class SmartAlert
    {
        public string Message { get; set; }
        public string Level { get; set; } 
    }
}