namespace AppointmentSystem.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;

        public override string ToString()
        {
            return $"Dr. {Name} - {Specialization}";
        }
    }
}
