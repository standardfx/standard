Branching Guide
===============
We will have the following branches in the repository:

**master**
* Where the latest beta code lives
* If all you want is build and consume, this is the btanch you want.

**edge**
* Bleeding edge code where most developments happen.
* Submit your PR here.

**release/v[version]**
* Release branches snapped from master.
* Do not submit pull requests to these branches.
* Fixes are not done in release branches. Any bug fix will be done in the feat branch.
* Release branches are downloadable via the 'Releases' tab on the main page. We generally avoid modifying releases.

**feat/[name]**
* Features (aka topics) under active development by more than one developer.
* Submit PRs here only if you've made prior arrangements to work on something in one of these branches.
* It is up to the developers creating these branches to decide what level of review is required.
* These features will only ship if they are successfully pulled to master or future via the standard PR and API review process.


*Last updated on 16 May, 2017*
