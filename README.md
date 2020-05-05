# KeepassPowershell
Powershell Cmdlets to work with Keepass databases.

This package only provides read-only functions.
 
Modifying Keepass entries is currently not planned.

## Getting started
* Download a release
* Unpack into a temporary folder
* Run Installer.ps1 in a Powershell session 
* Follow the instructions
* Enjoy!

To jump in take a look at the examples in the [Documentation/ExampleScripts](https://github.com/hummigbird1/KeepassPowershell/tree/master/Documentation/ExampleScripts) folder in this repository.

## Available Cmdlets
To see all Cmdlets provided and check if the installation was actually successful just run:

`Get-Module -ListAvailable KeepassPSCmdlets`

The output you receive should contain the Version and the list of Cmdlets like the following: 

* Find-KeepassEntry

    Helps you in finding Keepass entries by providing parameters for groups, tags and names and matching options like Wildcard and Regular Expressions

* Get-KeepassEntry

    Helps you retrieving Keepass entries that you know the ID of (e.g. by finding them once with the Find-KeepassEntry cmdlet first.)

* ConvertTo-NetworkCredential

    Convenience function to convert a Keepass entry to a Network Credential object that is often required by other cmdlets.

## Known Issues and Limitations
* The first use requires you to specify the location of the Keepass Binaries.
 
   After that the assemblies are loaded into the Powershell Application domain and can not be changed unless you exit the Powershell session.
   
   What does that mean?

   It's a minor issue that prevents parallel usage of different Keepass binaries/versions.

   First and foremost this will probably not affect you but if it does, be aware that I am trying to improve on that in the future.

* Currently only the out-of-the box key for Keepass Databases is supported (Masterpwassword, Windows-User Account, Keyfile)

## Feedback and Support
* Let me know if something does not work by creating a issue here in this repository.
* Let me know if I can improve something by creating a suggestion here in this repository.

# TODO
1. More description
2. Provide inline help in cmdlets
3. Examples
4. License