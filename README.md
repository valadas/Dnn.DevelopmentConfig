# Dnn.DevelopmentConfig

## Step 1
Fork the following projects. 
* [Dnn.Platform](https://github.com/dnnsoftware/Dnn.Platform)
* [Dnn.AdminExperience](https://github.com/dnnsoftware/Dnn.AdminExperience)
* [ClientDependency](https://github.com/dnnsoftware/ClientDependency)
* [CKEditorProvider](https://github.com/DNN-Connect/CKEditorProvider/)

## Step 2
Clone this project to your workstation. Any directory will do. It just has to be empty. 

## Step 3
Open up the Powershell CLI into the directory you created in step 2 and run the following.

`.\build.ps1 -ScriptArgs '--user="YOUR-GITHUB-ACCOUNT"'`

This will clone each of the required projects under your github account. 

## Step 4
Next you need to compile the Platform source and generate Nuget Packages.

`.\Platform\build.ps1 -Target CreateNugetPackages`

## Step 5

`.\build.ps1 -Target SetupReferences`
