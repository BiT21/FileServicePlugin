## File Service Plugin for Xamarin and Windows

A simple way to create a file system sandbox where your aplication will be able to save information, Text, Objects or Byte[].

### Setup
* Available on NuGet: https://www.nuget.org/packages/BiT21.Xam.Plugin.FileService/ [![NuGet](https://img.shields.io/nuget/v/Xam.Plugin.DeviceInfo.svg?label=NuGet)](https://www.nuget.org/packages/TBD/)
* Install into your PCL/netstandard project and Client projects.

**Platform Support**

|Platform|TFM|Version|
| ------------------- | ------------------: | ------------------: |
|Xamarin.Android|MonoAndroid10|API 10+|
|Xamarin.iOS|Xamarin.iOS10|iOS 7+|
|Windows 10 UWP|UAP10|10+|
|.NetFramework|net45|4.5+

### API Usage
To gain access to the FileService class use this methord.
```csharp
var f = CrossFileService.Current;
```
Before macking any call to FileService we need to specify the sandboxtag, that will define the sandbox we will be targeting the class calls.
```csharp
f.SandboxTag = "MyAplicationSandboxTag";
```
This tag translates to a folder on the device FileSystem. 

The original design stands that the application will be working with a single sandbox. This will change in future releases.

Thefore the initialization will stand:
```csharp
IFileService fileService = FileService.CrossFileService.Current;
fileService.SandboxTag = SANDBOX_TAG;
string content = "This is the content I need to save in a text file";

//Create file and save content.
await fileService.SaveTextFileAsync(content, filename);

//Read content from file
string text = await fileService.ReadTextFileAsync(filename);
Assert.AreEqual(content, text);

//Delete file from sandbox.
await fileService.DeleteFileAsync(filename);
Assert.IsFalse(await fileService.ExistFileAsync(filename));

//Delete sandbox
await fileService.DeleteSandboxAsync();
Assert.IsFalse(await fileService.ExistSandBoxAsync());
```

#### Roadmap
Planning to extend the plugin with

* Multi sandbox
* Encrypted storage. 

### Platform specifics
Sandbox will be created 

|Platform|Path|
| ------------------- | :------------------ |
iOS         |Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);|
Xamarin.Android     |Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);|
Windows_UWP |Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;|
 
#### Contributions
Contributions are welcome! If you find a bug please report it and if you want a feature please report it.

If you want to contribute code please file an issue and create a branch off of the current dev branch and file a pull request.

#### License
Under MIT, see LICENSE file.
