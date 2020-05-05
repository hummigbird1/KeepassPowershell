<# Example: List all Entries of a Keepass Database #>
<# 
    Prerequisites: 
        The Keepass Database is located in D:\temp
        The Keepass Database is named Test.kdbx
        The Keepass Database is secured with a Master Password 123 and is tied to the Current Windows User Account
        The Keepass Binaries are Located in a Folder D:\PortableApps\KeePass\
#>

<# First we need to convert the plain text to a secure string #>
$masterPassword = ConvertTo-SecureString -AsPlainText -Force -String "123"

<# Find a specific entry (by id) and assign it to a variable "entry" #>
$entry = Get-KeepassEntry -KeepassLocation D:\PortableApps\KeePass\ -InputObject D:\temp\Test.kdbx -MasterPassword $masterPassword -WindowsUserAccount -Id 4C4FF16451B63845BA66E7DAB83C0A22

<# First let's see how a part of the entry looks like we are working with: #>
<#

Id                           : 4C4FF16451B63845BA66E7DAB83C0A22
GroupName                    : Test
UserName                     : Michael321
...

#>

<# Use that found entry and convert it to a Network Credential object #>
$credential = ConvertTo-NetworkCredential -InputObject $entry -DomainPropertyName GroupName <# GroupName is the name of the property in the "entry" we provieded #>

<# Output the contents of the credential object we just created #> 
$credential 
<# Example output

UserName                       Password
--------                       --------
Michael321 System.Security.SecureString

#>

<# The credential object we just created then can be used as parameters for other cmdlet which require a Network Credential input #>

<# Lets say the Keepass Database is structured in the way that the Entry Group name reflects the Domain of the User Account we just gotten you can tell the Cmdlet to use that property as Domain #>
ConvertTo-NetworkCredential -InputObject $entry -DomainPropertyName GroupName

<# Example output with a fully qualified domain name 

UserName                            Password
--------                            --------
Michael321@Test System.Security.SecureString

#>

<# You can also specify to create a Non-UPN username by sepcifying false for the 'UpnUserName' parameter #>
ConvertTo-NetworkCredential -InputObject $entry -DomainPropertyName GroupName -UpnUserName:$false

<# Notice how the UserName has changed to the previous example

UserName                            Password
--------                            --------
Test\Michael321 System.Security.SecureString

#>

<# Of course you could alsp just pipe the entry to the cmdlet like the following to get the same result #>
$entry | ConvertTo-NetworkCredential -DomainPropertyName GroupName -UpnUserName:$false