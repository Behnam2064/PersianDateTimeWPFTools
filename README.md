# Persian calendar
## WPF calendar with support for Gregorian and solar calendars

A free Persian calendar with the ability to support the Gregorian calendar that can be used in WPF

## How to use?
Step 1: Add the following resources in the App.xaml file

```
<Application 
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary
                    Source="pack://application:,,,/PersianDateTimeWPFTools;component/Resources.xaml" />
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

## Controls
- Clock
- PersianCalendar
- PersianDatePicker
- PersianCalendarWithClock


## Dependency Properties
New Dependency Properties

<table>
<tr>
    <th>Name</th>
    <th>Description</th>
    <th>Default</th>
</tr>
<tr>
    <td>CustomCulture</td>
    <td>Selecting a different culture than the current software culture</td>
    <td>Based on the current software culture</td>
</tr>
<tr>
    <td>CustomCultureName</td>
    <td>Choosing a culture name that is different from the current software culture (such as fa-IR or en-US)</td>
    <td>Based on the current software culture</td>
</tr>
<tr>
    <td>AllowSelectBlackedOutDay</td>
    <td>Selectable holidays</td>
    <td>False</td>
</tr>
<tr>
    <td>ShowTodayButton</td>
    <td>Show a button to move to the current day</td>
    <td>False</td>
</tr>
</table>
