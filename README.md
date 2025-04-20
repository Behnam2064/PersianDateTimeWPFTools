# Persian calendar
## WPF calendar with support for Gregorian and solar calendars

- A free Persian calendar with the ability to support the Gregorian calendar that can be used in WPF
- You can write any style you like for the controls.
- Language change support and other resources

Please see the test project for further guidance.

## Controls
- Clock

![IMAGE_DESCRIPTION](https://raw.githubusercontent.com/Behnam2064/PersianDateTimeWPFTools/refs/heads/main/clock.png)
- PersianCalendar
![IMAGE_DESCRIPTION](https://raw.githubusercontent.com/Behnam2064/PersianDateTimeWPFTools/refs/heads/main/pc.png)
- PersianDatePicker
![IMAGE_DESCRIPTION](https://raw.githubusercontent.com/Behnam2064/PersianDateTimeWPFTools/refs/heads/main/pdp.png)
- PersianDateTimePicker
![IMAGE_DESCRIPTION](https://raw.githubusercontent.com/Behnam2064/PersianDateTimeWPFTools/refs/heads/main/pdpwc.png)
- PersianCalendarWithClock
![IMAGE_DESCRIPTION](https://raw.githubusercontent.com/Behnam2064/PersianDateTimeWPFTools/refs/heads/main/pdtp.png)

You can manually change the Culture of the control using the following code in XAML or C#.
**XAML**
```
CustomCultureName="fa-IR"
```
or
**C#**
```
pcwc1.CustomCulture = CultureInfo.CreateSpecificCulture("en-US");
```
If you do not select Culture, it will be automatically selected based on the software's Culture.

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
![IMAGE_DESCRIPTION](https://raw.githubusercontent.com/Behnam2064/PersianDateTimeWPFTools/refs/heads/main/PersianDateTimeWPFTools/TestPersianCalendar/assets/Persian-calendar-Custom-Style.png)


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


## How to change the theme
In the **App.xaml** file, you can select one of the following themes by selecting the **SelectedTheme** property in the InitResources class.
- Default
- DarkModern1
- LightModern1
```
<Application 
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:pdtt="https://github.com/Behnam2064/PersianDateTimeWPFTools"
             >
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <pdtt:InitResources
                    SelectedTheme="LightModern1"
                />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

## How to change theme in C#
You can do the following in the constructor of the App.xaml.cs class:
```
public partial class App : Application
    {        
        public App()
        {
            var init = new InitResources();
            init.SelectedTheme = PersianDateTimeWPFTools.Themes.BaseThemeName.DarkModern1;
        }
    }
```
or
```
public partial class App : Application
    {        
        public App()
        {
            InitResources.SetTheme(new ThemeDarkModern1());
        }
    }
```
## How to change the control language?
```
public partial class App : Application
    {        
        public App()
        {
            new InitResources()
            .ChangeLanguage("fa"); // en
        }
    }
```

## How to change the language of controls with our own resources
To read the language resources, please visit the link below.
[Github Link](https://github.com/Behnam2064/PersianDateTimeWPFTools/blob/main/PersianDateTimeWPFTools/PersianDateTimeWPFTools/Resources/Lang/Lang.en.xaml)
```
public partial class App : Application
    {        
        public App()
        {
            new InitResources()
            .ChangeLanguage(null, "pack://application:,,,/TestPersianCalendar;component/Lang.es.xaml"); // Your resource address
        }
    }
```