# Steps to configure this branch for research mode
- Build in ARM64
- Make sure HL2UnityPlugin dll and winmd files are present in `Assets/Plugins/WSA/ARM64` folder of unity project.
- Important: After building the visual studio solution from Unity, go to App/[Project name]/Package.appxmanifest and add the restricted capability to the manifest file. (Same as what you would do to enable research mode on HoloLens 1, reference: http://akihiro-document.azurewebsites.net/post/hololens_researchmode2/)

# FAQs