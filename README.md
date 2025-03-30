# Persian calendar
## WPF calendar with support for Gregorian and solar calendars

A free Persian calendar with the ability to support the Gregorian calendar that can be used in WPF

## How to use?
Step 1: Add the following resources in the App.xaml file

```
<Application 
             ...
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             ...
             >
    <Application.Resources>
        ...
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary
                    Source="pack://application:,,,/PersianDateTimeWPFTools;component/Resources.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
    ..
</Application>
```

Step 2:
Use in XAML (WPF) files
You can use below namespace in xaml (wpf)

xmlns:pdtt="https://github.com/Behnam2064/PersianDateTimeWPFTools"


Sample:
```
<Window ...
    xmlns:pdtt="https://github.com/Behnam2064/PersianDateTimeWPFTools"
    ...>
    <Grid>...
        <pc:PersianCalendar />
    ...</Grid>
</Window>
```

## Dependency Properties
New Dependency Properties
| Name | Description | Default |
| ------ | ------ | ------ |
| CustomCulture | Selecting a different culture than the current software culture | Based on the current software culture |
| CustomCultureName | Choosing a culture name that is different from the current software culture (such as fa-IR or en-US) | Based on the current software culture |
| AllowSelectBlackedOutDay | Selectable holidays | False |
| ShowTodayButton | Show a button to move to the current day | False |