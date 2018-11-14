Target .NET Framework Support
=============================
This page documents the priority of .NET framework targets that we will support.

Full .NET framework
-------------------
The full .NET framework is mostly pre-installed with Windows. Projects in this repo will be interested in the following versions:

|---------|-----------------|-----------------|
| Version | Client OS       | Server OS       |
|---------|-----------------|-----------------|
| 4.6.2   | Windows 10.1607 | Windows 2016    | 
| 4.5     | Windows 8       | Windows 2012    |
| 4.5.1   | Windows 8.1     | Windows 2012 R2 |
| 3.5     | (optional)      | (optional)      |
|---------|-----------------|-----------------|

The following targets are of lower priority. Many projects may not support them:

- Versions 1.0, 1.1, 2.0 and 3.0 are installed on Vista and before. We consider these platforms as optional legacy support
- Version `4.0` is an ambiguous target.
- Version `4.5.2` is an upgrade to `4.5.1` that must be installed manually. We envisage that most users have not installed it.
- Version `4.6` is installed on Windows 10.1507, and `4.6.1` on Windows 10.1511. We envisage that most users will have upgraded from these earlier Windows 10 editions.
- Version `4.7` is installed on Windows 10.1703. We believe that this build is not widely installed.
- Version `4.7.2` is installed on Windows 10.1803. We believe that this build is not widely installed.

The following targets are currently very new and we will be enhancing support for them in the near future:
- Version `4.7.1` is installed on Windows 10.1709 and Windows 2016.1709
