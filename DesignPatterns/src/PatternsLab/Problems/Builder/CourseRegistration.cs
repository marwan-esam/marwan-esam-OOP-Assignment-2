namespace PatternsLab.Problems.Builder;

public sealed class CourseRegistration
{
    public string StudentEmail { get; private set;}
    public string CourseCode { get; private set;}
    public string AccessMode { get; private set;}
    public string? GroupCode { get; private set;}
    public string? DiscountCode { get; private set;}
    public bool SendWhatsApp { get; private set;}
    public bool SendEmailWelcome { get; private set;}
    public string? MentorNote { get; private set;}
    public DateOnly? PreferredStart { get; private set;}

    private CourseRegistration() {}

    // private CourseRegistration(
    //     string studentEmail,
    //     string courseCode,
    //     string accessMode,
    //     string? groupCode,
    //     string? discountCode,
    //     bool sendWhatsApp,
    //     bool sendEmailWelcome,
    //     string? mentorNote,
    //     DateOnly? preferredStart)
    // {
    //     if (string.IsNullOrWhiteSpace(studentEmail)) throw new ArgumentException("email required");
    //     if (string.IsNullOrWhiteSpace(courseCode)) throw new ArgumentException("course required");

    //     if (accessMode == "LiveGroup" && string.IsNullOrWhiteSpace(groupCode))
    //         throw new InvalidOperationException("LiveGroup requires GroupCode");
    //     if (accessMode == "VideosOnly" && !string.IsNullOrWhiteSpace(groupCode))
    //         throw new InvalidOperationException("VideosOnly cannot have GroupCode");

    //     StudentEmail = studentEmail;
    //     CourseCode = courseCode;
    //     AccessMode = accessMode;
    //     GroupCode = groupCode;
    //     DiscountCode = discountCode;
    //     SendWhatsApp = sendWhatsApp;
    //     SendEmailWelcome = sendEmailWelcome;
    //     MentorNote = mentorNote;
    //     PreferredStart = preferredStart;
    // }

    public override string ToString()
        => $"{StudentEmail} → {CourseCode} [{AccessMode}] group={GroupCode ?? "-"} discount={DiscountCode ?? "-"} wa={SendWhatsApp} mail={SendEmailWelcome}";

    public sealed class Builder
    {
        private readonly CourseRegistration _courseRegistration;

        public Builder(string studentEmail, string courseCode, string accessMode)
        {
            _courseRegistration = new CourseRegistration
            {
                StudentEmail = studentEmail,
                CourseCode = courseCode,
                AccessMode = accessMode
            };        
        }

        public Builder WithGroup(string? code)
        {
            _courseRegistration.GroupCode = code;
            return this;
        }

        public Builder WithDiscount(string? code)
        {
            _courseRegistration.DiscountCode = code;
            return this;
        }

        public Builder EnableWhatsApp()
        {
            _courseRegistration.SendWhatsApp = true;
            return this;
        }

        public Builder EnableWelcomeEmail()
        {
            _courseRegistration.SendEmailWelcome = true;
            return this;
        }

        public Builder WithMentorNote(string? note)
        {
            _courseRegistration.MentorNote = note;
            return this;
        }

        public Builder WithPrefferedStart(DateOnly? preferredStart)
        {
            _courseRegistration.PreferredStart = preferredStart;
            return this;
        }

        public CourseRegistration Build()
        {
            if (string.IsNullOrWhiteSpace(_courseRegistration.StudentEmail)) throw new ArgumentException("email required");
            if (string.IsNullOrWhiteSpace(_courseRegistration.CourseCode)) throw new ArgumentException("course required");

            if (_courseRegistration.AccessMode == "LiveGroup" && string.IsNullOrWhiteSpace(_courseRegistration.GroupCode))
                throw new InvalidOperationException("LiveGroup requires GroupCode");
            if (_courseRegistration.AccessMode == "VideosOnly" && !string.IsNullOrWhiteSpace(_courseRegistration.GroupCode))
                throw new InvalidOperationException("VideosOnly cannot have GroupCode");

            return _courseRegistration;
        }
    }
}
public static class RegistrationCallSites
{
    public static CourseRegistration CreateLiveStudentUgly()
    {
        // return new CourseRegistration(
        //     "sara@mail.com",
        //     "SEF-101",
        //     "LiveGroup",
        //     "G1",
        //     "EARLY10",
        //     true,
        //     true,
        //     "Needs evening slot",
        //     new DateOnly(2026, 10, 1));
        return new CourseRegistration.Builder(studentEmail: "sara@mail.com", courseCode: "SEF-101", accessMode: "LiveGroup")
                .WithGroup("G1")
                .WithDiscount("EARLY10")
                .EnableWhatsApp()
                .EnableWelcomeEmail()
                .WithMentorNote("Needs evening slot")
                .WithPrefferedStart(new DateOnly(2026, 10, 1))
                .Build();
    }

    public static CourseRegistration CreateVideosOnlyUgly()
    {
        // return new CourseRegistration(
        //     "ali@mail.com",
        //     "SEF-101",
        //     "VideosOnly",
        //     null,
        //     null,
        //     false,
        //     true,
        //     null,
        //     null);
        return new CourseRegistration.Builder(studentEmail: "ali@mail.com", courseCode: "SEF-101", accessMode: "VideosOnly")
                .EnableWelcomeEmail()
                .Build();
    }
}