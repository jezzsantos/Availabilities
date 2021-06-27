# Availabilities Project

This is intended as a starter project for learning better software design, using test-driven-development techniques in digital products.

This is not production code, and its deliberately been naively implemented with some basic structures and separation of concerns, some semblance of an architecture, but there are glaring omissions, and naïve design decisions, of which you may want to  about yourself as you work in this code.

The immediate job at hand is to complete the implementation of this API, first as quickly as possible, and then later using some specific design/refactoring techniques.

# The Business Context 

We are building a fictitious API that helps some towns-folk manage the booking of a Sport Facility, let's just say it is sports ground or something like that where townies play: rugby, football, tennis and basketball etc. Its a multipurpose sports field. But only one team at a time can use it.

At the sports ground, we want to display (on a large screen) the availability of the sports facility in a visual calendar so that people driving by can see that the grounds are available for use. The town does not want the public to know who has the grounds reserved for what at this stage, so only to show when it is available/unavailable for use by others.

At the bottom of this giant "screen" we want a keyboard-kiosk that the public can use to create (and cancel) bookings for the facilities ahead of time. (Forget about identifying the user or who did what at this stage).

There will be a simple JSON API to reflect this kind of functionality:

* `GET /availabilities` - returns a list of contiguous available time slots (each with an `Id`, `StartUtc`, `EndUtc`)
* `POST /bookings` {`StartUtc`, `EndUtc`, `Description`} - creates a booking if and only if the timeslot is available, then reserves/removes availability for the duration of the booking.
* `GET /bookings` - returns a list of current bookings (each with `Id`, `StartUtc`, `EndUtc`, `Description`)
* `DELETE /bookings/{Id}` - deletes a specific booking, and "releases" the availability it once occupied

This API is already implemented in this codebase, but what is missing is the code managing the complex relationship between the bookings and availabilities services. Which is done through a "service" interface called `IAvailabilitiesService`.

This is how availabilities work in the real world:
* Availabilities are defined as a timeslot with a `StartUtc` and `EndUtc`. Adjacent timeslots will be merged into one availability (favoring the left side). These timeslots will be visualized on a giant calendar display.
* The minimum availability that can be booked is defined as "noon 2020 (UTC)", and the maximum availability is defined as "noon 2050 (UTC)". No bookings can be made outside those times.
* Bookings can be made for a specific timeslot, and that timeslot must fully fit (wholly) into an existing availability timeslot in order to create the booking. (otherwise we throw a HTTP 409).
* A booking cannot start before it ends, must be at least 15min long, no longer than 3hours long, and must not start in the past. (only future bookings are accepted).
* Booking start and end times are rounded up to the next nearest 15 min time slot. (eg. if a booking is requested to start at 1:01PM, the booking will actually start at 1:15PM). 
* Bookings can be created back to back, no need for any time-margins between one booking ending and another starting.
* Bookings are stored individually. Availabilities are stored individually, but availabilities are combined/merged into contiguous blocks of time, if they overlap or but up against each other (if they are adjacent).
* When a booking is created or cancelled, and the booking timeslot overlaps with any existing Availability timeslots, then those Availability timeslots are merged/extended/split to encapsulate that  timeslot. The sequence of availabilities that are stored will therefore never overlap and must be separated in time by at least 15mins.
* When a booking is "reserved" (ie. a booking is created) its timeslot is "punched out" out from an availability that must exist at that time.
  * In some cases, this timeslot will shorten the start or end of an existing Availability
  * In some cases this timeslot will divide an existing Availability into two parts (shortening the existing Availability, and creating a new Availability)
* When a booking is "released" (ie. a booking is cancelled) the timeslot representing the booked time is added/merged back into set of existing Availabilities.
  * In some cases, this timeslot will extend the timeslot of an existing Availability (if adjacent to or overlapping the end of an existing Availability)
  * In some cases, this timeslot will force a merge of two or more availabilities, requiring the deletion of one or more availabilities that are now eclipsed by a larger availability. 
  * In some cases this will add a new availability, disjointed from existing Availabilities.
* In any case, whenever a timeslot is added or removed it must result in a linear list of availabilities that never overlap with each other, and must never be adjacent to each other (in time), separated by at least 15mins  of time between them.

For the purposes of this exercise, both logical domains (Bookings and Availabilities) are in the same monolithic deployment package, and no attempt should be made to separate them into separate physical domains or services. They will however, need to coupled through well-defined interfaces (eg. the `IAvailabilitiesService`) so that they can communicate. No coupling at the data store level is permitted. (as far as you know there is no formal database and joining tables and querying is impossible)

The data store behind this API is an in-memory store that has been deliberately included to hide away from you (make it difficult to "see") the actual stored data, so that you don't focus on it, or its schema or design, as you might be used to doing in SQL crud-like systems. That is a distraction for this exercise.

# Instructions

* Install Jetbrains Rider IDE.
* Fork this source code repo to your own git repo, and checkout the `master` branch
* Hit F5 to run the API, and use Postman (or similar) to call the API's above.

## Step 1
Now, you have less than an hour to complete the implementation of the `IAvailabilitiesService` interface to achieve the goals of the first exercise. 

Do that in a pair, and ignore any test-driven-development approaches, just code the solution as you normally would today, no tests.

## Step 2

Next step, we will explore another approach for doing the same thing, but using some other practices. 

You will create a separate fork of this repo for that, when we get there. Your instructor will lead this next part.