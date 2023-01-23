## Introduction
When it comes to making HTTP requests in a C# .NET application, it's common to use the built-in HttpClient class. However, there are some best practices and optimizations that can be made by using the HttpClientFactory class instead. In this guide, we will explore the benefits of using HttpClientFactory and show how to implement it in a .NET Core application.

## The Example App
The example console app will show how to use HttpClientFactory in a .NET Core application. It will include examples of how to configure HttpClient instances using HttpClientFactory, as well as how to use dependency injection to inject these instances into your application's classes.

#### Service Container Setup

Create a Service Collection if your in a non Web App. In a WebApi project this is usually setup in your builder.
```C#
var services = new ServiceCollection();
```
#### Add Endpoints to the Service Container
Add each endpoint and customize the client as required.
```C#
services.AddHttpClient(nameof(HttpClientNames.GitHubApi), client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
    client.DefaultRequestHeaders.Add("User-Agent", "PocHttpClientApp");
        
});
```
#### Finalize by Building the Container
Build your Service Provider and you are now able to pull instances. Its common to save an instance in a variable... for larger apps consider 
Just getting the service from ServiceCollection as needed.
```C#
var provider = services.BuildServiceProvider();
			IHttpClientFactory? clientFactory = provider.GetService<IHttpClientFactory>();
```

#### Create Efficient HttpClients on demand
Example of passing a factory argument and using it to Create a HttpClient instance. This client is already configured and reasy to go as configured in the earlier code.
```c#
	async static Task UsingHttpClient(IHttpClientFactory? factory)
    {
        try
        {
            string url = "http://yahoo.com";
            var client = factory.CreateClient("TestClient");
            var response = await client.GetAsync(url); 
```

## Socket Usage
When using HttpClient directly, each instance creates a new socket for each request. This can lead to a large number of open sockets, which can cause performance issues. HttpClientFactory uses a pool of sockets, which helps to reduce the number of open sockets and improve performance.

## Centralized Setup for Each HTTP Endpoint

When using HttpClient directly, it's common to configure the client in the same place where it's used. This can lead to a lot of duplicated code and make it difficult to update the configuration in one place. HttpClientFactory allows you to configure the HttpClient instances in the startup code while building your ServiceContainer, making it easier to update the configuration and apply it to all instances.

## Dependency Injection in .NET Core App
In a .NET Core application, it's common to use dependency injection to manage the lifetime of objects. HttpClientFactory can be easily integrated with dependency injection, allowing you to inject HttpClientFactory instances into your application's classes. This is very beneficial for writing unit tests.

## Easy to Read and Predictable Code
When using HttpClient directly, it can be difficult to understand how the client is being configured and used deep in your codebase. HttpClientFactory makes it easy to understand how the HttpClient instances are being configured and used, making the code more readable and predictable and keeping to the SOLID principles.

## Use of Polly Policies
Polly is a .NET library that allows you to add resiliency to your application by applying policies such as retry and circuit breaker. HttpClientFactory makes it easy to use Polly policies with your HttpClient instances, allowing you to add resiliency to your application with minimal effort.

## Named Client vs Typed Clients

HttpClientFactory allows you to create both named and typed clients. Named clients are useful when you have multiple clients that are configured the same way, while typed clients are useful when you have a single client that is used throughout the application.

In conclusion, using HttpClientFactory over HttpClient in C# .net application can save you a lot of time and effort by providing a centralized setup for each http endpoint, easy to read and predictable code, socket usage, dependency injection in .net core app, use of polly policies, named client vs typed clients. This can help you to improve the performance, maintainability and resiliency of your application.





