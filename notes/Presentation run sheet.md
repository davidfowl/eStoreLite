# Deep dive into .NET performance and native AOT

## 45:00 Introduction

1 min (Title slide)

- Speakers introduce themselves & topic

## 44:00 Upgrading .NET versions

2 mins (2 slides)

- Performance is a forever goal, i.e. every release we work on performance
- Improvements from 3.1, 5.0, 6.0, 7.0, 8.0 (using project template)
- Point to stoub's performances posts:
  - https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/
  - https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-6/
  - https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-5/
- Talk about TechEmpower, show numbers increasing from version to version

## 42:00 Introduce eShopLite on ASP.NET Core 8.0

3 mins (1 slide)

- Show the app, extracted from eShop
- Backend API app with
  - Items API
  - Images endpoint, image files stored on disk
- Frontend Blazor app that calls backend API app to display list of items

## 39:00 1st optimization: Blazor SSR app that uses the API

5 mins (Demo title slide)

- Optimize HttpClient that calls the API
- Change to not buffer whole JSON response, use `GetFromJsonAsync<T>` helpers instead
  - Use memory/allocation profiles to see memory use before & after changes
- Add image forwarder using MapForwarder from YARP

## 34:00 2nd optimization: Images endpoint

5 mins

- Use PerfView to view results of a memory profile, GC stats, etc. of the images endpoint
- Reduce allocations, show memory profile LOH, etc.
- Add caching headers support by using in-box result types

## 29:00 ** Shift to backend API optimization **

## 29:00 3rd optimization: EF Core

10 mins

- Use VS profile session for CPU and allocation profile, these become our baseline numbers for Catalog API
- Show flame graph of CPU profile captured by dotnet-trace in SpeedScope
- Disabling tracking
- Change to keyset pagination
- Use DbContext pooling
- Disable thead safety checks
- Using compiled queries
- Using compiled models
- Using raw SQL

## 19:00 4th optimization: Move to minimal APIs

6 mins

- Use bombardier to drive fixed load amount (e.g. 10000 requests) against backend API app
- See lots time and allocations in MVC
- Rewrite APIs to use minimal APIs
- Discuss different architecture, reduction of layers and processing

## 13:00 ** Shift to startup time optimization **

- Switch to `aot` branch

## 13:00 5th optimization: Removing EF Core

3 mins

- Show that's there still time and allocations occurring due to EF Core (use VS tools again)
- Removing EF Core saves allocations and processing
- Could use Dapper (very popular) but we'll use Nanorm with a manual mapping method (as it's native AOT friendly)

## 10:00 6th optimization: native AOT

5 mins

- Use a harness app to properly measure app startup time inc. sending a first request on server ready
- At this point we've made enough changes to the app that we can enable native AOT
- Talk about native AOT benefits and trade-offs
- Enable native AOT and show startup time improvement

## 05:00 Close out

3 mins

- Return to slides
- Recap performance gains from original version to optimized version
- Summarize calls to action

## 02:00 Q & A on Pubble if time (2 minutes buffer)

## BONUS 1 Compare to gRPC

- Compare to gRPC implementation of API backend

## BONUS 2 Show Razor Slices for highly optimized non-interactive HTML endpoints

- Change frontend to render HTML using Razor Slices for highly optimized read-only pages
- Low allocation, low overhead
- Can be tactically used for specific endpoints where the benefits are worthwhile
