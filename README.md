# Virtual-Augmentation
A project about facial recognition, real-time heads-up-displays, and social media cross referencing.

## Index
- [Poster](#poster)
- [Code Compiling](#compilation)
- [Building](#building)

## Poster

![Project Poster](https://github.com/pdemange/Virtual-Augmentation/raw/master/Poster.png "Project Poster")

## Compiling & Running detect.cpp 
### Compilation
```bash
$ g++ detect.cpp -o detect.cpp `pkg-config --cflags --libs opencv` -fopenmp -lpthread
```
### Running
```bash
$ ./detect --helpo
```

## Building for the HoloLens
### Build Requirements
 - Fast network connection: you need to download around 100GB of stuff to build for the HoloLens
 - CPU capable of running Hyper-V for HoloLens emulator, or a real HoloLens

 - Windows 10
    - Must be Pro or Enterprise version
    - Build number 10.0.16299 or newer
    - Yes, it is very particular about having this version
 - Unity Editor Version 2017.3.1f1
    - Make sure to install the Universal Windows Platform build support
    - You can also install Visual Studio from here. If you don't you can install it separately.
    - Unity is less particular about the version than Windows and Visual Studio so you might be able to get away with something else for this.
 - Visual Studio 2017
    - You can install this through the Unity Editor installer
    - After installation, open the "Visual Studio Installer" program and also install the "Universal Windows Platform Development" package
 - Optional: HoloLens Emulator
    - If you don't have a real HoloLens you can use Microsoft's emulator
    - For this, you must also install Visual Studio 2015 with the Universal Windows Platform package
    - You need to have VS2015 installed for the HoloLens Emulator to install, but you can still use VS2017 to actually do the building. You might be able to uninstall VS2015 once the emulator is installed but I haven't tried it.
    - The emulator uses Hyper-V and Hyper-V has some weird requirements. It does not work on all CPUs that support virtualization
    - I had trouble because my CPU supported virtualization but not SLAT
    - See https://docs.microsoft.com/en-us/virtualization/hyper-v-on-windows/reference/hyper-v-requirements
###Build Procedure
1. Install everything listed above
1. Clone this repo
1. Open the HoloBuildTest directory as a project in Unity Editor
1. File → Build Settings...
1. For Target Device, select "HoloLens"
1. For Build Type, select "D3D"
1. For Build and Run On select "Local Machine"
1. Optional: if you want to be able to edit the scripts without rebuilding from Unity, check "Copy references" and "Unity C# Scripts"
1. Click "Build..."
1. Create a new directory and select it for the destination. DO NOT build into the project directory itself -- it will overwrite the Visual Studio solution for the source code! (a sub-directory is OK)
1. Unity will open a Windows Explorer in the project directory when it is done building. Open the folder you built into and then open the .sln file in Visual Studio
1. From the toolbar, select "Release" and "x86"
1. Click the dropdown from the run button:
    - Select "HoloLens Emulator" if you want to run it in the emulator
    - Select "Device" to deploy to the HoloLens over USB
    - Select "Remote Device" to deploy to the HoloLens over Wi-Fi
1. Click the run button
1. Visual studio will build the project and then fail. Click the run button again and it will successfully deploy to the HoloLens or emulator (yeah idk)

###Building to the real HoloLens
To build for the real HoloLens, you need to put it into Developer Mode. On the HoloLens:
1. Start → Settings → Update & Security → For developers
1. Turn Developer mode on
1. It's also handy to turn Device Portal on
