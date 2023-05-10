# Deep dive into .NET performance and native AOT

## 00:00 Introduction: Start with catalog API from eShop on ASP.NET Core 3.1

3 mins

- Show the app, extracted from eShop
- Backend API app with
  - Items API
  - Images endpoint, image files stored on disk
- Frontend Blazor app that calls backend API app to display list of items

## 00:03 1st optimization: Upgrade .NET versions

5 mins

- 5.0, 6.0, 7.0, 8.0
- Performance is a forever goal, i.e. every release we work on performance
- Talk about TechEmpower, show numbers increasing from version to version
- Point to the Upgrade Assistant (try to use it if we can)
- Point to stoub's performances posts

## 00:08 2nd optimization: Move to minimal APIs

5 mins

- Use bombardier to drive fixed load amount (e.g. 10000 requests) against backend API app
- Use VS profile session for CPU and allocation profile, these become our baseline numbers
- See lots time and allocations in MVC
- Rewrite APIs to use minimal APIs
- Discuss different architecture, reduction of layers and processing

## 00:13 3rd optimization: Optimize JSON options

7 mins

- Show bad usage of JSON options, e.g. creating instance each time
- Launch backend in WSL(?), use dotnet-trace in WSL to get profiles, copy it to Windows and view it in VS
- Change encoding settings
- Show differences between implicit JSON serialization in minimal APIs vs. serialize to HttpResponse vs. using Results.Json
  - Traps in each appraoch
- Use the source generator
- Change to IAsyncEnumerable serialization, talk about trade-offs

## 00:20 4th optimization: Images endpoint

5 mins

- Use PerfView to view results of a memory profile, GC stats, etc. of the images endpoint
- Reduce allocations, show memory profile LOH, etc.
- Add caching headers support by using in-box result types

## 00:25 5th optimization: EF Core

7 mins

- Show flame graph of CPU profile captured by dotnet-trace in SpeedScope
- Disabling tracking
- Change to keyset pagination
- Use DbContext pooling
- Disable DbContext pooling
- Using compiled queries
- Using compiled models
- Using raw SQL

## 00:32 6th optimization: Removing EF Core

3 mins

- Show that's there still time and allocations occurring due to EF Core (use VS tools again)
- Removing EF Core saves allocations and processing
- Could use Dapper (very popular) but we'll use Nanorm with a manual mapping method (as it's native AOT friendly)

## 00:35 7th optimization: Startup time

5 mins

- Use a harness app to properly measure app startup time inc. sending a first request on server ready
- At this point we've made enough changes to the app that we can enable native AOT
- Talk about native AOT benefits and trade-offs
- Enable native AOT and show startup time improvement

## 00:40 8th optimization: Blazor SSR app that uses the API

- Optimize HttpClient that calls the API
- Change to not buffer whole JSON response, use ReadAsJsonAsync<T> helpers instead
- Add image forwarder using MapForwarder from YARP

## 00:45 End

## BONUS 1 Compare to gRPC

- Compare to gRPC implementation of API backend

## BONUS 2 Show Razor Slices for highly optimized non-interactive HTML endpoints

- Change frontend to render HTML using Razor Slices for highly optimized read-only pages
- Low allocation, low overhead
- Can be tactically used for specific endpoints where the benefits are worthwhile
