<#
Deployd module



#>


function Install-DeploydApplications(
[parameter(Mandatory=$true,ValueFromPipeLine=$true)][string]$Computers,
[parameter(Mandatory=$true)][string]$Environment,
[parameter(Mandatory=$true,HelpMessage="Names of applications to install separated by commas. Specify installation path by adding |path e.g: GG.Web.Website|d:\wwwroot")][string]$Applications,
[string]$ApplicationVersion,
[switch]$Prepare=$false,
[switch]$ForceDownload=$false,
[switch]$ForceUnpack=$false,
[string]$PackageSource="") 
{
    $jobs = @()
    $sessions = @()

    $installScriptBlock = [scriptblock]{
                param([string]$Environment,[string]$Applications,[string]$ApplicationVersion,[bool]$Prepare,[bool]$ForceDownload,[bool]$ForceUnpack,[string]$PackageSource)
                $Applications.split(",") | ForEach {
                    $appName = $_
                    $installPath=""

                    if ($appName.Contains("|"))
                    {
                        $appNameSplit=$appName.Split("|")

                        $appName=$appNameSplit[0]
                        $installPath=$appNameSplit[1]

                    }
                    $command =$("deployd -e "+$Environment+" --app "+$appName)

                    if ($installPath -ne "")
                    {
                        $command += " --to $installPath"
                    }
                    if ($ApplicationVersion)
                    {
                        $command += " --version $ApplicationVersion"
                    }
                    if ($Prepare -eq $true)
                    {
                        $command += " -p";
                    } else {
                        $command += " -i";
                    }
                    if ($ForceDownload -eq $true)
                    {
                        $command += " -fd";
                    }
                    if ($ForceUnpack -eq $true)
                    {
                        $command += " -fu";
                    }
                    if ($PackageSource -ne "")
                    {
                        $command += " --from $PackageSource"
                    }

                    iex $command
                }
            };

    Execute-Jobs -Computers $Computers -Environment $Environment -ScriptBlock $installScriptBlock -ArgumentList $Environment,$Applications,$ApplicationVersion,$Prepare,$ForceDownload,$ForceUnpack,$PackageSource
}

function Update-DeploydApplications(
[parameter(Mandatory=$true,ValueFromPipeLine=$true)][string]$Computers,
[parameter(Mandatory=$true)][string]$Environment,
[string]$InstallPath,
[switch]$Prepare=$false,
[switch]$ForceDownload=$false,
[switch]$ForceUnpack=$false) 
{
    $jobs = @()
    $sessions = @()

    $updateScriptBlock = [scriptblock]{
                param([parameter(Mandatory=$true)][string]$Environment,[bool]$Prepare,[bool]$ForceDownload,[bool]$ForceUnpack)
                $command =$("deployd -u -e "+$Environment)
                if ($InstallPath)
                {
                    $command += " --to $InstallPath"
                }
                if ($Prepare -eq $true)
                {
                    $command += " -p";
                }
                if ($ForceDownload -eq $true)
                    {
                        $command += " -fd";
                    }
                    if ($ForceUnpack -eq $true)
                    {
                        $command += " -fu";
                    }
                iex $command
            };

    Execute-Jobs -Computers $Computers -Environment $Environment -ScriptBlock $updateScriptBlock -ArgumentList $Environment,$Prepare,$ForceDownload,$ForceUnpack
}

function Execute-Jobs([string]$Computers,[string]$Environment,[scriptblock]$ScriptBlock,$ArgumentList)
{
    $Computers.Split(",") | ForEach {
        $session = New-PSSession -ComputerName $_

        $sessions += $session

        $job = Invoke-Command -ScriptBlock $ScriptBlock -Session $session -AsJob -JobName $_ -ArgumentList $ArgumentList

        Register-TemporaryEvent $job StateChanged -Action {
            Write-Host "$($sender.Name): $($sender.State)"
        }

        $jobs += $job
        Write-Host "Created installation job on" $_
    }

    Write-Host "Waiting for all installations to complete..."
    $jobs | Wait-Job

    $jobs | ForEach {
        Receive-Job -Job $_ | Out-File $($_.Name+".log")
        $("Took " + $($_.PSEndTime - $_.PSBeginTime)) | Out-File $($_.Name+".log") -Append
    }

    $sessions | ForEach {
        Remove-PSSession $_
    }
}

function Create-InstallJob([string]$Environment, [string]$AppsToInstall) {
    return [scriptblock]{
            param($Environment,$AppsToInstall)
            $AppsToInstall.split(",") | ForEach {
                $command =$("deployd -i -e "+$Environment+" --app "+$_)
                iex $command
                New-Event "$_ installed"
            };
    };
        
}

function Register-TemporaryEvent(
    ## The object that generates the event
    $Object,

    ## The event to subscribe to
    $Event,

    ## The action to invoke when the event arrives
    [ScriptBlock] $Action
)
{
Set-StrictMode -Version Latest

$actionText = $action.ToString()
$actionText += @'

$eventSubscriber | Unregister-Event
$eventSubscriber.Action | Remove-Job
'@

$eventAction = [ScriptBlock]::Create($actionText)
$null = Register-ObjectEvent $object $event -Action $eventAction
}

Export-ModuleMember -Function Install-DeploydApplications
Export-ModuleMember -Function Update-DeploydApplications