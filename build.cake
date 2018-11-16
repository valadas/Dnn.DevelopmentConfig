#addin "Cake.Git"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var githubUser = Argument("user", "dnnsoftware");


//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var ownerOfCkEditor = githubUser == "dnnsoftware" ? "DNN-Connect" : githubUser;

var platformDir = Directory("./Platform/");
var adminExpDir = Directory("./Admin-Experience/");
var cdfDir = Directory("./Client-Dependancy/");
var ckDir = Directory("./CkEditor-Provider/");

var artifactDir = Directory("./Artifacts/");
var artifactFullPath = System.IO.Path.Combine(System.IO.Path.GetFullPath("./"), platformDir.ToString(), "Artifacts");

var buildDirFullPath = System.IO.Path.GetFullPath("./") + "\\";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
	{
		CleanDirectory(platformDir);
		CleanDirectory(adminExpDir);
        CleanDirectory(cdfDir);
        CleanDirectory(ckDir);
	});

Task("SetupDevEnvironment")
    .Does(() =>
	{
        GitClone("https://github.com/" + githubUser + "/Dnn.Platform.git", platformDir);
        GitClone("https://github.com/" + githubUser + "/Dnn.AdminExperience.git", adminExpDir);
        GitClone("https://github.com/" + githubUser + "/ClientDependency.git", cdfDir);
        GitClone("https://github.com/" + ownerOfCkEditor + "/CKEditorProvider.git", ckDir);
	});
    
Task("NugetInstall")
    .Does(() =>
	{
        NuGetInstall("DotNetNuke.Bundle", new NuGetInstallSettings {
            ExcludeVersion  = false,
            Prerelease  = true,
            OutputDirectory = adminExpDir.ToString() + "/packages",
            Source = new[] { artifactFullPath, "https://api.nuget.org/v3/index.json", "https://www.myget.org/F/dnn-software-public/api/v2" }
        });
        
        
	});
    
Task("NugetUpdate")
    .Does(() =>
	{
        NuGetUpdate(System.IO.Path.Combine(adminExpDir, "Dnn.AdminExperience.sln"), new NuGetUpdateSettings {
            Prerelease  = true,
            Source = new[] { artifactFullPath, "https://api.nuget.org/v3/index.json", "https://www.myget.org/F/dnn-software-public/api/v2" }
        });
	});
    
    
Task("BuildAll")
    .Does(() =>
	{
        //##### Build the AdminExperience project
		var externalSolutions = GetFiles(adminExpDir.ToString() + "/*.sln");
		Information("Found {0} solutions.", externalSolutions.Count);
        BuildSolution(externalSolutions.First().ToString());
        
        //##### Build the ClientDependency project
        externalSolutions = GetFiles(cdfDir.ToString() + "/ClientDependency.DNN.sln");
		Information("Found {0} solutions.", externalSolutions.Count);
        BuildSolution(externalSolutions.First().ToString());
        
        //##### Build the CKEditor project
        externalSolutions = GetFiles(ckDir.ToString() + "/*.sln");
		Information("Found {0} solutions.", externalSolutions.Count);
        BuildSolution(externalSolutions.First().ToString());
	});
        
//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("SetupDevEnvironment");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);


public bool BuildSolution(string pathToSolution)
{
    try
    {
        var solutionPath = pathToSolution.ToString();
    
        Information("Processing Solution File: {0}", solutionPath);
        Information("Starting NuGetRestore: {0}", solutionPath);
        NuGetRestore(solutionPath);

        Information("Starting to Build: {0}", solutionPath);
        MSBuild(solutionPath, settings => settings.SetConfiguration(configuration));

    }
    catch (Exception err){
		Error(err);

        return false;
    }
    
    return true;
}
