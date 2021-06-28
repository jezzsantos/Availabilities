# Availabilities Project

This is intended as a starter project for learning better software design, using test-driven-development techniques in digital products.

This is not production code, and its deliberately been naïvely implemented with some basic structures and some separation of concerns, some semblance of an architecture, and nice things, but there are glaring omissions, and naïve design decisions, of which you may want to reason about yourself as you work inside this code.

The immediate job at hand is to complete the implementation of this API, first as quickly as possible, and then later using some specific design/refactoring techniques. Hopefully you will learn something about better design in the process.

# The Business Context

We are building a fictitious API that helps some towns-folk manage the booking of a Sport Facility (let's just say that it is public sports ground or something like that) where the townies play: rugby, football, tennis and basketball etc. It is a multipurpose sports field. But only one group at a time can use it.

At the sports ground, we want to display (on a large public screen) the availability of the sports facility in a visual calendar so that people driving by can see that the grounds are available for use and when. The town does not want the public to know who has the grounds reserved this stage, only when it is available/unavailable for use by others.

At the bottom of this giant "screen" we want a tablet that the public can use to create (and cancel) bookings for the facilities ahead of time. (Forget about identifying the user who makes the booking, or who did what at this stage).

There will be a simple JSON API to reflect this kind of functionality:

* `GET /availabilities` - returns a list of (contiguous) available time slots (each with an `Id`, `StartUtc`, `EndUtc`)
* `POST /bookings` {`StartUtc`, `EndUtc`, `Description`} - creates a booking if and only if the timeslot is available, then reserves/removes availability for the duration of the booking.
* `GET /bookings` - returns a collection of current bookings (each with `Id`, `StartUtc`, `EndUtc`, `Description`)
* `DELETE /bookings/{Id}` - deletes a specific booking, and "releases" the availability it once occupied

This API is already implemented in this codebase, but what is missing is the code managing the complex relationship between the bookings and availabilities services. Which is done through a "service" interface called `IAvailabilitiesService`.

This is how availabilities work in the real world:
* Availabilities are defined as a timeslot with a `StartUtc` and `EndUtc`. Adjacent timeslots will be merged into one availability (favoring the left side). These timeslots will be visualized on a giant calendar display.
* The minimum availability that can be booked is defined as being from "noon 2020 (UTC)", and the maximum availability is defined as being "noon 2050 (UTC)". No bookings outside those times can be made.
* Bookings can be made for a specific timeslot, and that timeslot must fully fit (wholly) into an existing availability timeslot in order to create the booking. (otherwise we throw a HTTP 409 - Conflict).
* A booking cannot start before it ends, must be at least 15min long, no longer than 3hours long, and must not start in the past. (only bookings in teh future are accepted).
* Booking start and end times are rounded up to the next nearest 15min time slot. (eg. if a booking is requested to start at 1:01PM, the booking will actually start at 1:15PM. eg. if a booking is requested to start at 1:00PM, the booking will also start at 1:00PM). 
* Bookings can be created back to back (adjacent), no need for any time-margins between one booking ending and another starting.
* Bookings are stored individually. Availabilities are stored individually, but availabilities are combined/merged into contiguous blocks of time, if they overlap or but up against each other (if they are adjacent).
* When the system starts up and there are no bookings then, there is one availability in the system, starting at noon 2020 (UTC) and ending at noon 2050 (UTC).
* When a booking is created or cancelled, and the booking timeslot overlaps with any existing Availability timeslots, then those Availability timeslots are merged/extended/split to encapsulate that new timeslot. The sequence of availabilities that are stored will therefore never be immediately adjacent to each other or overlap with each other, and therefore should be separated in time by at least 15mins.
* When a booking is "reserved" (ie. a booking is created) the booked timeslot is "punched out" from an availability that must exist for that timeslot.
  * In some cases, this timeslot will shorten the start or end of an existing Availability
  * In some cases this timeslot will divide an existing Availability into two parts (shortening the existing Availability, and creating a new Availability)
* When a booking is "released" (ie. a booking is cancelled) the timeslot representing the booked time is added/merged back into set of existing Availabilities.
  * In some cases, this timeslot will extend the timeslot of an existing Availability (if adjacent to or overlapping the end of an existing Availability)
  * In some cases, this timeslot will force a merge of two or more availabilities, requiring the deletion of one or more availabilities that are now eclipsed by a larger availability. 
  * In some cases this will add a new availability, detached from existing Availabilities.
* In any case, whenever a timeslot is added or removed it must result in a linear list of availabilities that never overlap with each other, and must never be adjacent to each other (in time), separated by at least 15mins of time between them.

For the purposes of this exercise, both logical domains (Bookings and Availabilities) are in the same monolithic deployment package, and no attempt should be made to separate them into separate physical domains or services. 

They will however, need to coupled through well-defined interfaces (eg. the `IAvailabilitiesApplication`) so that they can communicate with each other. No coupling at the API or data store level is permitted. (as far as you know there is no formal database and joining tables and querying will be forbidden)

The data store behind this API (right now) is an in-memory store that has been deliberately included to hide away from you (make it difficult to "see") the actual stored data, so that you don't focus on it, or its schema or design, as you might be used to doing in SQL crud-like systems. That is a distraction for this exercise.

# Instructions

* Install Jetbrains Rider IDE.
* Fork this source code repo to your own git repo, and checkout the `master` branch
* Hit F5 to run the API, and use Postman (or similar) to call the API's above, for example,`GET https://localhost:5001/api/availabilities` should return the data, something like this:
```json
{
    "availabilities": [
        {
            "startUtc": "2020-01-01T12:00:00.0000000Z",
            "endUtc": "2050-01-01T12:00:00.0000000Z",
            "id": "5322b21221a24f3484bea6291db02bb5"
        }
    ]
}
```

## Step 1
Now, you have an hour to complete the implementation of the `IAvailabilitiesApplication` interface to achieve the goals of the first exercise. Some of the rules above have already been encoded into the request validation layer (eg. `CreateBookingRequestValidator.cs`). Other rules must be coded by you.

Do this work in a pair (if available), and ignore any test-driven-development approaches. Just go about coding the solution as you normally would today.

## Step 2

Next step, we will explore another approach for doing the same thing, but using some other practices. 

You will create a separate fork of this repo for that, when we get there. Your instructor will lead this next part.

> A footnote about trickery: There are no tricks here. There are no intentional traps for you to fall into. We are not interested so much in the correct solution that you come up with, we are interested in learning from how you write code today, and how you might change that in the future, to level up your skills. The problem set out here is from a real product, and you have about 2/3rds of the complexity of the real solution to solve here. This specific challenge was chosen because this kind of problem is fairly common in scheduling systems, and IMHO is a good one to get started with better design.
