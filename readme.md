# webauthdemo
The project shows how to add an MVP custom policy-based authorization to an ASP.NET application.

# Steps
1. Design and add policy requirements - [`UnggoyActionNameRequired`](./webauthdemo.unggoy/UnggoyActionNameRequired.cs) and
[`UnggoyTokenRequired`](./webauthdemo.unggoy/UnggoyTokenRequired.cs) records in the `webauthdemo.unggoy` project.
2. Name the policy. In this project the policy is named 'Unggoy'.
3. Add any attributes that shall apply the policy to API controllers - [`UnggoyActionAttribute`](./webauthdemo.unggoy/UnggoyActionAttribute.cs)
and [`UnggoyAuthorizeAttribute`](./webauthdemo.unggoy/UnggoyAuthorizeAttribute.cs) classes in the `webauthdemo.unggoy` project. 
4. Write the authorization handler for the policy - [`UnggoyAuthorizationHandler`](./webauthdemo.unggoy/UnggoyAuthorizationHandler.cs) class in the `webauthdemo.unggoy` project.
5. Write code that registers the policy with the service dependency injection container - the [`UnggoyExtensions`](./webauthdemo.unggoy/UnggoyExtensions.cs) class in the `webauthdemo.unggoy` project.
6. Register the created authorization component with the dependency injection container; look for `builder.Services.AddUnggoyAuthorization()` and `app.UseAuthorization()` in [Startup.cs](./webauthdemo.service/Program.cs);
note that `app.UseAuthorization()` must be called before `app.MapGrpcService` and `app.MapControllers` to ensure the correct order of authorization and action routing middleware.
7. Write the web service application and apply authorization attributes to the controllers. Note that the attributes can be applied to gRPC and WebAPI controller classes and methods.
A controller class may have a mixture of authorized and unauthorized action methods.

# Projects
- `webauthdemo.fscontrollers` - demonstrates how authorization can be applied to a WebAPI controller written in F#.
- `webauthdemo.grpc` - compiles gRPC protobuf definitions for use in other projects.
- `webauthdemo.service` - the application that hosts WebAPI and gRPC controllers with the Kestrel OWIN.
- `webauthdemo.test.component` - collection of unit tests that call the gRPC endpoints hosted in `webauthdemo.service`
- `webauthdemo.unggoy` - policy-based ASP.NET authorization components.