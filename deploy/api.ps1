curl -k "https://localhost/ninestar/api/personal-chart/get?dateOfBirth=1979-06-16&gender=Male" -H "Authorization: 1ba65158-2942-4bba-b6b5-fb39c15e64b0"

# Define variables
$API_BASE_URL = "https://localhost:443/ninestar/api/"
$GET_PERSONAL_PROFILE_URL = "personal-chart/get?dateOfBirth=1979-06-16&gender=Male"
$API_KEY = "1ba65158-2942-4bba-b6b5-fb39c15e64b0"
	
function ProcessErrors(){
	if($? -eq $false)
	{
		throw "The previous command failed (see above)";
	}
}

function _UpdateTLS {
	[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12, [Net.SecurityProtocolType]::Tls13
}

# Ignore SSL certificate errors (For Development ONLY)
function _IgnoreSSLCerts {	
	[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }
}

function _GetPersonalProfile() {
	echo "Getting personal profile..."
  
	$headers = @{
		"Authorization" = "Bearer $API_KEY"
		"Content-Type"  = "application/json"
	}	

	echo "Updating TLS..."

	_UpdateTLS

	echo "Ignore SSL Certs..."

	_IgnoreSSLCerts

	echo "Creating runspace..."

	$runspace = [System.Management.Automation.Runspaces.RunspaceFactory]::CreateRunspace()
	$runspace.Open()
	[System.Management.Automation.Runspaces.Runspace]::DefaultRunspace = $runspace

	$full_url = "{0}{1}" -f $API_BASE_URL, $GET_PERSONAL_PROFILE_URL

	echo "Full URL $full_url"

	# Make the GET request
	echo "Invoking API Call..."
	$response = Invoke-RestMethod -Uri $full_url -Headers $headers -Method Get

	# Output the response
	echo "Outputting the response..."
	$response | ConvertTo-Json -Depth 3
  
	echo $response

	$runspace.Close()
	$runspace.Dispose()

	ProcessErrors  
}

function Main {
	Try {
		_GetPersonalProfile
	}
	Catch {
		Write-Error $_.Exception
		exit 1
	}
}

Main