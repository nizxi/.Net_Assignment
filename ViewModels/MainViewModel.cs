using System.Windows.Input;
using AppointmentSystem.Models;

namespace AppointmentSystem.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel? _currentViewModel;
        private string _applicationTitle = "Hospital Appointment System";
        private string _version = "Version 1.0";

        public BaseViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public string ApplicationTitle
        {
            get => _applicationTitle;
            set => SetProperty(ref _applicationTitle, value);
        }

        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        public ICommand NavigateToAppointmentsCommand { get; }
        public ICommand NavigateToDoctorsCommand { get; }
        public ICommand NavigateToBookAppointmentCommand { get; }

        private AppointmentViewModel? _appointmentViewModel;
        private DoctorViewModel? _doctorViewModel;
        private BookAppointmentViewModel? _bookAppointmentViewModel;

        public MainViewModel()
        {
            NavigateToAppointmentsCommand = new RelayCommand(_ => NavigateToAppointments());
            NavigateToDoctorsCommand = new RelayCommand(_ => NavigateToDoctors());
            NavigateToBookAppointmentCommand = new RelayCommand(_ => NavigateToBookAppointment());

            // Initialize with appointments view
            NavigateToAppointments();
        }

        private void NavigateToAppointments()
        {
            _appointmentViewModel ??= new AppointmentViewModel();
            CurrentViewModel = _appointmentViewModel;
            ApplicationTitle = "Hospital Appointment System - View Appointments";
        }

        private void NavigateToDoctors()
        {
            _doctorViewModel ??= new DoctorViewModel();
            CurrentViewModel = _doctorViewModel;
            ApplicationTitle = "Hospital Appointment System - Doctor Management";
        }

        private void NavigateToBookAppointment()
        {
            // Pass the doctor view model for accessing available doctors
            _bookAppointmentViewModel ??= new BookAppointmentViewModel(_doctorViewModel ?? new DoctorViewModel(), _appointmentViewModel ?? new AppointmentViewModel());
            CurrentViewModel = _bookAppointmentViewModel;
            ApplicationTitle = "Hospital Appointment System - Book New Appointment";
        }
    }
}
