# WebApiDotNetCore Optimal Charging Solution version
#@author A. Eduardo Pimentel

This is a solution to Optimal Charging created in ASPNET webapi (.NET 6.0)

It contains some starting points to:
- API project which contains ChargeProfileController
- Core project to handle the business logic 
- Tests project to handle tests created for the Core project logic

API project features:
- HttpPost: CalculateOptimalSchedule (receiving Schedule object and returning the Optimal Charging Solution)

Core project features:
- CalculateOptimalSchedule (implements logic to apply Optimal Charging for Schedule received)