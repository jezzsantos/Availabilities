# Availabilities Project

This is intended as a starter project for learning better software design, using test-driven-development techniques in digital products.

This is not production code, and it is deliberately been naively implemented with some basic structures and some separation of concerns, some semblance of an architecture, and nice things, but there are glaring omissions, and naïve design decisions, of which you may want to reason about yourself as you work inside this code.

The immediate job at hand is to complete the implementation of this API, first as quickly as possible, and then later using some specific design/refactoring techniques. Hopefully, you will learn something about better design in the process.

# The Business Context

We are building a fictitious API that helps some townsfolk manage the booking of a Sports Facility (let's just say that it is a public sports ground or something like that) where the townies play: rugby, football, tennis, basketball, etc. It is a multipurpose sports field. But only one group at a time can use it.

At the sports ground, we want to display (on a large public screen) the availability of the sports facility in a visual calendar so that people driving by can see when the grounds are available for use and when. The town does not want the public to know who has the grounds reserved at this stage, only when it is available/unavailable for use by others.

At the bottom of this giant "screen", we want a tablet that the public can use to create (and cancel) bookings for the facilities ahead of time. (Forget about identifying the user who makes the booking, or who did what at this stage).

There will be a simple JSON API to reflect this kind of functionality:

* `GET /availabilities` - returns a list of (contiguous) available time slots (each with an `Id`, `StartUtc`, `EndUtc`)
* `POST /bookings` {`StartUtc`, `DurationInMins`, `Description`} - creates a booking if and only if the timeslot is available, then reserves/removes availability for the duration of the booking.
* `GET /bookings` - returns a collection of current bookings (each with `Id`, `StartUtc`, `EndUtc`, `Description`)
* `DELETE /bookings/{Id}` - deletes a specific booking, and "releases" the availability it once occupied

This API is already implemented in this codebase (areas that are ~~struck out~~ are already implemented), but what is missing is the code managing the complex relationship between the bookings and availabilities services. Which is done through a "service" interface called `IAvailabilitiesService`.

This is how Availabilities and Bookings work in the real world:
* All dates and times are in UTC.
* The timeslots behind the availabilities will be visualized on a giant calendar display.
* An `Availability` is defined as a notional "timeslot" with an `Id`, `StartUtc` and `EndUtc`. 
  * The set of Availability will consist of contiguous timeslots, with no joins and only gaps in time.
* ~~The minimum availability that can be booked is defined as being from "noon 2020 (UTC)", and the maximum availability is defined as being "noon 2050 (UTC)".~~ 
  * ~~No bookings outside those times can be made.~~
* Bookings can be made for a specific timeslot, and that timeslot must fully fit (wholly) inside (inclusive) an existing availability timeslot in order to create the booking. 
  * Otherwise, we throw a HTTP 409 - Conflict.
  * If you make a Booking today from 4pm to 5pm, and there is existing Availability from 4pm to 5pm today, the Booking can be created.
* ~~A booking cannot start before it ends~~
  * ~~Must be at least 15min long, no longer than 3hours long~~
  * ~~Must not start in the past. Only bookings in the future (within a minute tolerance) are accepted.~~
  * ~~eg if the time now (in the API) is 1:00:00PM and the booking was made for 12:59:59PM then it is permitted.~~
* Booking start and end times are automatically rounded-up by the API to the next nearest 15min timeslot, (within a 1-minute tolerance)
  * eg. if a booking is requested to start at 1:01:00PM, the booking will actually start at 1:15PM.
  * eg. if a booking is requested to start at 1:29:00PM, the booking will actually start at 1:30PM.
  * eg. if a booking is requested to start at 1:00:00PM, the booking will start at 1:00PM.
  * eg. if a booking is requested to start at 1:00:01PM, the booking will also start at 1:00PM.
* Bookings can be created back to back (adjacent), with no need for any time-margins between one booking ending and another starting (no overlapping).
* ~~Bookings are stored individually. Availabilities are stored individually~~, but availabilities are combined/merged into contiguous blocks of time (if they are created as overlapped or butted-up against each other).
* ~~When the system starts up and there are no bookings then, there is one availability in the system, starting at noon 2020 (UTC) and ending at noon 2050 (UTC).~~
* When a booking is created or cancelled, and the booking timeslot coincides with any existing Availability timeslots, then those Availability timeslots are merged/extended/split to encapsulate that new timeslot. The resulting sequence of availabilities that are stored will therefore never be immediately adjacent to each other or overlap with each other, and therefore overall the timeline of availabilities should be separated (in time) by gaps at least 15mins long.
* When a booking is "reserved" (ie. a booking is created) the booked timeslot is "punched out" from an existing Availability (that must have existed for that timeslot).
  * In some cases, this timeslot will shorten the start or end date of an existing Availability.
  * In some cases, this timeslot will divide an existing Availability into two parts (shortening the existing Availability, and creating a new Availability)
* When a booking is "released" (ie. a booking is cancelled) the timeslot representing the booked time is added/merged back into set of existing Availabilities.
  * In some cases, this timeslot will extend the timeslot of an existing Availability (if adjacent to or overlapping the end of an existing Availability)
  * In some cases, this timeslot will force a merge of two Availabilities, requiring the deletion of one availability that is now eclipsed by a larger resulting Availability. 
  * In some cases, this will add a new Availability, detached from existing Availabilities.
* In any case, whenever a timeslot is added or removed from the Availabilities it must result in a linear list of Availability that never overlap with each other, and must never be adjacent to each other (in time), separated by gaps at least 15mins long.

For example:
There would be only one availability, that looks like this timeline when there are no bookings in the system:

          |--------------------------------------------------------------------------------------------|
2020-01-01T00:00:00.000Z                                                                     2020-01-01T00:00:00.000Z

When you create a new Booking, such as this one:
                                        |------------------------------|
                                     StartUtc                    +DurationInMins

The existence of that Booking would have this effect on breaking the set of Availabilities into two pieces:

          |-----------------------------|                              |-------------------------------|
2020-01-01T00:00:00.000Z             StartUtc                        EndUtc                  2020-01-01T00:00:00.000Z

If that same Booking was to be cancelled/deleted, then the Availabilities would be restored to this single: 

          |--------------------------------------------------------------------------------------------|
2020-01-01T00:00:00.000Z                                                                     2020-01-01T00:00:00.000Z


## Caveats

For the purposes of this exercise, both logical domains (Bookings and Availabilities) are in the same monolithic deployment package, and no attempt should be made to separate them into separate physical domains or services. 

They will however, need to coupled through well-defined interfaces (eg. the `IAvailabilitiesApplication`) so that they can communicate with each other. No coupling at the API or data store level is permitted. (as far as you know there is no formal database and joining tables and querying will be forbidden)

The data store behind this API (right now) is already implemented as an in-memory store that has been deliberately included to hide away from you (make it difficult to "see") the actual stored data, so that you don't focus on it, or its schema or design, as you might be used to doing in SQL crud-like systems. That is a distraction for this exercise.

# Instructions

* Install Jetbrains Rider IDE.
* Install .NET6
* Make sure to run `dotnet dev-certs https --trust`
* Fork this source code repo to your own git repo, and checkout the `master` branch
* Hit F5 to run the API, and use PostMan (or similar tools) to call the API's above, for example,`GET https://localhost:5001/api/availabilities` should return the data, something like this:

> Note: In PostMan you may need to disable "SSL certification verification" so that you can use the built-in dotnet development certificate.

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
> Note: Add the `Accept: application/json` header to the request in PostMan

Now, create a new Booking with this API: `POST https://localhost:5001/api/bookings` including data such as this:

```json
{
    "startUtc": "2023-01-01T12:00:00.0000000Z",
    "durationInMins": 15
}
```

> Note: that you should receive a `405-NotImplementedException`, which is the correct response, since at this point, we have no code, and the code just throws a `NotImplementedException`

## Step 1


### Getting your bearings

The code base is loosely structured in layers (ApiService -> Application -> Storage), and we don't yet define a domain layer. The patterns are loosely based off of ports and adapters architecture.

> Note: These are not strict production code patterns, this is just an exercise

The API layer contains a class `AvailabilitiesService.cs` which represents all your public REST API's. (similar to ASP.NET Controllers, but we are using ServiceStack here).

The API layer also includes request validators, (eg. `CreateBookingRequestValidator.cs` etc.) these validators are auto-wired up by ServiceStack, and no coding is required. They are automatically run when an inbound HTTP request is received, and your code in the service class (`AvailabilitiesService.cs` is only invoked when the validator passes).

> Note: You need to define a request validator for every inbound API call  

Notice that every API call (in `AvailabilitiesService.cs`) defines a method with the same name as the HTTP verb, and also the type of the one and only parameter passed to the method (we call it a "Request DTO") defines a `[Route]` attribute that also defines the allowable verbs.

Finally, every API call unwraps all data from the various parts of the request (QueryString, Body, Headers etc), and delegates that call to the Application Layer component.

The Application Layer either returns data to the API layer to construct a response, or the Application Layer throws an exception to describe a 4XX or 500 response. The mappings that define Exception->HTTP StatusCode are defined in `ServiceHost.cs`.

> The Application layer should never have any knowledge about HTTP requests (ever!)

### Get started

Now, you have 1 hour to complete the implementation of the `IAvailabilitiesApplication` interface to complete this API and achieve the goals of the first exercise (Notes are in there to guide you). 

Some of the rules above (that are already ~~stuck out~~) have already been encoded into the request validator (`CreateBookingRequestValidator.cs`). The rest is up to you to codify.

Recommendation: Do this work in a pair (if available), and ignore any test-driven-development approaches. There are extensive tests you can run to see how you are progressing.

Just go about coding the solution as you normally would today, and try to give yourself one hour to get it done. 

> If you run over the hour, it does not really matter, the point of this constraint is to simulate the conditions that you experience at work every day (and we can then see how that affects your output given your coding practices today).

## Step 2

Next step, we will explore another approach for doing the same exercise, but using some slightly different practices. 

You will move your existing code to a separate branch in your repo and then perform this step on the main/master branch. 

Your instructor will lead this next part.

> A footnote about trickery: There are no tricks here. There are no intentional traps for you to fall into. We are not interested so much in the correctness of the solution that you come up with here, we are more interested in you learning from how you write code today, and how you might change that in the future, to level up your skills. 
> The problem laid out here is close to one from a real product (and you have about 2/5rds of the complexity of the real solution to solve here). This specific challenge was chosen because this kind of product problem is fairly common in many scheduling systems, and IMHO is a good one to get started with learning better design, without resorting to academic and unrelated puzzles to your daily work.
