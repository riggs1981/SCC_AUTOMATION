# Import Active Directory module
Import-Module ActiveDirectory

# Get all AD users
$users = Get-ADUser -Filter *

# Initialize an empty array to store user data
$userData = @()

# Iterate over each user
foreach ($user in $users) {
    # Create a custom object with required attributes
    $userObject = New-Object PSObject -Property @{
        "SamAccountName" = $user.SamAccountName
        "UserPrincipalName" = $user.UserPrincipalName
        "FirstName" = $user.GivenName
        "LastName" = $user.Surname
       
    }

    # Add the user object to the array
    $userData += $userObject
}

# Convert the array to JSON format
$userData | ConvertTo-Json
