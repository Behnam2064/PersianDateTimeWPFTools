# Persian calendar
## WPF calendar with support for Gregorian and solar calendars

- A free Persian calendar with the ability to support the Gregorian calendar that can be used in WPF
- You can write any style you like for the controls.
- Language change support and other resources

Please see the test project for further guidance.

## Controls
- Clock
- PersianCalendar
- PersianDatePicker
- PersianDateTimePicker
- PersianCalendarWithClock

![IMAGE_DESCRIPTION](https://github.com/Behnam2064/PersianDateTimeWPFTools/blob/main/PersianDateTimeWPFTools/TestPersianCalendar/Controls.png)

## How to use?
Step 1: Add the following resources in the App.xaml file

```
<Application 
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:pdtt="https://github.com/Behnam2064/PersianDateTimeWPFTools"
             >
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <pdtt:InitResources />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

Step 2:
Use in XAML (WPF) files
You can use below namespace in xaml (wpf)

xmlns:pdtt="https://github.com/Behnam2064/PersianDateTimeWPFTools"


Sample:
```
<Window 
    xmlns:pdtt="https://github.com/Behnam2064/PersianDateTimeWPFTools">
    <Grid>
        <pdtt:PersianCalendar />
    </Grid>
</Window>
```


## Ability to write custom styles for all controls
![IMAGE_DESCRIPTION](https://github.com/Behnam2064/PersianDateTimeWPFTools/blob/main/PersianDateTimeWPFTools/TestPersianCalendar/Persian-calendar-Custom-Style.png)


## Dependency Properties
New Dependency Properties

| Name        | Description           | Default  |
| :------------- |:-------------| :-----|
| CustomCulture      | Selecting a different culture than the current software culture | Based on the current software culture |
| CustomCultureName     | Choosing a culture name that is different from the current software culture (such as fa-IR or en-US)      |   Based on the current software culture |
| AllowSelectBlackedOutDay | Selectable holidays      |    False |
| ShowTodayButton | Show a button to move to the current day   |    False |
| ShowConfirmButton | Show a button to display confirm button   |    False |
| DisplayDate | Displays the current date   |    ? |
| DisplayDateStart | Start displaying the date   |    ? |
| DisplayDateEnd | End of displaying date   |    ? |
| DisplayMode | Date display type (like Month,Year,Decade)   |    Month |
| FirstDayOfWeek | First day of the week  |    Sunday |
| SelectionMode | Type of selection  |    SingleDate |
| IsTodayHighlighted | Show current day as highlights  |    True |