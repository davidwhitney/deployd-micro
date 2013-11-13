deployd-micro - continuous deployment for windows
=============

Deployment on Windows is harder than it should be.

Every Windows shop that manages its own servers does deployment their own way. Sometimes this involves an developer or ops staffer copying files to servers by hand. Other times it involves MSDeploy and pushing from a copy of Visual Studio. These processes are error prone, none repeatable and cause friction.

Some people script continuous deployments. Often leveraging tools like TeamCity, Jenkins or Go in conjunction with PowerShell scripts or batch files to coordinate an "automated xcopy". This is normally the sweet spot of getting people over the hump towards structured repeatable deployments. Building in this way though, often forces lots of your deployment knowledge to be tied up in scripts hidden in your CI server. You also end up with a signal/noise problem where your deployment files have a mix of responsibities, from building packages, physically copying files and then orchestrating an installation.

Deployd-micro is an attempt to build on these CI-based deployment systems by putting together a small suite of command line tools that can be chained together, removing all of the common tasks and friction from deployments. The project follows some core beliefs:

- Your source control should be the source of installation scripts and knowledge
- Packaging and transport are concerns that should be taken care of simply, using conventions
- Everyones environments are different, so the ability to configure tools in a non-perscriptive way is important
- In order to build confidence, you should be able to ease into a CD pipeline, rather than having to build it all at once and "bet the farm"

The aim of deployd is to take care of all the reusable bits.
Take care of packaging, take care of transport, take care of installation, so that every time, you only need to think about your application concerns.

What is it then?
=================

- deployd-micro is a suite of small applications that can be chained together into a deployment pipeline. 
- It has baked in conventions for run first, pre and post install hooks and it's built around NuGet and Zip file packages.

What isn't it?
==============

- deployd-micro isn't an environment provisioning system/framework. If you're looking for that, go look at Chef for Windows.

The tools
==========

- deployd => a command line tool that installs packages sources from it's configured feed.
- deployd.watchman => an optional system service installed side by side with deployd, offers a REST interface to trigger deployments and query a servers current state
- deployd-remote => an optional component that turns a command line call into a watchman API call to trigger deployments remotely
- deployd-package => a quick and dirty auto-packager that creates a NuGet package out of a directory
- deployd-mothership => an optional environment grouping and deployment app to trigger deployments visually and radiate status

The idea is that you can chain these tools into any configuration you wish.

Minimum barrier to entry
========================

- .NET 4.5 installed on your server
- A server with a copy of deployd and it's config file copied to it pointing at a package source.
- A package source: a folder full of NuGet or Zip files with names in the format MyApp.1.0.0.0.nupkg at the minimum (or a remote NuGet feed uri)
- Two hands to invoke deployd.exe on the command line.

When you invoke "deployd /app=MyApp /i", deployd will:

- Source the package from your package source
- By default unpack into an /Apps subdirectory
- It'll log the installation.
- If you repeat the install command, it'll create a versioned backup and re-install your software.

You can override the app installation directory and the package source from the configuration .json file.

Conventions
===========

Your apps will be unpacked into: ~/Apps/AppName
On install, version numbered backups are made in: ~/Apps/AppName/x.x.x.x

During install, the staged (unpacked, not-installed) version of your app will be searched for any files matching the following patterns:

- hook-first-install*
- hook-pre-install*
- hook-post-install*

All matching files found will be executed on the command line. Known extensions will be auto-executed in their appropriate environments presuming that you have all the information required to do this in your environment PATH.

An example:

hook-pre-install.rb will be executed as "ruby hook-pre-install.rb" on the command line.

All deployd directory paths are copied into environmental variables and made available if the script of your choice supports it. These variables start with "Deployd."

An end to end concept
=====================

Presume you have a copy of TeamCity that is running as a CI server

- On the target server, install deployd.watchman.
- On the target server, configure deployd to point to your package source, perhaps a network share.
- Optionally install deployd.mothership on a deployment server with the watchman installs set to autoregister.

- Add a build step at the end of your build calling deployd-package to package your asset.
- Add a build step to copy your package to your package location.
- Add a call to deployd-remote with the package name and target server name as command line parameters.

- You CI server, on pulling a code change, would then build your software, deployd would package it.
- Your CI server would then copy the asset somewhere.
- deployd-remote would ask watchman to invoke an install.
- deployd would pull the package from the published source and install.

Design decisions
================

- Each part of deployd is small and replaceable.
- Each part operates in conjunction with it's peers, but doesn't depend on them.
- deployd itself does a single locked install per instance.
- You must be able to execute any element of deployd by hand

Usage
=====

Packaging
---------
    deployd-package.exe --source c:\path\to\my\application --target c:\path\to\packages\folder

Deploying
---------

From a http NuGet feed:
    deployd.exe --install --app MyApplicationName--from http://url/of/nuget/feed --to d:\where\to\install

From a file-system NuGet feed:
    deployd.exe --install --app MyApplicationName --from \\path\to\my\nuget\feed --to d:\where\to\install

Deploy a specific version
    deployd.exe --install --app MyApplicationName --from http://url/of/nuget/feed --to d:\where\to\install --version 1.1.0.0

Prepare a package for installation. This will download the package if necessary and stage it for installation. The actual installation will be much quicker because only the activation step needs to be performed.
    deployd.exe --prepare --app MyApplicationName --from http://url/of/nuget/feed --to d:\where\to\install --version 1.1.0.0

Most commands have a short form:
    deployd.exe -i -a MyApplicationName -f http://url/of/nuget/feed -t d:\where\to\install -v 1.1.0.0
    deployd.exe -p -a MyApplicationName -f http://url/of/nuget/feed -t d:\where\to\install -v 1.1.0.0

You can also have deployd update all installed applications to their latest available version.
    deployd.exe --update -f http://url/of/nuget/feed -t d:\where\to\install

Help
----
Show a list of available commands:
    deployd.exe

See what packages are available from the source, what is currently installed (including 'staged' versions):
    deployd.exe --status --from http://url/of/nuget/feed --to d:\where\to\install

Configuration
-------------
You can set configuration values so that you don't have to specify them explicitly each time.

To set a default installation folder:

    deployd.exe --config InstallRoot c:\path\to\install\folder

To set a default package source
    deployd.exe --config PackageSource http://url/of/package/source

Using deployd remotely
======================
We provide a Powershell module which makes it simple to install applications to remote machines.
The module uses PS Remoting so you need to have enabled this on the machine you're installing to. You also need to have permissions to run deployd and write to files and folders in the target installation folder.

Installing the Module
---------------------
To import the module: 

1. Copy the PowershellModules/Deployd folder into c:\windows\system32\WindowsPowershell\Modules

2. Run the following command in a Powershell console:

    import-module PowerShellModules\Deployd -Force -Verbose

Running deployd remotely
------------------------
Using the deployd Powershell module you can specify any number of target computers and applications to install. Each application specified will be installed on each computer specified. A log file will be created for each target computer named [computername].log.

To install a single application to a single computer:
    Install-DeploydApplications -Environment Staging -Computers computer1 -Applications "MyApplication" -PackageSource http://url/of/package/source -InstallPath c:\install\path\on\remote\computer

You can specify multiple computers in comma-delimited format:
    Install-DeploydApplications -Environment Staging -Computers computer1,computer2,computer3 -Applications "MyApplication" -PackageSource http://url/of/package/source -InstallPath c:\install\path\on\remote\computer

You can also specify multiple applications in comma-delimited format. Each application will be installed on each computer.
    Install-DeploydApplications -Environment Staging -Computers computer1,computer2,computer3 -Applications "MyApplication1,myApplication2,MyApplication3" -PackageSource http://url/of/package/source -InstallPath c:\install\path\on\remote\computer

If you need to install an application in a specific folder you can do this in the following way:
    Install-DeploydApplications -Environment Staging -Computers computer1,computer2,computer3 -Applications "MyApplication1|c:\specific\install\path,myApplication2,MyApplication3" -PackageSource http://url/of/package/source -InstallPath c:\install\path\on\remote\computer

Update all applications on remote computers
    Update-DeploydApplications -Environment Staging -Computers computer1,computer2,computer3 -PackageSource http://url/of/package/source -InstallPath c:\install\path\on\remote\computer
