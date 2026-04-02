# Hospital Appointment System - Desktop Application

## Project Overview

A modern, professional desktop application built with **WPF (Windows Presentation Foundation)** using the **MVVM (Model-View-ViewModel)** architectural pattern. The system allows hospitals to manage appointments, doctors, and patient information efficiently.

---

## Features

### ✅ Core Features Implemented

#### 1. **View Appointments Page**
- Display all scheduled appointments in a data grid
- **Real-time search** filtering by patient name, doctor name, email, or department
- View detailed appointment information upon selection
- **Status Management**: Update appointment status (Scheduled → Confirmed → InProgress → Completed → Cancelled)
- **Delete Appointments**: Remove cancelled or obsolete appointments
- Auto-refresh functionality

#### 2. **Doctor Management Page**
- View all registered doctors with specializations
- Display doctor availability status
- View detailed doctor information (name, specialization, phone, email)
- Remove doctors from the system
- Doctor list with real-time updates

#### 3. **Book Appointment Page**
- **Patient Information Form**: Collect name, email, and phone
- **Appointment Details**: Select doctor, date, and time
- **Form Validation**: 
  - Email format validation
  - Phone number length validation (10+ digits)
  - Date validation (future dates only)
  - Required field enforcement
- **Status Feedback**: Real-time validation status
- **Reset Form**: Clear all fields with one click
- Submit new appointments with automatic ID generation

---

## Technical Stack

### Architecture & Design Patterns
- **Pattern**: MVVM (Model-View-ViewModel)
- **Framework**: .NET 8.0 Windows Desktop
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Language**: C# 11+

### Key Libraries
- **MVVM Community Toolkit** (v8.2.2): For advanced MVVM support
- **System.Net.Mail**: Email validation

### Project Structure
```
AppointmentSystem/
├── Models/
│   ├── Appointment.cs          (Appointment entity with status enum)
│   └── Doctor.cs               (Doctor entity)
├── ViewModels/
│   ├── BaseViewModel.cs         (INotifyPropertyChanged base)
│   ├── RelayCommand.cs          (ICommand implementation)
│   ├── AppointmentViewModel.cs  (Appointment logic & data)
│   ├── DoctorViewModel.cs       (Doctor logic & data)
│   ├── BookAppointmentViewModel.cs (Form & validation logic)
│   └── MainViewModel.cs         (Navigation & coordination)
├── Views/
│   ├── MainWindow.xaml          (Main application shell)
│   ├── AppointmentView.xaml     (Appointment management)
│   ├── DoctorView.xaml          (Doctor management)
│   └── BookAppointmentView.xaml (Appointment booking form)
├── App.xaml                     (Application resources & theming)
└── AppointmentSystem.csproj     (Project file)
```

---

## MVVM Pattern Implementation

### Model Layer
```csharp
public class Appointment
{
    public int Id { get; set; }
    public string PatientName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }
    // ... other properties
}

public enum AppointmentStatus
{
    Scheduled, Confirmed, InProgress, Completed, Cancelled
}
```

### ViewModel Layer
- **BaseViewModel**: Implements `INotifyPropertyChanged` for two-way binding
- **AppointmentViewModel**: Manages appointment data, filtering, status updates
- **DoctorViewModel**: Manages doctor data and availability
- **BookAppointmentViewModel**: Handles form validation and submission
- **MainViewModel**: Manages navigation between views

### View Layer
- **XAML Binding**: All views bind to ViewModels using `{Binding}`
- **Commands**: Use `RelayCommand` for button interactions
- **Data Templates**: Dynamic view switching via ContentControl

---

## Event-Driven Programming

### 1. **ICommand Implementation**
```csharp
public RelayCommand SubmitCommand => 
    new RelayCommand(_ => SubmitAppointment(), _ => IsFormValid && !IsSubmitting);
```

### 2. **PropertyChanged Events**
```csharp
public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

### 3. **Form Lifecycle Events**
- **Window Loaded**: Initialize data context
- **DataGrid Selection**: Display selected appointment details
- **TextBox TextChanged**: Trigger real-time search filtering
- **Button Click**: Execute commands for form submission, deletion, etc.

### 4. **Data Binding Events**
- Two-way binding for form inputs
- One-way binding for display data
- UpdateSourceTrigger for real-time filtering

---

## UI Controls Used

### Standard Controls
| Control | Usage | Location |
|---------|-------|----------|
| **TextBox** | Patient name, email, phone input | BookAppointmentView |
| **ComboBox** | Doctor selection dropdown | BookAppointmentView |
| **DatePicker** | Appointment date selection | BookAppointmentView |
| **Button** | Navigation, actions, form submission | All views |
| **DataGrid** | Display appointments and doctors | AppointmentView, DoctorView |
| **TextBlock** | Labels, status messages, details | All views |
| **StackPanel** | Layout and grouping | All views |
| **Grid** | Layout with rows/columns | All views |
| **Border** | Visual sections with styling | AppointmentView, BookAppointmentView |
| **ScrollViewer** | Scrollable form content | BookAppointmentView |

---

## Form Validation Logic

### BookAppointmentView Validation
```csharp
private void ValidateForm()
{
    IsFormValid = !string.IsNullOrWhiteSpace(PatientName) &&
                 !string.IsNullOrWhiteSpace(PatientEmail) &&
                 !string.IsNullOrWhiteSpace(PatientPhone) &&
                 IsValidEmail(PatientEmail) &&
                 IsValidPhone(PatientPhone) &&
                 SelectedDoctor != null &&
                 AppointmentDate >= DateTime.Now.Date;
}

private bool IsValidEmail(string email)
{
    try
    {
        var addr = new System.Net.Mail.MailAddress(email);
        return addr.Address == email;
    }
    catch { return false; }
}

private bool IsValidPhone(string phone)
{
    return !string.IsNullOrWhiteSpace(phone) && phone.Length >= 10;
}
```

---

## Data Filtering & Search

### Real-Time Search Implementation
```csharp
public string SearchText
{
    get => _searchText;
    set
    {
        if (SetProperty(ref _searchText, value))
        {
            FilterAppointments(); // Trigger on every keystroke
        }
    }
}

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
```

---

## Status Management

### Appointment Status Flow
```
Scheduled → Confirmed → InProgress → Completed
   ↓
Cancelled (any stage)
```

### Status Update Command
```csharp
private void UpdateAppointmentStatus(AppointmentStatus? status)
{
    if (SelectedAppointment == null || status == null) return;

    SelectedAppointment.Status = status.Value;
    StatusMessage = $"Appointment status updated to {status}";
    OnPropertyChanged(nameof(SelectedAppointment));
}
```

---

## Application Theming

### Color Scheme
- **Primary**: #2E5090 (Professional Blue)
- **Secondary**: #1E3A5F (Dark Blue)
- **Accent**: #0D8AA8 (Teal)
- **Success**: #28A745 (Green)
- **Danger**: #DC3545 (Red)
- **Warning**: #FFC107 (Yellow)

### Global Styles (App.xaml)
```xaml
<Style x:Key="ButtonStyle" TargetType="Button">
    <Setter Property="Background" Value="{StaticResource PrimaryBrush}"/>
    <Setter Property="Foreground" Value="White"/>
    <Setter Property="Padding" Value="10,8"/>
</Style>

<Style x:Key="TextBoxStyle" TargetType="TextBox">
    <Setter Property="Height" Value="32"/>
    <Setter Property="BorderThickness" Value="1"/>
    <Setter Property="Padding" Value="8"/>
</Style>
```

---

## How to Build & Run

### Prerequisites
- Visual Studio 2022 or later
- .NET 8.0 SDK
- Windows 10/11

### Build Steps
1. Open `AppointmentSystem.csproj` in Visual Studio
2. Build the solution (Ctrl+Shift+B)
3. Run the application (F5)

### Initial Data
- Sample appointments and doctors are pre-loaded
- Data is in-memory (not persisted between sessions)

---

## Key Features Explained

### 1. **Appointment Lifecycle**
- Users can book new appointments with automatic validation
- Doctors and dates are selected from dropdowns
- Appointment status can be updated from scheduled to completed
- Completed or cancelled appointments can be removed

### 2. **Real-Time Search**
- Search across multiple fields simultaneously
- Case-insensitive matching
- Updates instantly as you type
- Clear results by clearing search box

### 3. **Doctor Management**
- View all available doctors and their specializations
- See availability status at a glance
- Manage doctor information
- Quick access to doctor details

### 4. **Form Validation**
- Email format checking
- Phone number length validation
- Date picker with future-only constraint
- Required field enforcement
- Real-time feedback

---

## Future Enhancement Possibilities

1. **Database Integration**: Replace in-memory data with SQL Server/Entity Framework
2. **Appointment Reminders**: Email/SMS notifications
3. **Conflict Detection**: Prevent double-booking
4. **Recurring Appointments**: Support for repeated appointments
5. **Analytics Dashboard**: Reports and statistics
6. **User Authentication**: Login system for patients and staff
7. **Print Functionality**: Generate appointment confirmations
8. **Undo/Redo**: Restore deleted appointments
9. **Multi-language Support**: Localization
10. **Mobile Companion App**: Cross-platform access

---

## Screenshots Description

### Main Window
- Professional header with hospital branding
- Navigation buttons for all three main views
- Dynamic content area showing selected view
- Footer with copyright information

### View Appointments
- Data grid with all appointment columns
- Search/filter bar with real-time results
- Selected appointment details panel
- Status management buttons
- Delete functionality

### Doctor Management
- Complete doctor list with specializations
- Availability status column
- Doctor details display on selection
- Remove doctor button

### Book Appointment
- Organized form sections with styling
- Patient information inputs
- Doctor selection dropdown
- Date and time pickers
- Reason for visit textarea
- Real-time form validation indicator
- Submit and clear buttons

---

## Code Quality Features

✅ **Strong Typing**: Full C# type safety
✅ **MVVM Separation**: Clean separation of concerns
✅ **Data Binding**: Automatic UI synchronization
✅ **Command Pattern**: Decoupled event handling
✅ **Validation**: Comprehensive form validation
✅ **Error Handling**: Try-catch blocks in critical sections
✅ **Extensibility**: Easy to add new views/viewmodels
✅ **Maintainability**: Clear naming conventions
✅ **Performance**: Efficient filtering and data updates

---

## Author Notes

This application demonstrates professional WPF development with:
- Proper MVVM implementation
- Event-driven architecture
- Responsive UI design
- User-friendly validation
- Professional styling and theming

The codebase is production-ready and can be easily extended with database integration and additional features.
