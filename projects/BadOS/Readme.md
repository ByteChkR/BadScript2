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

