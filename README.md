# GHLearning-EasyQuartzDashboard
[![.NET](https://github.com/gordon-hung/GHLearning-EasyQuartzDashboard/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/gordon-hung/GHLearning-EasyQuartzDashboard/actions/workflows/dotnet.yml) [![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/gordon-hung/GHLearning-EasyQuartzDashboard) [![codecov](https://codecov.io/gh/gordon-hung/GHLearning-EasyQuartzDashboard/graph/badge.svg?token=OZYAualf9y)](https://codecov.io/gh/gordon-hung/GHLearning-EasyQuartzDashboard)

## Purpose and Scope
GHLearning-EasyQuartzDashboard is a .NET application that provides a web-based dashboard for managing and monitoring Quartz.NET jobs. The system offers a comprehensive interface for job scheduling, execution tracking, and management while following Clean Architecture principles for maintainability and testability.

This overview document introduces the system's core capabilities, architectural approach, and major components. For more detailed information about the system architecture, see Architecture, and for specific details on Quartz.NET integration, refer to Quartz Integration.

## System Architecture
GHLearning-EasyQuartzDashboard follows Clean Architecture principles with clearly separated layers and dependencies flowing inward toward the domain core. 
The application is structured into five main projects:
| Project  | Description  | Responsibility |
|:--------|:--------|:--------|
| Web   | Presentation layer   | User interface, controllers, views, API endpoints    |
| Application   | Application layer   | Use cases, business logic orchestration, request handlers    |
| Core   | 	Domain layer   | Business rules, domain entities, repository interfaces    |
| Infrastructure   | Infrastructure layer   | External concerns, repository implementations, data access    |
| SharedKernel   | 	Cross-cutting concerns   | Common utilities, extensions, shared components    |

## Summary
GHLearning-EasyQuartzDashboard provides a comprehensive solution for Quartz.NET job management with a clean, maintainable architecture. The system demonstrates best practices in .NET application development, including:

- Clean Architecture for separation of concerns
- Mediator pattern for handling requests
- Repository pattern for data access
- Integration with modern technologies for scheduling, persistence, and monitoring

The following sections of this wiki provide more detailed information about specific aspects of the system.

## Sources
- [Quartz.NET](https://www.quartz-scheduler.net/)
- [Quartz.Net 持久化及 SignalR Core Dashboard的作法](https://dotblogs.com.tw/nethawk/2024/10/02/quartz-net-dashboard)
- [[ASP.NET Core] 將 Quartz.Net 排程作業 Host 於 ASP.NET Core 網站中，並以 SignalR 實現 Dashboard 頁面](https://dotblogs.com.tw/wasichris/2020/12/16/172524)
