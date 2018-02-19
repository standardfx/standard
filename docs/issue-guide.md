Issue Guide
===========
This page outlines how the core coding team thinks about and handles issues. For us, issues posted here represent actionable work that should be done 
at some future point. It may be as simple as a small product or test bug or as large as the work tracking the design of a new feature. However, it 
should be work that falls under the project charter. We will keep issues open even if the core coding team internally has no plans to address them 
in an upcoming release, as long as we consider the issue to fall under our purview.

When we close issues
--------------------
As noted above, we don't close issues just because we don't plan to address them in an upcoming release. So why do we close issues? There are few major reasons:

1. Issues unrelated to the project. When possible, we'll try to find a better home for the issue and point you to it.
2. Cross cutting work better suited for another team. Sometimes the line between the project and its dependencies blur. For some issues, we may feel that the work is better suited for the dependencies authors other partner. In these cases, we'll close the issue and open it with the partner team. If they end up not deciding to take on the issue, we can reconsider it here.
3. Nebulous and Large open issues. Large open issues are sometimes better suited for [the forum](http://www.reddit.com/standardfx), especially when the work will cross the boundaries of our project and its dependencies.

Sometimes after debate, we'll decide an issue isn't a good fit for our project. In that case, we'll also close it. Because of this, we ask that you don't 
start working on an issue until it's tagged with [up for grabs]() or [api-approved](https://github.com/standardfx/Standard/labels/approved). Both you and the 
team will be unhappy if you spend time and effort working on a change we'll ultimately be unable to take. We try to avoid that.

Labels
------
We use [labels](https://github.com/standardfx/Standard/labels) on our issues in order to classify them. We have the following categories per issue:
* **Area**: These area-&#42; labels call out the assembly or assemblies the issue applies to. In addition to labels per assembly, we have a few other area labels: `area-Infrastructure`, for issues that relate to our build or test infrastructure, and `area-Meta` for issues that deal with the repository itself, the direction of the the project, our processes, etc. See [full list of areas](#areas).
* **Issue Type**: These labels classify the type of issue.  We use the following types:
 * `bug`: Bugs in an assembly.
 * `api-*` (`api-approved`, `api-needs-work`): Issues which would add APIs to an assembly (see [API Review process](api-review-process.md) for details).
 * `enhancement`: Improvements to an assembly which do not add new APIs (e.g. performance improvements, code cleanup).
 * `test bug`: Bugs in the tests for a specific assembly.
 * `test enhancement`: Improvements in the tests for a specific assembly (e.g. improving test coverage).
 * `documentation`: Issues related to documentation (e.g. incorrect documentation, enhancement requests).
 * `question`: Questions about the product, source code, etc.
* **Other**:
 * [up for grabs](): Small sections of work which we believe are well scoped. These sorts of issues are a good place to start if you are new. Anyone is free to work on these issues.
 * `needs more info`: Issues which need more information to be actionable. Usually this will be because we can't reproduce a reported bug. We'll close these issues after a little bit if we haven't gotten actionable information, but we welcome folks who have acquired more information to reopen the issue.

In addition to the above, we have a handful of other labels we use to help classify our issues. Some of these tag cross cutting 
concerns (e.g. `tenet-performance`, whereas others are used to help us track additional work needed before closing an 
issue (e.g. `api-needs-exposed`).

Milestones
----------
We use `milestones` to prioritize work for each upcoming release.

Assignee
--------
We assign each issue to assignee, when the assignee is ready to pick up the work and start working on it. If the issue is not assigned to anyone and you want to pick it up, please say so - we will assign the issue to you. If the issue is already assigned to someone, please coordinate with the assignee before you start working on it.

Areas
-----
Areas are tracked by labels **area-&#42;**. Each area typically corresponds to one or more contract assemblies.

See [Code Owners](code-owners.md)

Triage rules - simplified
-------------------------
1. Each issue has exactly one **area-&#42;** label
2. Issue has no **Assignee**, unless someone is working on the issue at the moment
3. Use **up for grabs** as much as possible, ideally with a quick note about next steps / complexity of the issue
4. Set milestone to **Future**, unless you can 95%-commit you can fund the issue in specific milestone
5. Each issue has exactly one "*issue type*" label (**bug**, **enhancement**, **api-needs-work**, **test bug**, etc.)
6. Don't be afraid to say no, or close issues - just explain why and be polite
7. Don't be afraid to be wrong - just be flexible when new information appears


*Last updated by Betty on 5 Jun, 2017*
