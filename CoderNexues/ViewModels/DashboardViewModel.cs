namespace CoderNexues.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int ActiveCamps { get; set; }
        public int TotalSubmissions { get; set; }
        public List<TopStudentDto> TopStudents { get; set; }
        public List<TopStudentDto> StrugglingStudents { get; set; }

    }

    public class TopStudentDto
    {
        public string StudentName { get; set; }
        public string CampName { get; set; }
        public int TotalScore { get; set; }
    }
}
