## File Service Plugin for Xamarin and Windows
[FileService-Plugin](FileService-Plugin.md)
>
> Library version 2.0 has been published. This nuget contains .Net Standard version of this FileService logic.
> New code at src.NetStandard.
> NugetID -> [BiT21.FileService](https://www.nuget.org/packages/BiT21.FileService/) [![NuGet](https://img.shields.io/nuget/v/Xam.Plugin.DeviceInfo.svg?label=NuGet)](https://www.nuget.org/packages/Bit21.FileService/)
>
> Pending to update this README to reference the new code.
>
A simple way to create a file system sandbox where your aplication will be able to save information, Text, Objects or Byte[].

### Setup
* Available on NuGet: [BiT21.FileService](https://www.nuget.org/packages/BiT21.FileService/) [![NuGet](https://img.shields.io/nuget/v/Xam.Plugin.DeviceInfo.svg?label=NuGet)](https://www.nuget.org/packages/Bit21.FileService/)
* Install into your PCL/netstandard project and Client projects.

**Platform Support**

|Platform|
| ------------------- | 
NetStandard 2.0

### API Usage
On version 3.0 we have provided several constructors that allows to chose the target folder where FileService will set his sandbox so save files and folders for the application.

```cs
/// <summary>
/// ctor
/// </summary>
/// <param name="sandboxTag"></param>
/// <remarks>The default SpecialFolder is <see cref="System.Environment.SpecialFolder.LocalApplicationData"/>remarks>
public FileServiceImplementation(string sandboxTag) 

/// <summary>
/// ctor
/// </summary>
/// <param name="sandboxTag">Name of the sandbox for this instance.</param>
/// <param name="specialFolder">Root Environment folder where to set the root sandbox folder</param>
public FileServiceImplementation(string sandboxTag, System.Environment.SpecialFolder specialFolder) 
```

Therefore the initialization will stand:
```csharp
IFileService fileService = new FileServiceImplementation(SANDBOX_TAG);
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
Planning to extend service with

* Encrypted storage. On Test proyect you have a sample to use FileService in combination wth BiT21.EncryptDecrypt to protect file data.


#### Contributions
Contributions are welcome! If you find a bug please report it and if you want a feature please report it.

If you want to contribute code please file an issue and create a branch off of the current dev branch and file a pull request.

#### License
Under MIT, see LICENSE file.
