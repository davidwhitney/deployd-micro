param(
	[parameter(Mandatory=$false)][string]$Environment,
	[parameter(Mandatory=$false)][string]$InstallationRoot
	)
	

# Set the name of your website/application
$applicationName = "MyWebsite"

Write-Host "This is a powershell script executed on the target server before $applicationName is installed"
Write-Host "This installation is for the '$Environment' environment"
Write-Host "The application is installed to '$InstallationRoot'"

# Sample website administration
Import-Module WebAdministration
$website = Get-Website $applicationName

if ($website)
{
	$bindings = Get-WebBinding $applicationName
	Write-Host "$applicationName has $($bindings.length) bindings"
	foreach($binding in $bindings)
	{
		$trimmed = $binding.bindingInformation.TrimEnd(":")
		Write-Host "$applicationName has binding $trimmed"
		Write-Host "This is the ip/port we would take out of the pool"
	}

	$appPool = Get-WebAppPool "$websiteName AppPool"
	if ($appPool)
	{
		Stop-WebAppPool "$websiteName AppPool"
	}
}