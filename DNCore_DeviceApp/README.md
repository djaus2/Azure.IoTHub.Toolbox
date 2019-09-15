# DNCore_DeviceApp

This is a .NET Core app to run on an IoT Device. It connects to an Azure IoT Hub as the device end.

- Edit settings.json to match the IoT Hub connection settings.
- Copy the required version  to a folder on the target (The Toolbox can do this to a File Share).
- Open a (remote) command prompt to that location and run the .exe

## For the Toolbox:
- Create folder Properties off this app's root and Unzip PublishProfiles.zip into it.
- Build the Publish profiles and copy folders (except Assets) in *This project root*\bin\Release\netcoreapp2.2 to <br>
*Toolbox root*(\Devices.
- The published builds will then be listed on the Deploy page of the Toolbox
