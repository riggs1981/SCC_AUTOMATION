Param
(
    [parameter(Mandatory=$true)]
    [string] $firstname,
    [parameter(Mandatory=$true)]
    [string] $lastname,
    [parameter(Mandatory=$true)]
    [string] $directorate,
    [parameter(Mandatory=$true)]
    [string] $serviceArea,
    [parameter(Mandatory=$true)]
    [string] $jobTitle,
    [parameter(Mandatory=$true)]
    [string] $pw,
    [parameter(Mandatory=$true)]
    [string] $generatedLogin,
    [parameter(Mandatory=$true)]
    [string] $payrollId,
    [parameter(Mandatory=$true)]
    [string] $officeLocation,
    [parameter(Mandatory=$true)]
    [string] $budgetCode,
    [parameter(Mandatory=$true)]
    [string] $managerSamAc,
    [parameter(Mandatory=$true)]
    [string] $email,
    [parameter(Mandatory=$false)]
    [string] $iterator,
    [parameter(Mandatory=$true)]
    [string] $OU
)

$result = [PSCustomObject]@{
    outcome = $null
}

try {
    $displayname = $firstname + " " + $lastname
    $uniqueName = $firstname + " " + $lastname + $iterator
    $generatedCompany = "SCC " + $directorate
    $fixedBU = $budgetCode
    $cred = Get-AutomationPSCredential -Name "PowerAutomateGeneric"

    # Add -PasswordPolicies DisablePasswordExpiration to the New-ADUser cmdlet
    New-ADUser -Credential $cred -Name $uniqueName -SamAccountName $generatedLogin -EmployeeID $payrollId -UserPrincipalName $email -DisplayName $displayname -GivenName $firstname -Surname $lastname -City $serviceArea -Office $officeLocation -StreetAddress $directorate -Company $generatedCompany -Title $jobTitle -Department $fixedBU -EmailAddress $email -Manager $managerSamAc -AccountPassword (ConvertTo-SecureString $pw -AsPlainText -Force) -Enabled:$true -Path $OU -PasswordNeverExpires $true

    $result.outcome = "Success"
} catch {
    # If an exception occurs, set the outcome property to "Error"
    $result.outcome = "Error: $($_.Exception.Message)"
}

# Convert the custom object to JSON
$jsonResult = $result | ConvertTo-Json

# Output the JSON result
Write-Output $jsonResult
