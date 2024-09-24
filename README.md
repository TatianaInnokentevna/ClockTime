Unity Clock Project
This is a Unity project that implements an animated, working clock using DOTween for smooth hand transitions and TMP_InputField for user input to set the clock manually. The clock synchronizes with the current time fetched from an external server (Yandex) and allows users to set their own time via a user interface.

Features
Analog Clock: The clock features hour, minute, and second hands that move in real-time, with animations powered by DOTween.
Time Synchronization: The clock fetches the current time from the Yandex time API and sets the clock hands accordingly.
Manual Time Setting: Users can manually set the time through input fields for hours, minutes, and seconds. Any invalid input (like 25 hours or 61 minutes) will be ignored or handled appropriately.
Smooth Animations: Clock hands move smoothly between time changes using DOTween.
Installation
Unity Version: This project uses Unity version 2022.x or higher.

Packages Required:

DOTween: A popular Unity plugin for tweening animations.
TextMesh Pro: For input fields and text rendering.
Make sure to install DOTween through the Unity Package Manager or directly via the Asset Store.

How to Use
Clone this repository or download the project files.
Open the project in Unity.
In the scene, you will find the clock GameObject with the following components:
Hour Hand (PivotHour)
Minute Hand (PivotMin)
Second Hand (PivotSec)
Canvas with Input Fields for entering hours, minutes, and seconds.
Time Synchronization
On start, the clock will automatically sync with the Yandex time server to get the current time. If the time cannot be fetched, it will default to the system’s local time.

Manual Time Setting
In the UI, users will see input fields labeled "Hours", "Minutes", and "Seconds".
After entering the desired time values, click the "Change Time" button to update the clock.
Input validation is in place to ensure proper values (e.g., no more than 24 hours or 60 minutes).
Key Scripts
Clock.cs
This script controls the clock’s functionality. It contains methods for:

Fetching the time from the server and synchronizing the clock.
Animating the hands of the clock using DOTween.
Allowing the user to manually set the time.
Key methods:

GetTimeFromServer(): Fetches time from Yandex and updates the clock.
UpdateClockHands(): Updates the angles of the clock hands based on the current time.
SetTimeManually(int hour, int minute, int second): Allows the user to set the time manually.
Input Handling
The project uses TMP_InputField for collecting user input for hours, minutes, and seconds. The input fields are set to clear their placeholder text when clicked on.

Time Validation
Input values for hours, minutes, and seconds are validated to ensure they are within acceptable ranges. If invalid values are entered, the clock will ignore them or reset to a default valid time.

Known Issues
Placeholder text may sometimes overlap with the input field if focus is not properly handled. Make sure to activate input fields using ActivateInputField() to avoid this.
The clock syncs only once at the start. Future versions may include continuous time syncing after regular intervals.
Future Improvements
Continuous Time Sync: Implement periodic server synchronization to keep the clock updated.
Timezone Support: Allow users to choose different timezones.
Custom Skins: Add customization options for the clock face and hands.
Contributing
Feel free to fork this repository and submit pull requests for any improvements, bug fixes, or new features.

License
This project is open-source and free to use under the MIT License.

Contact
For any questions or issues, feel free to open an issue on GitHub or reach out via telegram @tatuhka94
