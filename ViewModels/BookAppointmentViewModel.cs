using System.Collections.ObjectModel;
using System.Windows.Input;
using AppointmentSystem.Models;

namespace AppointmentSystem.ViewModels
{
    public class BookAppointmentViewModel : BaseViewModel
    {
        private string _patientName = string.Empty;
        private string _patientEmail = string.Empty;
        private string _patientPhone = string.Empty;
        private DateTime _appointmentDate = DateTime.Now.AddDays(1);
        private TimeSpan _appointmentTime = new TimeSpan(09, 00, 0);
        private Doctor? _selectedDoctor;
        private string _description = string.Empty;
        private bool _isSubmitting = false;
        private string _statusMessage = string.Empty;
        private bool _isFormValid = false;

        private DoctorViewModel _doctorViewModel;
        private AppointmentViewModel _appointmentViewModel;

        public string PatientName
        {
            get => _patientName;
            set
            {
                if (SetProperty(ref _patientName, value))
                    ValidateForm();
            }
        }

        public string PatientEmail
        {
            get => _patientEmail;
            set
            {
                if (SetProperty(ref _patientEmail, value))
                    ValidateForm();
            }
        }

        public string PatientPhone
        {
            get => _patientPhone;
            set
            {
                if (SetProperty(ref _patientPhone, value))
                    ValidateForm();
            }
        }

        public DateTime AppointmentDate
        {
            get => _appointmentDate;
            set
            {
                if (SetProperty(ref _appointmentDate, value))
                    ValidateForm();
            }
        }

        public TimeSpan AppointmentTime
        {
            get => _appointmentTime;
            set => SetProperty(ref _appointmentTime, value);
        }

        public Doctor? SelectedDoctor
        {
            get => _selectedDoctor;
            set
            {
                if (SetProperty(ref _selectedDoctor, value))
                    ValidateForm();
            }
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public bool IsSubmitting
        {
            get => _isSubmitting;
            set => SetProperty(ref _isSubmitting, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsFormValid
        {
            get => _isFormValid;
            set => SetProperty(ref _isFormValid, value);
        }

        public ObservableCollection<Doctor> AvailableDoctors { get; }

        public ICommand SubmitCommand { get; }
        public ICommand ResetCommand { get; }

        public BookAppointmentViewModel(DoctorViewModel doctorViewModel, AppointmentViewModel appointmentViewModel)
        {
            _doctorViewModel = doctorViewModel;
            _appointmentViewModel = appointmentViewModel;

            AvailableDoctors = _doctorViewModel.Doctors;
            SubmitCommand = new RelayCommand(_ => SubmitAppointment(), _ => IsFormValid && !IsSubmitting);
            ResetCommand = new RelayCommand(_ => ResetForm());
        }

        private void ValidateForm()
        {
            IsFormValid = !string.IsNullOrWhiteSpace(PatientName) &&
                         !string.IsNullOrWhiteSpace(PatientEmail) &&
                         !string.IsNullOrWhiteSpace(PatientPhone) &&
                         IsValidEmail(PatientEmail) &&
                         IsValidPhone(PatientPhone) &&
                         SelectedDoctor != null &&
                         AppointmentDate >= DateTime.Now.Date;

            StatusMessage = string.Empty;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            return !string.IsNullOrWhiteSpace(phone) && phone.Length >= 10;
        }

        private void SubmitAppointment()
        {
            if (!IsFormValid || SelectedDoctor == null)
            {
                StatusMessage = "Please fill in all required fields correctly.";
                return;
            }

            IsSubmitting = true;
            try
            {
                var appointment = new Appointment
                {
                    PatientName = PatientName,
                    PatientEmail = PatientEmail,
                    PatientPhone = PatientPhone,
                    AppointmentDate = AppointmentDate,
                    AppointmentTime = AppointmentTime,
                    DoctorName = SelectedDoctor.Name,
                    Department = SelectedDoctor.Specialization,
                    Description = Description,
                    Status = AppointmentStatus.Scheduled
                };

                _appointmentViewModel.AddAppointment(appointment);
                StatusMessage = $"Appointment successfully booked for {PatientName}!";
                ResetForm();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error booking appointment: {ex.Message}";
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        private void ResetForm()
        {
            PatientName = string.Empty;
            PatientEmail = string.Empty;
            PatientPhone = string.Empty;
            AppointmentDate = DateTime.Now.AddDays(1);
            AppointmentTime = new TimeSpan(09, 00, 0);
            SelectedDoctor = null;
            Description = string.Empty;
            StatusMessage = string.Empty;
            ValidateForm();
        }
    }
}
