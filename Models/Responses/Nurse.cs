namespace dotnet.Models
{
    public class Nurse
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int DutyDuration { get; set; }
        public int SharePercentage { get; set; }
        public int Salary { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public virtual User User { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    }
    public class Nurses
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IEnumerable<Nurse> NursesList { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Count { get; set; }
    }
}
