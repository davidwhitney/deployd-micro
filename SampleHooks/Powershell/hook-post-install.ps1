param([parameter(Mandatory=$false)][string]$Environment)

# Set the name of your website/application
$applicationName = "MyWebsite"

Write-Host "This is a powershell script executed on the target server after $applicationName is installed"
Write-Host "This installation is for the '$Environment' environment"
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
		Write-Host "This is the ip/port we would put in the pool"
	}
	try {
		$appPool = Get-WebAppPool "$applicationName AppPool"
		if ($appPool)
		{
			Start-WebAppPool "$applicationName AppPool"
			Write-Host "Started application pool"
		}
	} catch
	{

	}

	foreach($binding in $bindings)
	{
		Write-Host "Site warmup $trimmed"
		$url = "http://"+$trimmed
		$req=[system.Net.HttpWebRequest]::Create($url);
		$res = $req.getresponse();
		$stat = $res.statuscode;
		$res.Close();
	}

	Write-Host "Site returned status code $stat"
}
