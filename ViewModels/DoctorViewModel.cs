using System.Collections.ObjectModel;
using System.Windows.Input;
using AppointmentSystem.Models;

namespace AppointmentSystem.ViewModels
{
    public class DoctorViewModel : BaseViewModel
    {
        private ObservableCollection<Doctor> _doctors;
        private Doctor? _selectedDoctor;
        private bool _isLoading = false;
        private string _statusMessage = string.Empty;

        public ObservableCollection<Doctor> Doctors
        {
            get => _doctors;
            set => SetProperty(ref _doctors, value);
        }

        public Doctor? SelectedDoctor
        {
            get => _selectedDoctor;
            set => SetProperty(ref _selectedDoctor, value);
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
        public ICommand RefreshCommand { get; }

        private List<Doctor> _allDoctors = new();

        public DoctorViewModel()
        {
            _doctors = new ObservableCollection<Doctor>();
            DeleteCommand = new RelayCommand<Doctor>(DeleteDoctor, CanDeleteDoctor);
            RefreshCommand = new RelayCommand(_ => LoadDoctors());

            LoadDoctors();
        }

        public void LoadDoctors()
        {
            IsLoading = true;
            try
            {
                _allDoctors = GenerateSampleDoctors();
                Doctors = new ObservableCollection<Doctor>(_allDoctors);
                StatusMessage = $"Loaded {Doctors.Count} doctors";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading doctors: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void AddDoctor(Doctor doctor)
        {
            doctor.Id = _allDoctors.Count > 0 ? _allDoctors.Max(d => d.Id) + 1 : 1;
            _allDoctors.Add(doctor);
            Doctors.Add(doctor);
            StatusMessage = $"Doctor {doctor.Name} added";
        }

        private void DeleteDoctor(Doctor? doctor)
        {
            if (doctor == null) return;

            _allDoctors.Remove(doctor);
            Doctors.Remove(doctor);
            StatusMessage = $"Doctor {doctor.Name} deleted";
            SelectedDoctor = null;
        }

        private bool CanDeleteDoctor(Doctor? doctor) => doctor != null;

        private List<Doctor> GenerateSampleDoctors()
        {
            return new List<Doctor>
            {
                new Doctor
                {
                    Id = 1,
                    Name = "Sarah Johnson",
                    Specialization = "Cardiology",
                    Phone = "555-1001",
                    Email = "sarah.johnson@hospital.com",
                    IsAvailable = true
                },
                new Doctor
                {
                    Id = 2,
                    Name = "Michael Chen",
                    Specialization = "Neurology",
                    Phone = "555-1002",
                    Email = "michael.chen@hospital.com",
                    IsAvailable = true
                },
                new Doctor
                {
                    Id = 3,
                    Name = "Lisa Anderson",
                    Specialization = "Orthopedics",
                    Phone = "555-1003",
                    Email = "lisa.anderson@hospital.com",
                    IsAvailable = false
                },
                new Doctor
                {
                    Id = 4,
                    Name = "David Martinez",
                    Specialization = "Dermatology",
                    Phone = "555-1004",
                    Email = "david.martinez@hospital.com",
                    IsAvailable = true
                },
                new Doctor
                {
                    Id = 5,
                    Name = "Jennifer White",
                    Specialization = "General Practice",
                    Phone = "555-1005",
                    Email = "jennifer.white@hospital.com",
                    IsAvailable = true
                }
            };
        }
    }
}
