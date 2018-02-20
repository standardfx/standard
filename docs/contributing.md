Contributing to Standard Project
======================
The StandardFX Project team maintains several guidelines for contributing to the Standard Project repos, which 
are provided below. Many of these are straightforward, while others may seem subjective. A team member will 
be happy to explain why a guideline is defined as it is.


Contribution Guidelines
=======================
- [Copyright](#copyright) describes the licensing practices for the project.
- [General Contribution Guidance](#general-contribution-guidance) describes general contribution guidance, including more subjective stylistic guidelines.
- [Contribution Bar](#contribution-bar) describes the bar that the team uses to accept changes.
- [Contribution Workflow](contributing-workflow.md) describes the workflow that the team uses for considering and accepting changes.


General Contribution Guidance
=============================
There are several issues to keep in mind when making a change.

DOs and DON'Ts
--------------
* **DO** follow our [coding style](csharp-coding-style.md)
* **DO** give priority to the current style of the project or file you're changing even if it diverges from the general guidelines.
* **DO** include tests when adding new features. When fixing bugs, start with adding a test that highlights how the current behavior is broken.
* **DO** keep the discussions focused. When a new or related topic comes up it's often better to create new issue than to side track the discussion.
* **DO** blog and tweet (or whatever) about your contributions, frequently!
* **DO NOT** send PRs for style changes. 
* **DON'T** surprise us with big pull requests. Instead, file an issue and start a discussion so we can agree on a direction before you invest a large amount of time.
* **DON'T** commit code that you didn't write. If you find code that you think is a good fit to merge, file an issue and start a discussion before proceeding.
* **DON'T** submit PRs that alter licensing related files or headers. If you believe there's a problem with them, file an issue and we'll be happy to discuss it.
* **DON'T** add additional functionality without filing an issue and discussing with us first. See [Contributing workflow](contributing-workflow.md).

Working style
-------------
See the [contribution workflow](contribution-workflow.md) page for how we manage our projects.


Contribution "Bar"
==================
Project maintainers will merge changes that align with project priorities and/or improve the product significantly for a broad set 
of apps. Proposals must also satisfy the [guidelines](#contribution-guidelines).

Maintainers will not merge changes that have narrowly-defined benefits, due to compatibility risk. The codebase may be used by 
several products and on different platforms. Changes to the codebase can become part of these products, but are first reviewed 
and tested to ensure they are correct for those products and will not inadvertently break applications. We may revert changes if 
they are found to be breaking.

Contributing Ports
------------------
We encourage ports to other platforms.

Ports have a weaker contribution bar, since they do not contribute to compatibility risk with existing products. For ports, 
we are primarily looking for functionally correct implementations.


Copyright
=========

Source License
--------------
The StandardFX Project project uses multiple licenses for the various project repositories. Most projects use 
the [MIT License](https://opensource.org/licenses/MIT) for code and 
the [Creative Commons Attribution 4.0 International Public License (CC-BY)](https://creativecommons.org/licenses/by/4.0/) license 
for documentation. The [Apache 2 License](https://opensource.org/licenses/Apache-2.0) is also used. 

See the license file at the root of project repositories for the specific license.

Binary License
--------------
We produces a distribution of Standard Project licensed under the [Binary Distribution License](distro-license.txt). Other groups 
or companies may produce their own distributions of StandardFX Project.

File Headers
------------
A copyright file header is applied to every code file. Please ensure this prior to providing a commit.

Here is an example for C++ or C# source file:

```
// <copyright>
// Licensed to the StandardFX Project project under one or more agreements.
// The StandardFX Project project licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
```

Copying Files from Other Projects
---------------------------------
StandardFX Project uses some files from other projects, typically where a binary distribution does not exist or would be inconvenient.

The following rules must be followed for PRs that include files from another project:

- The license of the file is [permissive](https://en.wikipedia.org/wiki/Permissive_free_software_licence).
- The license of the file is left in-tact.
- The contribution is correctly attributed in the [third party notices](../THIRD-PARTY-NOTICE.txt) file in the repository, as needed.

Porting Files from Other Projects
---------------------------------
There are many good algorithms implemented in other languages that would benefit the Standard Project project. The rules for porting a 
Java file to C#, for example, are the same as would be used for copying the same file, as described above.

[Clean-room](https://en.wikipedia.org/wiki/Clean_room_design) implementations of existing algorithms that are not permissively licensed 
will generally not be accepted. If you want to create or nominate such an implementation, please create an issue to discuss the idea.

Contributor License Agreement
-----------------------------
You must sign a [Contributor License Agreement (CLA)](contributor-license-agreement.md) before your PR will be merged. This is a one-time requirement. 

You don't have to do this up-front. You can simply clone, fork, and submit your pull-request as usual. When your pull-request is created, it is 
classified by a build bot. If the change is trivial (e.g. you just fixed a typo), then the PR is labelled with `cla-not-required`. Otherwise it's 
classified as `cla-required`. Once you signed a CLA, the current and all future pull-requests will be labelled as `cla-signed`.


*Last updated by Betty on 5 Jun, 2017*
