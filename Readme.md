# Steps to configure this branch for research mode
- Build in ARM64
- Make sure HL2UnityPlugin dll and winmd files are present in `Assets/Plugins/WSA/ARM64` folder of unity project.
- Important: After building the visual studio solution from Unity, go to App/[Project name]/Package.appxmanifest and add the restricted capability to the manifest file.
```
<Package 
xmlns:mp="http://schemas.microsoft.com/appx/2014/phone/manifest" 
xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10" 
xmlns:uap2="http://schemas.microsoft.com/appx/manifest/uap/windows10/2" 
xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities" //(this line)
IgnorableNamespaces="uap uap2 mp rescap" //(rescap in this line)
xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
>
<Capabilities>
    <Capability Name="internetClient" />
    <rescap:Capability Name="perceptionSensorsExperimental" /> //(this line)
    <DeviceCapability Name="webcam" />
</Capabilities>
```
- There is a bug in research mode AHAt depth capturing in hololens2, because of which sometimes Ahat depth are not captured. To resolve this make sure you hololens does not go to sleep, or restart the device before running the app.

# FAQs
- Point clouds not getting saved/rendered
-- If no point clouds are getting saved, 1. check configuration steps from above, 2. Restart hololens
- Point clouds not coming in screenshots or video capture
-- Ensure while building in Unity, player settings has a field Stereo Rendering, which should be set to multi pass
- In general to record reserach mode data, keep the game object as well as main camera at origin.