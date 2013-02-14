deployd-micro
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

Attempt 2 at a smaller, paired down version of the deployd concept.