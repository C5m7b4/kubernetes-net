# kubernetes for .net cor apis

firstly we are going to create a blank folder in our dev directory

then we are going to initialize it

```js
npm init -y
```

then let's create a README.md in the root of the directory for our notes.

now we are going to create a file in the root called .gitignore. We'll add stuff to it as we see the need. This is not going to be a typical node project, so we won't use npx gitignore node.

now initial the git repository

```js
git init
git add .
git commit -m "initial commit"
```

then we are going to go ahead and link this up with our github repo. Refresh the page and we should see our readme and our repo should be setup and we are ready to get started.

firstly, we will create our first branch to work off of

```js
git checkout -b branch1
```

## branch 1

This code comes from this awesome [youtube video](https://www.youtube.com/watch?v=DgVjEo3OGBI)

Here we are going to take a look at slides 01 through 03. I am not a powerpoint guy, but I give my best shot:

now we should play through the service architechture powerpoint presention:
![alt service-archiceture](images/01-service-architecture.png)

now we should play through the platform service Architecture presention:

![alt platform-service-architecture](images/02-platform-architecture.png)

now we should play through the command service Architecture presentation:

![alt commmand-architecture](images/02-command-acrhitecture.png)

thats it for our introduction into what we are going to build, so let's commit this sad powerpoint work and get to writing some code.

## branch 4

let's get started by creating a few projects

first lets check out version of .NET to make sure everything is cozy, so let's run

```js
dotnet --version
```

![alt dotnet-version](images/03-dotnet-version.png)

for our first project, we are going to run this command

```js
dotnet new webapi -n PlatformService
```

now our folder structure should look something like this:

![alt folder-structure](images/04-folder-structure.png)

we can open that folder in vscode by typing

```js
code -r PlatformService
```

now when this opens, you may see a prompt like this:

![alt prompt](images/05-install%20prompt.png)

definately choose yes, becuase this will give you the ability to debug your applications with ease.

now your folder structure inside of platform service should look like this:

![alt platformservice-structure](images/06-platformservice-structure.png)

the first thing we are going to do with this project is to delete the WeatherForecast.csl file and delete the Controller for that as well.

let's open the PlatformService.csproj file so we can make sure that as we install our dependencies, that they actually get installed. This will be helpfull fo sure. So, let's install some dependencies:

```js
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

now your PlatformService.csproj should look like this:

![alt packages](images/07-packages.png)

just for clarity, i'll put the code here are well

```js
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="6.0.10" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="6.0.10">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="6.0.10" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.10" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.2.3" />
  </ItemGroup>

</Project>

```

## branch 5

now, create a folder in the root of our project called Models and create a file inside of there called Platform.cs. This is what this file should look like. Its pretty straight forward

as a side note, once your are in a class, you can add properties with the shortcut by just typing prop, and then you will get intellisence which will help you out.

![alt shortcut](images/08-shortcut.png)

```js
namespace PlatformService.Models
{
  public class Platform
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Publisher { get; set; }
    public string? Cost { get; set; }
  }
}
```

now we are going to add some annotations to this, but we are going to get some squigglies, so let's take a look at how to fix those squigglies:

![alt squigglies](images/09-squigglies.png)

so, to fix this, just put the cursor inside of the offending word, and press ctrl-. and you will get some intellisense that will allow you to import the correct using statement that you need.

![alt helpers](images/010-helpers.png)

so, now our class should look like this:

```js
using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models
{
  public class Platform
  {
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]

    public string? Name { get; set; }
    [Required]
    public string? Publisher { get; set; }
    [Required]
    public string? Cost { get; set; }
  }
}
```

now, we are going to create our DbContext. Create a folder in the root of the project called Data and add a file inside of that called AppDbContext.cs

another shortcut that you can use is for creating constructors, so you just basically just type ctor and vscode will help you out. just press tab and vscode will automatically create a constructor for you.

![alt ctor](images/011-ctor.png)

```js
using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {

    }

    public DbSet<Platform> Platforms { get; set; }
  }
}
```

then we need to wire this up on our Program.cs file

```js
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
```

