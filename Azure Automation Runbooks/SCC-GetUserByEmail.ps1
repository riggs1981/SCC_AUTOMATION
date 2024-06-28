# Param for Power Automate Connector
param($useremail = "")
# Connect to Active Directory
Import-Module ActiveDirectory

# Retrieve user details based on User Email
try {
    $User = Get-ADUser -Filter {UserPrincipalName -eq $useremail} -Properties *
    if ($User) {
        $UserDetails = @{
            Name = $User.Name
            UserPrincipalName = $User.UserPrincipalName
            DistinguishedName = $User.DistinguishedName
            Email = $User.EmailAddress
            UserID = $User.SamAccountName
            # Add more properties as needed
        }
        $UserDetails | ConvertTo-Json
    } else {
        Write-Host "User with UserPrincipleName '$useremail' not found."
    }
} catch {
    Write-Host "Error retrieving user details: $_"
}
