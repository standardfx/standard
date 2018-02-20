Breaking Changes
================
As a framework library, we want developers to be reasonably assured that their code will not break with each release upgrade, while at the 
same time be agile in offering the latest features.

We try to balance compatibility and the need to evolve by categorizing breaking changes into 4 buckets:

1. Public Contract
2. Grey Area
3. Bugfix
4. Clearly internal


## Bucket 1: Public Contract
*Clear violation of public contract.*

Examples:
* Renaming or removing of a public type, member, or parameter
* Changing the value of a public constant or enum member
* Sealing a type that wasn't sealed
* Making a virtual member abstract
* Adding an interface to the set of base types of an interface
* Removing a type or interface from the set of base types
* Changing the return type of a member
* ...or any other incompatible change to the shape of an API surface


## Bucket 2: Grey Area
*Changes that is expected to impact reasonable usage scenarios.*

Examples:
* Throwing a new/different exception type in an existing common scenario
* An exception is no longer thrown
* A different behavior is observed after the change for an input
* Decreasing the range of accepted values within a given parameter
* A new instance field is added to a type (impacts serialization)
* Change in timing/order of events (even when not officially documented)
* Change in parsing of input and throwing new errors (even if parsing behavior is not officially documented)


## Bucket 3: Bugfix
*Change of behavior that developers could have depended on, but probably wouldn't.*

Examples:
* Correcting behavior in a subtle corner case
* This [sort of scenario](https://xkcd.com/1172) dreamt up by XKCD


## Bucket 4: Clearly internal
*Changes to surface area or behavior that is clearly internal or non-breaking in theory.*

Examples:
* Changes to internal API that break private reflection


## Our take on the buckets
It is impossible to evolve a code base without making breaking changes. However, we also don't want to agonize everyone with changes that cause 
too much pain. The default position is:

* All bucket 1, 2, and 3 breaking changes require talking to the repo owners first:
  - We generally **don't accept** change proposals that are in bucket #1.
  - We **might accept** change proposals that are in #2. See below for more details.
  - We **usually accept** changes that are in bucket #3 and #4.
* If you're not sure which bucket applies to a given change, talk to the repo owners.


## How we decide on bucket #2

Buckets #2 is tricky. It doesn't matter if the old behavior is "wrong", we still need to think through the implications. With compatibility 
a major concern, there are a number of possibilities:

* **Compat switch**. Accept the change, but add a compatibility switch that allows developers to bring back the old behavior if necessary.
* **Polyfill**. Accept the change, and add a polyfill library that developers can reference if the old behavior is wanted.
* **New API**. Implement the changes in a new library or namespace.
* **Documentation**. In some minor cases, it is necessary to break legacy code, and alternatives just aren't feasible. In this case, we would clearly document the change.
* **Reject**. Sometimes we just have to live with our bad decisions in the past, but we'll try not make the same mistake again :D


*Last updated on 16 May, 2017*
