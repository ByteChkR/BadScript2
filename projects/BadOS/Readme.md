# BadOS project for Virtual Machines

## Filesystem

- `/`
    + Root Directory
    + Non-Persistent
- `/home`
    + User Home Directories
- `/conf`
    + Persistent OS Configs


## `/startup.bs`
Loads os specified in startup.json


## `/startup.json`
```json
{
    "OS_DIR": "/os",
    "OS_NAME": "Bad OS"
}
```

## `$OS_DIR/init.bs`

Entry point for the os.
loads core systems
loads core plugins


## Creating a VM File System

Run `bs build Release` to create the filesystem with all projects built in release mode.

Run `bs build Debug` to create the filesystem with all projects built in debug mode.

Run `bs build Clean` to clear all build/output/temp files from the directory


Run `bs vm Test.vm.json` to test the file system in a non-persistent vm.

