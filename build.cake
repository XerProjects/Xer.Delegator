///////////////////////////////////////////////////////////////////////////////
// ADDINS/TOOLS
///////////////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutions = GetFiles("./**/*.sln");
var projects = GetFiles("./**/*.csproj").Select(x => x.GetDirectory());
var myGetFeed = EnvironmentVariable("MYGET_SOURCE");
var myGetApiKey = EnvironmentVariable("MYGET_API_KEY");
var nuGetFeed = EnvironmentVariable("NUGET_SOURCE");
var nuGetApiKey = EnvironmentVariable("NUGET_API_KEY");

GitVersion gitVersion;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    gitVersion = GitVersion(new GitVersionSettings {
        UpdateAssemblyInfo = true
    });
    
    // Executed BEFORE the first task.
    Information("Xer.Delegator");
    Information("Parameters");
    Information("///////////////////////////////////////////////////////////////////////////////");
    Information("Branch: {0}", gitVersion.BranchName);
    Information("Version semver: {0}", gitVersion.LegacySemVerPadded);
    Information("Version assembly: {0}", gitVersion.MajorMinorPatch);
    Information("Version informational: {0}", gitVersion.InformationalVersion);
    Information("///////////////////////////////////////////////////////////////////////////////");
});

Teardown(context =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Cleans all directories that are used during the build process.")
    .Does(() =>
{
    // Clean solution directories.
    foreach(var project in projects)
    {
        Information("Cleaning {0}", project);
        DotNetCoreClean(project.FullPath);
    }
});

Task("Restore")
    .Description("Restores all the NuGet packages that are used by the specified solution.")
    .Does(() =>
{
    var settings = new DotNetCoreRestoreSettings
    {
        ArgumentCustomization = args => args
            .Append("/p:Version={0}", gitVersion.LegacySemVerPadded)
            .Append("/p:AssemblyVersion={0}", gitVersion.MajorMinorPatch)
            .Append("/p:FileVersion={0}", gitVersion.MajorMinorPatch)
            .Append("/p:AssemblyInformationalVersion={0}", gitVersion.InformationalVersion)
    };

    // Restore all NuGet packages.
    foreach(var solution in solutions)
    {
        Information("Restoring {0}...", solution);
      
        DotNetCoreRestore(solution.FullPath, settings);
    }
});

Task("Build")
    .Description("Builds all the different parts of the project.")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        ArgumentCustomization = args => args
            .Append("/p:Version={0}", gitVersion.LegacySemVerPadded)
            .Append("/p:AssemblyVersion={0}", gitVersion.MajorMinorPatch)
            .Append("/p:FileVersion={0}", gitVersion.MajorMinorPatch)
            .Append("/p:AssemblyInformationalVersion={0}", gitVersion.InformationalVersion)
    };

    // Build all solutions.
    foreach(var solution in solutions)
    {
        Information("Building {0}", solution);
        
        DotNetCoreBuild(solution.FullPath, settings);
    }
});

Task("Test")
    .Description("Execute all unit test projects.")
    .IsDependentOn("Build")
    .Does(() =>
{
    var projects = GetFiles("./Tests/**/*.Tests.csproj");
    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
    };

    foreach(var project in projects)
    {
        DotNetCoreTest(project.FullPath, settings);
    }
});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .Description("This is the default task which will be ran if no specific target is passed in.")
    .IsDependentOn("Test");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);