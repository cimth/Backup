# Backup

## Introduction

This console application lets the user backup directories and files. 
Source directories will be mirrored to the destination path and files will be copied into the directory given as destination.
The destination directory might be located on another connected device.

## Screenshot

![Screenshot](Screenshot.png)

## Included dependencies

| Dependency            | Version |
|-----------------------|---------|
| .NET                  | 7.0     |

## Configuration

The configuration of the backup is done via XML files. 
Those files must be placed inside the directory _backup\_profiles_. 
Thus the folder structure looks as follows:

```
parent-directory
|
|-- backup_profiles
|   |
|   |-- profile1.xml
|   |-- my-backup-profile.xml
|   |-- ...
|
|-- Application
    |-- Backup / Backup.exe
    |-- ...
``` 

The _Application_ directory can be renamed as desired.
The names of the profile files are also free selectable. 
However note that those names are shown when selecting the correct profile after starting the application.
Thus in the example structure above the user could choose between the profiles 'profile1' and 'my-backup-profile' to be executed. 

The following file shows an example configuration. 
There can be added multiple _backup\_location_ entries if required. 
Each of those entries defines the backup of one single directory or one single file which is given in the _\<src\>_-attribute.
The destination path (_\<dest\>_-attribute) is the directory which will be the mirror of the source directory or which will contain the source file.
Thus, in the case of files you can store multiple files located in different source directories at the same destination.
When using directories as source path, however, an exactly mirror will be created at the destination path.

It is also possible to add multiple _path_ entries inside the _exclude_ element. 
If no path should be excluded, the whole _exclude_ entry has to be omitted.

```xml
<?xml version="1.0" encoding="utf-8"?>
<backup_profile>
    <backup_location>
        <src>/home/user/to_backup</src>
        <dest>/media/user/device/is_backup</dest>
        <exclude>
            <path>/home/user/to_backup/do_not_backup</path>
        </exclude>
    </backup_location>
</backup_profile>
```

In this example the directory _/home/user/to_backup_ is backed up to _/media/user/device/is\_backup_. The sub directory _do\_not\_backup_
however is not copied during the backup because it is marked as an excluded path.

**Attention:** Even if _to\_backup_ is a file it is saved as _is\_backup/to\_backup_ which means that _is\_backup_ is always interpreted as a directory.

### Wildcards

You can use wildcards for excluding multiple files or directories with configuring only one exclude path.
Currently the following wildcards are allowed:

|                wildcard pattern                |                                  meaning                                   |                                example                                 |
|:----------------------------------------------:|:--------------------------------------------------------------------------:|:----------------------------------------------------------------------:|
|                  \*._\<ext\>_                  |              exclude all files with the given file extension               |      _\*/.class_ for excluding all files with _.class_ extension       |
|   \*/_\<file\>_ <br/>or<br/> \*\\_\<file\>_    |               exclude all files with the given relative path               |    _\*/.DS_STORE_ for excluding all files with the name _.DS_Store_    |
| \*/_\<dir\>_/\* <br/>or<br/> \*\\_\<dir\>_\\\* | exclude all sub directories inside the backup location with the given name | _\*/node_modules/\*_ for excluding all _node\_modules_ sub directories |

Note that only the directory wildcard with your system's directory separator character will work. 
Thus, in Linux or macOS you need to use the wildcard \*/_\<dir\>_/\*, while in Windows you have to use the wildcard \*\\_\<dir\>_\\\*.

The following file shows how to use the example wildcards from the table above inside an actual backup profile:

```xml
<?xml version="1.0" encoding="utf-8"?>
<backup_profile>
    <backup_location>
        <src>/home/user/to_backup</src>
        <dest>/media/user/device/is_backup</dest>
        <exclude>
            <path>*.class</path>
            <path>*/node_modules/*</path>
        </exclude>
    </backup_location>
</backup_profile>
```

### Global exclude paths

You can also set global exclude paths which then will be applied to all listed backup locations.

With the following configuration, all _.class_ files and all _node_modules_ sub directories inside _to\_backup\_1_ 
and _to\_backup\_2_ will be excluded from the backup.

```xml
<?xml version="1.0" encoding="utf-8"?>
<backup_profile>
    <!-- Exclude in all backup locations -->
    <exclude>
        <path>*.class</path>
        <path>*/node_modules/*</path>
    </exclude>
   
    <backup_location>
        <src>/home/user/to_backup_1</src>
        <dest>/media/user/device/is_backup_1</dest>
    </backup_location>

    <backup_location>
        <src>/home/user/to_backup_2</src>
        <dest>/media/user/device/is_backup_2</dest>
     </backup_location>
</backup_profile>
```


## Structure

The `Start` class inside the `Start` namespace marks the entry point for the application. 
This class is responsible for reading the backup profiles and starting the backup. 
If an error occurs while fetching the backup configurations or while reading an invalid user input, the `Start` class will show a message for locating the problem.

The actual Backup is done by the `BackupRunner` class inside the `Start` namespace. 

For converting the XML configuration files the `XML` package contains the class `BackupProfileConverter`.
This converter is responsible for creating the objects of the types `BackupProfile` and `BackupLocation` inside the
`Data` package. 
Note that an object of the type `BackupLocation` corresponds to a directory that should be backed up.

## Build and use the application

1. Publish for the desired platform (Linux, macOS and Windows tested)
2. If not published as 'Self-contained' install the .NET runtime on the device
3. Create the folder structure described below the chapter 'Configuration'
4. Create one or more backup profiles (.xml files, structure see in chapter 'Configuration')
5. Start the application (Linux: Backup, Windows: Backup.exe)
6. Choose one of the backup profiles (the number displayed inside the square brackets 
   (e.g. enter _1_ and then press _Return_ for selecting the profile with the prefix '\[1\]'))

### Attention:
If giving a directory path as the source for the backup, the application will exactly create a copy of this source directory at the destination path.
This includes deleting files at the destination that are not longer saved at the source.
Thus, take care when defining and running backup profiles. Dry runs might help you to analyze how the application handles your specific backup profiles.

### Dry runs:

You can use the flag '--dry' (e.g. enter _1 --dry_, then press _Return_) to get displayed all the changes that would be made by the chosen backup profile without actually performing any of this changes.
This is a good way to check what changes will be made by running a backup profile before using it to backup real data.

### Multiple runs:

You can do multiple backup runs without needing to restart the application each time.
Therefore the application asks you whether you want to start another run after one backup has successfully finished.
This request will not be made if the backup finishes with an error.