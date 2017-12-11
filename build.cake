#addin "Cake.Azure"

//////////////////////////////////////////////////////////////////////
// EXAMPLES
//////////////////////////////////////////////////////////////////////
// PS> .\build.ps1
// PS> .\build.ps1 -Target Build

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = new Version(Argument("build_version", "0.0.0.0"));
var versionSuffix = Argument("version_suffix", "beta");

Information("Arguments:");
Information($"target:={target}");
Information($"configuration:={configuration}");
Information($"build_version:={version}");
Information($"version_suffix:={versionSuffix}");
Information("\n");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/ToyStorage/bin") + Directory(configuration);
var artifactDir = Directory("./artifacts");

//////////////////////////////////////////////////////////////////////
// SETUP/TEARDOWN
//////////////////////////////////////////////////////////////////////

IProcess azureStorageEmulatorProcess = null;

Teardown(context =>
{
	if(azureStorageEmulatorProcess != null)
	{
		azureStorageEmulatorProcess.Dispose();
	}
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    //CleanDirectory(buildDir);
    //CleanDirectory(artifactDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
	DotNetCoreRestore("./src/SimpleUptime.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
	Information($"Building version {version}");

	DotNetCoreBuild("./src/SimpleUptime.sln", new DotNetCoreBuildSettings
    {
        Configuration = configuration,
		ArgumentCustomization  = args => args.Append($"/p:VersionPrefix={version.ToString(3)} /m --no-restore")
    });
});

Task("Run-Unit-Tests")
    .Does(() =>
{
	DotNetCoreTest("./src/SimpleUptime.UnitTests", new DotNetCoreTestSettings
    {
        Configuration = configuration,
    });
});

Task("Run-Integration-Tests")
    //.IsDependentOn("End-StartAzureStorageEmulator")
    .Does(() =>
{
	DotNetCoreTest("./src/SimpleUptime.IntegrationTests", new DotNetCoreTestSettings
    {
        Configuration = configuration
    });
});

Task("Begin-StartAzureStorageEmulator")
    .Does(() =>
{
	var fileName = @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe";
	var processSettings = new ProcessSettings
	{
		Arguments = "start"
	};

	azureStorageEmulatorProcess = StartAndReturnProcess(fileName, processSettings);
});

Task("End-StartAzureStorageEmulator")
    .IsDependentOn("Begin-StartAzureStorageEmulator")
    .Does(() =>
{
	using(azureStorageEmulatorProcess)
	{
		azureStorageEmulatorProcess.WaitForExit();

		var exitCode = azureStorageEmulatorProcess.GetExitCode();
		var exitCodeMessage = $"Azure Storage Emulator Start exit code: {exitCode}";
		const int SuccessExitCode = 0;

		Information(exitCodeMessage);

		if(exitCode != SuccessExitCode)
		{
			throw new Exception(exitCodeMessage);
		}
	}
});

Task("Pack")
	.Does(() => 
{
	Information($"Packing version {version}");

    Information("Copy resource group to artifacts...");
	Information($@"{artifactDir}\SimpleUptime.ResourceGroup");
    CopyDirectory($@".\src\SimpleUptime.ResourceGroup\bin\{configuration}", $@".\{artifactDir}\SimpleUptime.ResourceGroup");
});

Task("Publish-ResourceGroup")
    .Does(() => 
{
    Information("Publishing Resource Group");

    AzureLogin("c4b5de22-cde4-4c12-8ccd-af20539b607f", "84a1aba8-ca0a-4b84-ab98-0823c39164a0", "LxkH4DEDgyGptj2s");
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
	.IsDependentOn("Begin-StartAzureStorageEmulator")
	.IsDependentOn("Build")
	.IsDependentOn("Run-Unit-Tests")
	.IsDependentOn("Run-Integration-Tests");
	//.IsDependentOn("Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);