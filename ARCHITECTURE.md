# WPF Appointment System - Architecture & Design Document

## System Architecture

### MVVM Pattern Overview

```
┌─────────────────────────────────────────────────────┐
│                    VIEW LAYER (XAML)                │
│  ┌──────────────────────────────────────────────┐   │
│  │  MainWindow.xaml                             │   │
│  │  AppointmentView.xaml                        │   │
│  │  DoctorView.xaml                             │   │
│  │  BookAppointmentView.xaml                    │   │
│  └──────────────────────────────────────────────┘   │
│         │                                             │
│         │ Data Binding                                │
│         ▼                                             │
├─────────────────────────────────────────────────────┤
│              VIEWMODEL LAYER (C#)                   │
│  ┌──────────────────────────────────────────────┐   │
│  │  MainViewModel                               │   │
│  │  ├─ AppointmentViewModel                     │   │
│  │  ├─ DoctorViewModel                          │   │
│  │  └─ BookAppointmentViewModel                 │   │
│  └──────────────────────────────────────────────┘   │
│         │                                             │
│         │ INotifyPropertyChanged                      │
│         │ Commands (ICommand)                         │
│         ▼                                             │
├─────────────────────────────────────────────────────┤
│               MODEL LAYER (C#)                      │
│  ┌──────────────────────────────────────────────┐   │
│  │  Appointment                                 │   │
│  │  Doctor                                      │   │
│  │  AppointmentStatus (Enum)                    │   │
│  └──────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
```

---

## Layer-by-Layer Architecture

### 1. VIEW LAYER (Presentation)
**Responsibility**: Display data and capture user input

#### Components:
- **MainWindow.xaml**: Application shell with navigation
- **AppointmentView.xaml**: Display and manage appointments
- **DoctorView.xaml**: Display and manage doctors
- **BookAppointmentView.xaml**: Form to create appointments

#### Key Features:
- XAML markup for UI definition
- Data binding expressions
- Command binding to buttons
- Event triggers (TextChanged, SelectionChanged)
- Style definitions
- Resource references

#### Technology:
- Windows Presentation Foundation (WPF)
- XAML (XML-based UI markup)
- Data binding ({Binding}, {RelativeSource})
- Markup extensions

---

### 2. VIEWMODEL LAYER (Business Logic & Coordination)
**Responsibility**: Manage data, handle commands, coordinate between View and Model

#### BaseViewModel
```csharp
public abstract class BaseViewModel : INotifyPropertyChanged
{
    // Raises PropertyChanged when data changes
    public event PropertyChangedEventHandler? PropertyChanged;
    
    // Auto-update binding on property change
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```

#### MainViewModel
```
Responsibilities:
├─ Navigation coordination
├─ View switching logic
├─ ViewModel lifecycle management
└─ Application state
```

#### AppointmentViewModel
```
Responsibilities:
├─ Load appointments from data source
├─ Real-time filtering/search
├─ Update appointment status
├─ Delete appointments
├─ Generate status messages
└─ In-memory data management
```

#### DoctorViewModel
```
Responsibilities:
├─ Load doctors from data source
├─ Doctor CRUD operations
├─ Delete doctors
├─ Generate status messages
└─ In-memory data management
```

#### BookAppointmentViewModel
```
Responsibilities:
├─ Form field management
├─ Real-time validation
├─ Email/phone validation
├─ Form submission
├─ Form reset
└─ Error handling
```

#### Command Pattern (RelayCommand)
```csharp
public class RelayCommand : ICommand
{
    // Executes the command
    private readonly Action<object?> _execute;
    
    // Determines if command can execute
    private readonly Predicate<object?>? _canExecute;
    
    // Event fired when CanExecute changes
    public event EventHandler? CanExecuteChanged;
}
```

---

### 3. MODEL LAYER (Data Entities)
**Responsibility**: Represent core business entities

#### Appointment Model
```csharp
public class Appointment
{
    public int Id { get; set; }
    public string PatientName { get; set; }
    public string PatientEmail { get; set; }
    public string PatientPhone { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public string DoctorName { get; set; }
    public string Department { get; set; }
    public string Description { get; set; }
    public AppointmentStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
}

public enum AppointmentStatus
{
    Scheduled,
    Confirmed,
    InProgress,
    Completed,
    Cancelled
}
```

#### Doctor Model
```csharp
public class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Specialization { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public bool IsAvailable { get; set; }
}
```

---

## Data Flow Architecture

### Unidirectional Data Flow (MVVM)

```
┌─────────────────────────────────────────────────────┐
│                                                       │
│  USER INTERACTION                                    │
│  (Click, Type, Select)                              │
│           ↓                                           │
│  VIEW EVENT TRIGGERED                               │
│  (TextChanged, SelectionChanged, Click)             │
│           ↓                                           │
│  COMMAND/EVENT HANDLER                              │
│  (RelayCommand.Execute)                             │
│           ↓                                           │
│  VIEWMODEL METHOD                                   │
│  (ValidateForm, FilterAppointments, etc.)           │
│           ↓                                           │
│  PROPERTY CHANGED                                   │
│  (OnPropertyChanged event raised)                   │
│           ↓                                           │
│  BINDING UPDATE                                     │
│  (Data binding updates View)                        │
│           ↓                                           │
│  UI REFRESH                                         │
│  (WPF renders new state)                            │
│                                                       │
└─────────────────────────────────────────────────────┘
```

---

## Event-Driven Architecture

### Event Flow Diagram

```
┌─ View Layer Events ──────────────────┐
│                                       │
│  Button.Click ──┐                    │
│                 ├─→ RelayCommand ────┼─→ ViewModel Methods
│  TextBox.TextChanged ──┐             │
│                        ├─→ Binding ──┼─→ Property Changed
│  ComboBox.SelectionChanged ──┐       │
│                              └──────→ Data Update
│  DatePicker.SelectedDateChanged │
│                                 │
└─────────────────────────────────┘
                    │
                    ▼
        ┌─ PropertyChanged Events ──┐
        │                           │
        │ OnPropertyChanged         │
        │ ObservableCollection      │
        │ PropertyChanged Event     │
        │                           │
        └─ Updates UI Binding ──────┘
```

---

## Binding Architecture

### Data Binding Types Used

#### 1. OneWay Binding (View receives updates)
```xaml
<TextBlock Text="{Binding StatusMessage}"/>
<!-- ViewModel → View -->
```

#### 2. TwoWay Binding (View ↔ ViewModel)
```xaml
<TextBox Text="{Binding PatientName, UpdateSourceTrigger=PropertyChanged}"/>
<!-- User types → Property updated → Validation triggered → UI updated -->
```

#### 3. Command Binding
```xaml
<Button Command="{Binding SubmitCommand}" Content="Submit"/>
<!-- Button click → Command.Execute → ViewModel method -->
```

#### 4. Collection Binding
```xaml
<DataGrid ItemsSource="{Binding Appointments}"/>
<!-- ObservableCollection → DataGrid auto-updates -->
```

---

## Validation Architecture

### Multi-Layer Validation Flow

```
┌─────────────────────────────────────────────┐
│           USER INPUT                         │
│    (TextBox, ComboBox, DatePicker)          │
│              ↓                               │
├──────────────────────────────────────────────┤
│        PROPERTY SETTER (ViewModel)           │
│  - SetProperty called                       │
│  - Field updated                            │
│  - PropertyChanged event raised             │
│              ↓                               │
├──────────────────────────────────────────────┤
│     VALIDATION METHOD TRIGGERED             │
│  - ValidateForm() called                    │
│  - All validation rules applied             │
│  - IsFormValid property updated             │
│              ↓                               │
├──────────────────────────────────────────────┤
│        BINDING UPDATE (View)                 │
│  - IsFormValid binding refreshed            │
│  - Button.IsEnabled updated                 │
│  - Visual feedback displayed                │
│              ↓                               │
│        UI STATE CHANGE                       │
│  - Submit button enabled/disabled            │
│  - Error messages shown                      │
│  - Status indicator updated                  │
└─────────────────────────────────────────────┘
```

---

## Command Architecture

### RelayCommand Execution Flow

```
User clicks Button
       ↓
WPF raises Command Binding
       ↓
RelayCommand.CanExecute() checked
       ├─ True → Continue
       └─ False → Button disabled, stop
       ↓
RelayCommand.Execute() called
       ↓
Action<object?> _execute invoked
       ↓
ViewModel method executed
       ↓
Data updated / Event raised
       ↓
PropertyChanged event triggers
       ↓
View updates via binding
```

---

## Navigation Architecture

### Multi-View Navigation Pattern

```
┌──────────────────────────────────────────┐
│            MainWindow                     │
│  ┌────────────────────────────────────┐  │
│  │  Header (Navigation Buttons)        │  │
│  │  [Appointments] [Book] [Doctors]    │  │
│  └────────────────────────────────────┘  │
│  ┌────────────────────────────────────┐  │
│  │  ContentControl                     │  │
│  │  Content={Binding CurrentViewModel} │  │
│  │                                     │  │
│  │  DataTemplate mapping:              │  │
│  │  AppointmentVM → AppointmentView   │  │
│  │  DoctorVM → DoctorView             │  │
│  │  BookAppointmentVM → BookView      │  │
│  │                                     │  │
│  │  [Current View Renders Here]        │  │
│  └────────────────────────────────────┘  │
│  ┌────────────────────────────────────┐  │
│  │  Footer                             │  │
│  └────────────────────────────────────┘  │
└──────────────────────────────────────────┘
```

### Navigation Flow

```
Button Clicked
       ↓
NavigateToXXXCommand executed
       ↓
MainViewModel.NavigateToXXX() called
       ↓
Appropriate ViewModel created/cached
       ↓
CurrentViewModel property updated
       ↓
PropertyChanged event raised
       ↓
ContentControl binding refreshed
       ↓
DataTemplate lookup performed
       ↓
Correct View instantiated
       ↓
New view displayed
```

---

## Resource Management

### Application Resources (App.xaml)

```xaml
┌─ Application.Resources ─────────┐
│                                  │
│  ├─ Colors                       │
│  │  ├─ PrimaryColor             │
│  │  ├─ SecondaryColor           │
│  │  ├─ AccentColor              │
│  │  ├─ SuccessColor             │
│  │  └─ DangerColor              │
│  │                               │
│  ├─ Brushes                      │
│  │  ├─ PrimaryBrush             │
│  │  ├─ SecondaryBrush           │
│  │  └─ ...                       │
│  │                               │
│  └─ Styles                       │
│     ├─ ButtonStyle              │
│     ├─ TextBoxStyle             │
│     ├─ ComboBoxStyle            │
│     └─ ...                       │
│                                  │
└──────────────────────────────────┘
```

---

## Performance Optimization

### 1. Lazy Loading ViewModels
```csharp
public MainViewModel()
{
    // ViewModels created only when needed
    _appointmentViewModel ??= new AppointmentViewModel();
    CurrentViewModel = _appointmentViewModel;
}
```

### 2. Efficient Filtering
```csharp
private void FilterAppointments()
{
    // Uses LINQ Where with case-insensitive search
    var filtered = _allAppointments.Where(a =>
        a.PatientName.Contains(_searchText, StringComparison.OrdinalIgnoreCase)
    ).ToList();
    // UI auto-updates via ObservableCollection
}
```

### 3. Virtual Scrolling
```xaml
<DataGrid ItemsSource="{Binding Appointments}"/>
<!-- WPF DataGrid has built-in virtualization -->
```

---

## Error Handling Strategy

### Try-Catch Implementation
```csharp
public void LoadAppointments()
{
    IsLoading = true;
    try
    {
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
```

### User Feedback
- **StatusMessage** property for error/success messages
- **IsLoading** property for loading indicators
- **IsFormValid** property for form validation feedback

---

## Extension Points

### Adding New Views

1. Create new ViewModel inheriting from BaseViewModel
2. Create new XAML View UserControl
3. Add DataTemplate to MainWindow.xaml
4. Add navigation command to MainViewModel
5. Add navigation button to MainWindow header

### Adding New Data Entities

1. Create new Model class
2. Create new ViewModel (optional)
3. Create new View/UserControl
4. Add to navigation structure

### Adding New Validation Rules

1. Add validation method to ViewModel
2. Call from property setter
3. Update IsFormValid property
4. UI automatically reflects changes

---

## Design Principles Applied

### SOLID Principles
- **S**ingle Responsibility: Each class has one job
- **O**pen/Closed: Easy to extend without modifying
- **L**iskov Substitution: ViewModels extend BaseViewModel
- **I**nterface Segregation: Small, focused interfaces
- **D**ependency Inversion: Bindings abstract dependencies

### Other Principles
- **DRY** (Don't Repeat Yourself): Base classes, global styles
- **KISS** (Keep It Simple, Stupid): Clear, straightforward code
- **YAGNI** (You Aren't Gonna Need It): No unnecessary features
- **Separation of Concerns**: Clear layer boundaries

---

## Testing Considerations

### Unit Testing Points
- Validation logic in ViewModels
- Filtering logic
- Status update logic
- Command execution

### Integration Testing Points
- Data binding
- Navigation between views
- Form submission end-to-end

### UI Testing Points
- Visual verification
- User interaction flows
- Data grid updates

---

## Deployment Architecture

```
AppointmentSystem.exe
    ├─ .NET Runtime (required)
    ├─ WPF Libraries (bundled)
    ├─ MVVM Toolkit (NuGet package)
    └─ All dependencies
```

### System Requirements
- Windows 10 or later
- .NET 8.0 Runtime
- ~100MB disk space
- No external database required (demo version)

---

## Security Considerations

### Current Implementation
- In-memory data only
- No authentication/authorization
- No network communication
- Local desktop application

### Future Security Enhancements
- Database with encrypted credentials
- User authentication
- Role-based authorization
- Input validation/sanitization
- Audit logging

---

## Scalability Architecture

### Current Limitations
- Sample data in memory
- Single-user desktop application
- No concurrent access handling

### Future Scalability Improvements
- Database backend (SQL Server, PostgreSQL)
- Web service API
- Multi-user support
- Concurrent appointment booking
- Real-time notifications
- Cache management

---

**This architecture demonstrates enterprise-level WPF application design with clear separation of concerns, extensibility, and maintainability.**
