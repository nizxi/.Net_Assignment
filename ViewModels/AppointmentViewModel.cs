using System.Collections.ObjectModel;
using System.Windows.Input;
using AppointmentSystem.Models;

namespace AppointmentSystem.ViewModels
{
    public class AppointmentViewModel : BaseViewModel
    {
        private ObservableCollection<Appointment> _appointments;
        private Appointment? _selectedAppointment;
        private string _searchText = string.Empty;
        private bool _isLoading = false;
        private string _statusMessage = string.Empty;

        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set => SetProperty(ref _appointments, value);
        }

        public Appointment? SelectedAppointment
        {
            get => _selectedAppointment;
            set => SetProperty(ref _selectedAppointment, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterAppointments();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand DeleteCommand { get; }
        public ICommand UpdateStatusCommand { get; }
        public ICommand RefreshCommand { get; }

        private List<Appointment> _allAppointments = new();

        public AppointmentViewModel()
        {
            _appointments = new ObservableCollection<Appointment>();
            DeleteCommand = new RelayCommand<Appointment>(DeleteAppointment, CanDeleteAppointment);
            UpdateStatusCommand = new RelayCommand<AppointmentStatus>(UpdateAppointmentStatus, CanUpdateStatus);
            RefreshCommand = new RelayCommand(_ => LoadAppointments());

            LoadAppointments();
        }

        public void LoadAppointments()
        {
            IsLoading = true;
            try
            {
                // Simulate loading from database
                _allAppointments = GenerateSampleAppointments();
                Appointments = new ObservableCollection<Appointment>(_allAppointments);
                StatusMessage = $"Loaded {Appointments.Count} appointments";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading appointments: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void AddAppointment(Appointment appointment)
        {
            appointment.Id = _allAppointments.Count > 0 ? _allAppointments.Max(a => a.Id) + 1 : 1;
            _allAppointments.Add(appointment);
            Appointments.Add(appointment);
            StatusMessage = $"Appointment added for {appointment.PatientName}";
        }

        private void DeleteAppointment(Appointment? appointment)
        {
            if (appointment == null) return;

            _allAppointments.Remove(appointment);
            Appointments.Remove(appointment);
            StatusMessage = $"Appointment for {appointment.PatientName} deleted";
            SelectedAppointment = null;
        }

        private bool CanDeleteAppointment(Appointment? appointment) => appointment != null;

        private void UpdateAppointmentStatus(AppointmentStatus? status)
        {
            if (SelectedAppointment == null || status == null) return;

            SelectedAppointment.Status = status.Value;
            StatusMessage = $"Appointment status updated to {status}";
            OnPropertyChanged(nameof(SelectedAppointment));
        }

        private bool CanUpdateStatus(AppointmentStatus? status) => SelectedAppointment != null;

        private void FilterAppointments()
        {
            if (string.IsNullOrWhiteSpace(_searchText))
            {
                Appointments = new ObservableCollection<Appointment>(_allAppointments);
                return;
            }

            var filtered = _allAppointments.Where(a =>
                a.PatientName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                a.DoctorName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                a.PatientEmail.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                a.Department.Contains(_searchText, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            Appointments = new ObservableCollection<Appointment>(filtered);
        }

        private List<Appointment> GenerateSampleAppointments()
        {
            return new List<Appointment>
            {
                new Appointment
                {
                    Id = 1,
                    PatientName = "John Smith",
                    PatientEmail = "john.smith@email.com",
                    PatientPhone = "555-0101",
                    AppointmentDate = DateTime.Now.AddDays(1),
                    AppointmentTime = new TimeSpan(09, 30, 0),
                    DoctorName = "Sarah Johnson",
                    Department = "Cardiology",
                    Description = "Regular checkup",
                    Status = AppointmentStatus.Scheduled
                },
                new Appointment
                {
                    Id = 2,
                    PatientName = "Emma Davis",
                    PatientEmail = "emma.davis@email.com",
                    PatientPhone = "555-0102",
                    AppointmentDate = DateTime.Now.AddDays(2),
                    AppointmentTime = new TimeSpan(10, 00, 0),
                    DoctorName = "Michael Chen",
                    Department = "Neurology",
                    Description = "Brain scan follow-up",
                    Status = AppointmentStatus.Confirmed
                },
                new Appointment
                {
                    Id = 3,
                    PatientName = "Robert Wilson",
                    PatientEmail = "robert.wilson@email.com",
                    PatientPhone = "555-0103",
                    AppointmentDate = DateTime.Now.AddDays(3),
                    AppointmentTime = new TimeSpan(14, 30, 0),
                    DoctorName = "Lisa Anderson",
                    Department = "Orthopedics",
                    Description = "Knee pain consultation",
                    Status = AppointmentStatus.Scheduled
                },
                new Appointment
                {
                    Id = 4,
                    PatientName = "Amanda Taylor",
                    PatientEmail = "amanda.taylor@email.com",
                    PatientPhone = "555-0104",
                    AppointmentDate = DateTime.Now.AddDays(4),
                    AppointmentTime = new TimeSpan(11, 00, 0),
                    DoctorName = "David Martinez",
                    Department = "Dermatology",
                    Description = "Skin condition check",
                    Status = AppointmentStatus.Completed
                },
                new Appointment
                {
                    Id = 5,
                    PatientName = "Christopher Lee",
                    PatientEmail = "chris.lee@email.com",
                    PatientPhone = "555-0105",
                    AppointmentDate = DateTime.Now.AddDays(-1),
                    AppointmentTime = new TimeSpan(15, 00, 0),
                    DoctorName = "Jennifer White",
                    Department = "General Practice",
                    Description = "Annual physical",
                    Status = AppointmentStatus.Cancelled
                }
            };
        }
    }
}
