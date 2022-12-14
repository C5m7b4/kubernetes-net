# kubernetes for .net cor apis

firstly I want to give a shoutout to a guy named Les Jackson. He put a video up on youtube that is an 11 hour course on microservices. If you have not watched, you can find it [here](https://www.youtube.com/watch?v=DgVjEo3OGBI)

I really wanted to drive some of these concepts home for myself and possible others in the future, so I documented my entire 11 hour journey into this repo. Again, this not my work, but an inspiration, and something that can refer back to in the future when implementing microservices.

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

## branch 6

now we are going to add our repository. let's create an interface in the data folder. make a file named IPlatformRepo.cs

```js
using PlatformService.Models;

namespace PlatformService.Data
{
  public interface IPlatformRepo
  {
    bool SaveChanges();

    IEnumerable<Platform> GetAllPlatforms();
    Platform GetPlatformById(int id);
    void CreatePlatform(Platform plat);
  }
}
```

now create the concrete class to inherit from out interface. Create a filed called PlatformRepo.cs in the data folder

one thing you will notice right off, is that vscode is not happy with us:

![alt interfaces](images/012-interfaces.png)

we can use the same technique by putting the cursor inside of the word IPlatformRepo and pressing ctrl-. to get some intellisense:

![alt implement-interface](images/013-implement-interface.png)

after you click on implement interface, vscode will automatically stub out all of the methods that you need in order to fulfill the contract between the two.

it should not look like this:

```js
using PlatformService.Models;

namespace PlatformService.Data
{
  public class PlatformRepo : IPlatformRepo
  {
    public void CreatePlatform(Platform plat)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
      throw new NotImplementedException();
    }

    public Platform GetPlatformById(int id)
    {
      throw new NotImplementedException();
    }

    public bool SaveChanges()
    {
      throw new NotImplementedException();
    }
  }
}
```

let's start to fill this file out. we should start with a constructor first. don't forget about the ctor shortcut

let's look at a stub for this what we will use a lot in the upcoming exercises

![alt context](images/014-context.png)

to fix this, put the cursor in the _context word and press ctrl-. and select create private read-only field:

![alt read-only](images/015-read-only.png)

let's start at the bottom and fill out this class

```js
    public bool SaveChanges()
    {
      return (_context.SaveChanges() >= 0);
    }
```

```js
    public IEnumerable<Platform> GetAllPlatforms()
    {
      return _context.Platforms.ToList();
    }
```

```js
    public Platform GetPlatformById(int id)
    {
      var platform = _context.Platforms.FirstOrDefault(p => p.Id == id);
      if (platform == null)
      {
        return new Platform { };
      }
      else
      {
        return platform;
      }
    }
```

```js
    public void CreatePlatform(Platform plat)
    {
      if (plat == null)
      {
        throw new ArgumentNullException(nameof(plat));
      }

      _context.Platforms.Add(plat);
    }
```

now our final PlatformRepo.cs should look like this:

```js
using PlatformService.Models;

namespace PlatformService.Data
{
  public class PlatformRepo : IPlatformRepo
  {
    private readonly AppDbContext _context;

    public PlatformRepo(AppDbContext context)
    {
      _context = context;
    }
    public void CreatePlatform(Platform plat)
    {
      if (plat == null)
      {
        throw new ArgumentNullException(nameof(plat));
      }

      _context.Platforms.Add(plat);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
      return _context.Platforms.ToList();
    }

    public Platform GetPlatformById(int id)
    {
      var platform = _context.Platforms.FirstOrDefault(p => p.Id == id);
      if (platform == null)
      {
        return new Platform { };
      }
      else
      {
        return platform;
      }
    }

    public bool SaveChanges()
    {
      return (_context.SaveChanges() >= 0);
    }
  }
}
```

inn order to be able to inject this into our constructors, we need to register this with our Program.cs file. This is an important step in the process:

so, in Program.cs, add this little snippet of code:

```js
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
```

now, just to keep things tidy, we are going to make sure that our project builds:

```js
dotnet build
```

now, you may see a warning here, but we'll take care of that later on:

![alt build-error](images/016-build-error.png)

before we commit this, we need to add some stuff to our gitignore, because right now there appears to be a lot of files to commit. first let's check to see where they are coming from:

add this to the .gitignore file

```js
bin
obj
```

ok, now we can commit this branch before we move on

## branch 7

now we are going to seed out in-memory database so create a file in the Data folder called PrepDb.cs

let's just stub out the start of this file so we can add it to our Program.cs file:

```js
namespace PlatformService.Data
{
  public static class PrepDb
  {
    public static void PrepPopulation(IApplicationBuilder app, bool isProd)
    {

    }
  }
}
```

then in our Program.cs file, let's add this snippet just before the app.Run() command:

```js
PrepDb.PrepPopulation(app, app.Environment.IsProduction());
```

At first, our file looks like this:

![alt prep-db](images/017-prep-db.png)

so, lets get rid of that warning. go into the PlatformService.csproj file and commend out this line

![alt comment](images/018-comment.png)

now you wll see that this error will go away

new this file should look like this:

```js
using PlatformService.Models;

namespace PlatformService.Data
{
  public static class PrepDb
  {
    public static void PrepPopulation(IApplicationBuilder app, bool isProd)
    {
      using (var serviceScope = app.ApplicationServices.CreateScope())
      {
        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
      }
    }

    private static void SeedData(AppDbContext context, bool isProd)
    {
      if (!context.Platforms.Any())
      {
        Console.WriteLine("--> Seeding Data...");
        context.Platforms.AddRange(
          new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
          new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
          new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
        );

        context.SaveChanges();
      }
      else
      {
        Console.WriteLine("--> we already have data");
      }
    }
  }
}
```

now just for safety, lets run a build

```js
dotnet build
```

everything should be good. oh wait, i noticed we got a lot of nullable warnings so lets go back to our Platform.cs class in the models folder and remove the ? character

![alt build](images/019-build.png)

now let's run this command

```js
dotnet build
```

and we should see this:

![alt seeding](images/020-seeding-data.png)

we want to make sure that we are seeing the Seeding Data in our console.

## branch 8

now we are going to create some DTOs {Data Transformation Object}

so, lets create another folder in the root of our project called Dtos and create a file inside of that called PlatformReadDto.cs

```js
namespace PlatformService.Dtos
{
  public class PlatformReadDto
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public string Publisher { get; set; }

    public string Cost { get; set; }
  }
}
```

now let's create another Dto called PlatformCreateDto.cs

```js
using System.ComponentModel.DataAnnotations;

namespace PlatformService.Dtos
{
  public class PlatformCreateDto
  {
    [Required]
    public string Name { get; set; }
    [Required]
    public string Publisher { get; set; }
    [Required]
    public string Cost { get; set; }
  }
}
```

so now we have our Dtos, but we need a way to map these to our model.
let's commit what we have and in the next branch, we will create our mappers.

## branch 9

firstly, we need to register AutoMapper so in the Progam.cs file, we need to add this snippet:

```js
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
```

in the root of our project, we are going to create a new folder called Profiles and in there we are going to create a new file called PlatformProfiles.cs

```js
using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles
{
  public class PlatformProfile : Profile
  {
    public PlatformProfile()
    {
      // source -> target
      CreateMap<Platform, PlatformReadDto>();
      CreateMap<PlatformCreateDto, Platform>();
    }
  }
}
```

now let's make sure we don't have any errors, so we'll do a run

```js
dotnet run
```

## branch 10

now we are going to create our controller for this app, so let's create a file called PlatformsController.cs in the Controllers folder.

```js
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;

namespace PlatformService.Controllers
{
  [Route("api/Platforms")]
  [ApiController]
  public class PlatformsController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly IPlatformRepo _repo;

    public PlatformsController(IPlatformRepo repo, IMapper mapper)
    {
      _repo = repo;
      _mapper = mapper;
    }

    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
      Console.WriteLine("--> Getting Platforms");

      var platformItem = _repo.GetAllPlatforms();

      return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
    }
  }
}
```

now let's test everythig out up to this point, so we'll run ths command

```js
dotnet run
```

now we can actually test our work finally. We are going to use insomnia to test our api. here is the link to the [app](https://insomnia.rest/) or you can just type insomnia app into your brower and find the download

once that is loaded, we are going to go to the dashboard and create a new project:

![alt create](images/012-create.png)

![alt new](images/021-new.png)

![alt kubernetes](images/022-kubernetes.png)

now we have a clean slate to work with for this project

let create a new folder for this project:

![alt new-folder](images/023-new-folder.png)

and we are going to call this new folder PlatformService

now we are going to create a new request to test out our service:

![alt new-request](images/024-new-request.png)

now we need to figure out where our request needs to come from:

![alt localhost](images/025-localhost.png)

so, let's just fix this up by changing our ports, so open up Properties/luanchSettings.json and change the port to look like this:

```js
"applicationUrl": "https://localhost:5001;http://localhost:5000",
```

now let's re-run the application

```js
dotnet run
```

and we should see the ports change

![alt new-ports](images/026-new-ports.png)

now we are going to rename that request to 'Get all Platforms' and we are going to make it look like this:

![alt platforms](images/027-platforms.png)

we have our first successfull endpoint. let's move on shall we.

now let's add an endpoint for getting a platform by it's id

```js
    [HttpGet("{id}",Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
      var platformItem = _repo.GetPlatformById(id);

      if (platformItem != null)
      {
        return Ok(_mapper.Map<PlatformReadDto>(platformItem));
      }
      else
      {
        return NotFound();
      }
    }
```

let's spin it up and give that a try

```js
dotnet run
```

now in insomnia, let's test our request like this:

![alt get-platform-by-id](images/028-get-platform-by-id.png)

everything is working well, so we need one more endpoint, then on to docker and then on to kubernetes. so, let's add that final endpoint to create a new platform

```js
    [HttpPost]
    public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
      var platformModel = _mapper.Map<Platform>(platformCreateDto);
      _repo.CreatePlatform(platformModel);
      _repo.SaveChanges();

      var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

      return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
    }
```

now let's test it out

```js
dotnet run
```

and in insomnia, let's create a test like this:
one note, for this request, because it's going to be a post, we need to tell insomnia that we have a json body

![alt json-body](images/029-json-body.png)

![alt post](images/030-post.png)

we can also look at the headers to see the location parameter

![alt location](images/032-headers.png)

now if we run our Get all Platforms again, we should see our new platform

![alt get-all](images/031-get-all.png)

before we wrap up this branch, let's take a look at debugging our app, as that is very important when things don't go as planned. To do this, let's look at the debug section of vscode.

![alt debug-1](images/033-debug-1.png)

hit the play button and run that endpoint in insomnia, and you should see the code break:

![alt debug-2](images/034-debug-2.png)

we can also see that our app is running on two different ports:

![alt ports](images/035-ports.png)

let's take a look at our swagger documentation

![alt swagger](images/036-swagger.png)

## branch 11

make sure you have docker desktop running firstly:

![alt docker-desktop](images/037-docker-desktop.png)

now make sure to install the plugin into vscode

![alt plugin](images/038-plugin.png)

in the root of our project create a file called Dockerfile

here is how i am finding the image:

![alt find](images/039-find.png)

![alt docker-hub](images/040-docker-hub.png)

![alt tags](images/041-tags.png)

```js
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

WORKDIR /app

COPY *.csproj ./

RUN dotnet restore

COPY . ./

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app

COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "PlatformService.dll"]
```

make sure the filename matches with whats in the bin directory

![alt dll](images/042-dll.png)

now let's check our version of docker

![alt docker-version](images/043-docker-version.png)

let's now open up the Docker Desktop app and check to make sure that we have kubernetes enabled:

![alt enable-kubernetes](images/044-enable-kubernetes.png)

![alt enabled](images/045-running.png)

now let's build our first docker file

```js
docker build -t c5m7b4/platformservice .
```

be sure to use your docker hub username instead of mine and don't forget the period at the end, because that is the directory that it need to look to to find the Dockerfile. now your bash is going to go crazy, so no worries there and after it finishes doing all its work, you should be able to see your new image:

![alt image](images/046-image.png)

and you should also be able to see it in the Docker Desktop app:

![app image](images/047-docker-image.png)

let's spin up our image and give it a test run

```js
docker run -p 8080:80 -d  c5m7b4/platformservice
```

![alt first-run](images/048-first-run.png)

firstly, you should get a hash back letting us know our image is running, and you should also be able to see that the container is running in the Docker plugin in vscode. there are a few more things that we can do also

```js
docker ps
```

this will show us all running containers

![alt running-containers](images/049-running-containers.png)

if you want to see all containers that you might have that are not running, you can try this command

```js
docker ps -a
```

if you want to see all images that you've created

```js
docker images
```

you can also stop the container by using it's id

```js
docker stop 
```

sometimes it takes a little time to stop the container

one thing to note is that if we run the container again using the same command as before:

```js
docker run -p 8080:80 -d  c5m7b4/platformservice
```

![alt two-containers](images/050-two-containers.png)

notice that now, we have two of these containers.

so let's run docker ps again and get the container id so we can stop it

![alt second-container](images/051-second-container.png)

```js
docker stop 2224fc5941c3
```

so, to start a stopped container, use the id that was created for your

```js
docker start 2224fc5941c3
```

you can also use the plugin

![alt remove](images/052-remove.png)

now we are going to push our image up to docker hub. I'm not sure, but I think you have to do a docker login first, and then after that it should remember your credentials:

```js
docker push c5m7b4/platformservice
```

this will take a few minutes, but after it's finished, we can log into dockerhub and see our new image which we will need for kubernetes

![alt docker-hub](images/053-docker-hub.png)

next up, we'll test out our container

## branch 12

let's find out what containers that we have that we can start up first

```js
docker ps -a
```

![alt available-containers](images/054-available-containers.png)

now let's start our container up so we can test it

```js
docker start 6dcbea6544d9
```

you will see the container running in the Docker Desktop

![alt running-container](images/055-running-container.png)

lets now create a new folder in insomnia

![alt new-folder](images/056-new-insomnia-folder.png)

im going to call it Docker env, then create another folder inside of that for our PlatformService. Inside of that, create a new request called Get all Platforms

![alt get-all-platforms](images/057-get-all-platforms.png)

make sure you use port 8080 as that is what we specified when we spun up our container.

that should be good enough for now. YOu can create and test out the other two endpoints, but the point of all of this is to get to kubernetes. this was just a means to an end. so lets stop here and finally get on to kubernetes

## branch 13

kubernetes is a container orchestrator. it handles automatically restarting container even if they stop. it's often referred as k82 because of the number of letters

there are two main types of users

- developers
- administrators

we are to have basicaly containers and under that we will have pods

here we are going to look at the kubernetes powerpoint

## branch 14

let's make sure our folder structure looks something like this. You probably won't have images or powerpoints, but you should at least have the PlatformService folder

![alt structure](images/059-structure.png)

we are going to create a new folder called K8S. this folder is just going to hold our kubernetes deploy files, so let's open that up in vscode

we are going to do the first part of the powerpoint presention, so create a file called platforms-depl.yaml

it also might help with this to install this plugin into vscode

![alt yaml-plugin](images/060-yaml-plugin.png)

```js
apiVersion: apps/v1
kind: Deployment
metadata:
  name: platforms-depl
spec:
  replicas: 1
  selector:
    matchLabels:
      app: platformservice
  template:
    metadata:
      labels:
        app: platformservice
    spec:
      containers:
        - name: platformservice
          image: c5m7b4/platformservice:latest
```

this will basically deploy a container into a pod. spaced here are critical so we might run into some problems here, but we'll keep our fingers crossed as we move forward and test things out.

now we are going to try to run our deployment file

first, let's make sure that kubernetes is ready so run this command

```js
kubectl version --short
```

and you should see something like this:

![alt kubernetes-version](images/061-kube-version.png)

kubectl will be the command that we will use for everything kubernetes related as we will see in the near future.

to deploy our container, we are going to run this command:

```js
kubectl apply -f platforms-depl.yaml
```

hopefully you will see this:

![alt first-kubernetes-deployment](images/062-first-kubernetes-deployment.png)

now let's check to see if this really worked or not

```js
kubectl get deployments
```

![alt deployments](images/063-deployments.png)

at this point, depending on your computers specs, you may see that the ready column might have 0/1, so it might take a bit of time to spin all this up.

let's take a look at our pods

```js
kubectl get pods
```

![alt pods](images/064-pods.png)

also at this time, you might need to run this a few times, while waiting for all this to complete

if you take a look at our docker plugin, you will see that we are up and running:

![alt docker-plugin](images/065-docker-plugin.png)

if we now go to our docker desktop, it should look like this:

![alt docker-desktop](images/066-docker-desktop.png)

you can also click on the platformservice and see what the logs look like:

![alt logs](images/067-logs.png)

notice our seeding data console log that we provided

congrats, you just deployed your first kubernetes project

## branch 15

you may notice some weirdness when you commit your changes and then checkout the master branch, but it will sort itself out.

if it makes any difference, we can destroy and redeploy this like so:

```js
kubectl delete deployment platforms-depl
```

![alt destroy](images/068-destoy.png)

and now docker desktop is empty

```js
kubectl get deployments
```

![alt no-deployments](images/069-no-deployments.png)

lets now fire things back up

```js
kubectl apply -f platforms-depl.yaml
```

now things should look good in the vscode plugin and in Docker Desktop

now we are going to create a node port so we can access this container from our local computer

let's create a file in our K8S project called platforms-np-srv.yaml
the np is for node port

and it should look like this:

```js
apiVersion: v1
kind: Service
metadata: 
  name: platformnpservice-srv
spec:
  type: NodePort
  selector:
    app: platformservice
  ports:
    - name: platformservice
      protocol: TCP
      port: 8070
      targetPort: 80
```

i am going to experiment here a little bit and deviate from the tutorial, and I am going to try to use port 8070, because I normally have IIS running on my computer and it takes port 80, which was specified in the tutorial, but I want to test this out on port 8070. We'll see how that goes for me ????
I'm pretty sure the port 80 if the port of the service and the port 8070 is the port for my computer, but I guess we'll find out when we test it out. easy fix, if that's not the case though.

```js
kubectl apply -f platforms-np-srv.yaml
```

lets make sure it is working

```js
kubectl get services
```

![alt services](images/070-services.png)

let's get brave and test everything out now. so go back to insomnia, and we are going to create a new folder for our K8S and inside of that, create a new folder called PlatformService. then we are going to create a new test called Get all Platforms

!!!!!!!!!!!!!!!!!!!!!
disclaimer, I made a mistake. I though that the port 8070 was the one that i need to use but after some testing, i realized that this is the internal port, so we are going to delete this deployment, fix our mistake and then make it right.

so in our platforms-np-srv.yaml file, make these changes

```js
apiVersion: v1
kind: Service
metadata: 
  name: platformnpservice-srv
spec:
  type: NodePort
  selector:
    app: platformservice
  ports:
    - name: platformservice
      protocol: TCP
      port: 80
      targetPort: 80
```

then let's remove our deployment

```js
kubectl delete service platformnpservice-srv
```

now our service should be gone, so let's recreate it the correct way

```js
kubectl apply -f platforms-np-srv.yaml
```

now let see what our services look like

```js
kubectl get services
```

![alt re-deploy](images/071-re-deploy.png)

so now the ip that we need to test with is going to be the 31637 port, so let's setup insomnia like this:

![alt get-all-platforms](images/072-get-all-platforms.png)

all right!!!! this all looks really good. congrats if you made it this far. the next steps is to create our command service and get these boys talking to each other synchronously and asynchronously

## branch 16

let go to a command line at the root of our project and run this command

```js
dotnet new webapi -n CommandsService
```

![alt new-project](images/073-new-project.png)

now our folder stucture should look like so:

![alt new-folder-structure](images/074-new-folder-structure.png)

you will probably see this box pop up, so hit yes, so we can properyly debug this application

![alt suggestion](images/075-suggestion.png)

now let's open that in vscode and get started. open the CommandsService.csproj file so we can make sure that our dependencies that we are about to install do in fact show up.

```js
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
 dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

so now our CommandsService.csproj file should look like this:

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
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.2.3" />
  </ItemGroup>

</Project>

```

now, let's cleanup our project by removing the references to the weatherforcast

now in the properties folder under launchSetting.json, let's change our ports so when we fire up both services, the ports dont conflict with each other:

```js
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:34219",
      "sslPort": 44322
    }
  },
  "profiles": {
    "CommandsService": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:6001;http://localhost:6000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}

```

now let's just make sure this will run 

```js
dotnet run
```

just make sure that everything is working ok.

now let's create our first controller, so in the controllers folder create a file called PlatformsController.cs

```js
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
  [Route("api/c/platforms")]
  [ApiController]
  public class PlatformsController : ControllerBase
  {
    public PlatformsController()
    {

    }

    [HttpPost]
    public ActionResult TestInboundConnection()
    {
      Console.WriteLine("--> Inbound Post # command service");
      return Ok("Inbound test ok from platforms controller");
    }
  }
}
```

now we are going to test this with insomnia:

let's do a dotnet run command

```js
dotnet run
```

it should be running on port 6000

![alt port-6000](images/076-port-6000.png)

let'g go to insomnia and create folder for testing this out by creating a folder for the Command Service. Let's create a new request called 'Test inbound connection'

![alt inbound-connection](images/078-inbound-connection.png)

if we look at our console in vscode, we should see that we got that

![alt inbound-received](images/079-inbound-received.png)

## branch 17

let's go back to the PlatformService application in vscode and we'll make an inefficient way of comunicating between services.

so, to test this out, we are going to create a new folder called SyncDataServices

inside of that folder, we are going to create another folder called Http

inside of here we are going to create an interface, so create a file called ICommandDataClient.cs

```js
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
  public interface ICommandDataClient
  {
    Task SendPlatformToCommand(PlatformReadDto plat);
  }
}
```

now we will create the concrete class to go with our interface so create a filed called CommandDataClient.cs in that same Http folder.

```js
using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
  public class CommandDataClient : ICommandDataClient
  {
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public CommandDataClient(HttpClient httpClient, IConfiguration config)
    {
      _httpClient = httpClient;
      _config = config;
    }

    public async Task SendPlatformToCommand(PlatformReadDto plat)
    {
      var httpContent = new StringContent(
        JsonSerializer.Serialize(plat),
        Encoding.UTF8,
        "application/json");

      var response = await _httpClient.PostAsync("http://localhost:6000/api/c/platforms", httpContent);

      if (response.IsSuccessStatusCode)
      {
        Console.WriteLine("--> Sync POST to CommandSerice was OK!");
      }
      else
      {
        Console.WriteLine("--> Sync POST to CommandService was NOT OK!");
      }
    }
  }
}
```

now we really dont like having that url hard-coded, so let's store that url in our config file. so go to the appsettings.Development.json file and add this into it:

```js
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "CommandService": "http://localhost:6000/api/c/platforms"
}

```

then let's replace the url call with our config variable

```js
var response = await _httpClient.PostAsync(_config["CommandService"], httpContent);
```

now to use this, we need to register this in our Program.cs file like so, and we can put this just below the repo stuff.

```js
builder.Services.AddHttpClient<ICommandDataClient, CommandDataClient>();
```

let's commit this, and in the next section, we will actually test all this out.

## branch 18

first let's inject our new service into the controller

```js
    public PlatformsController(IPlatformRepo repo, IMapper mapper, ICommandDataClient commandDataClient)
    {
      _repo = repo;
      _mapper = mapper;
      _commandDataClient = commandDataClient;
    }
```

don't forget to add the create readonly field as well.

then come down to the post and make it look like this

```js
    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
      var platformModel = _mapper.Map<Platform>(platformCreateDto);
      _repo.CreatePlatform(platformModel);
      _repo.SaveChanges();

      var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

      try
      {
        await _commandDataClient.SendPlatformToCommand(platformReadDto);
      }
      catch (System.Exception ex)
      {
        Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
      }

      return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
    }
```

now let's check things out, so we do a dotnet build and then a dotnet run

now let's open our CommandService application in another instance of vscode and do a dotnet run on it so they are both running. Now if we create a plaform in our PlatformService, it should alert the CommandService of the new platform.

now that we have both of those up and running, let's go back to insomnia and create a platform and see what's happening here.

![alt create-platform](images/080-create-platform.png)

so, if we look at the console for our platform service, we should see this:

![alt ok](images/081-ok.png)

and if we look at the console of our CommandService, we should see this:

![alt ok](images/082-ok.png)

one thing to note here that i did notice, is that if you are having problems with the two talking, especially using https, this command might be helpful to run on both projects

```js
dotnet dev-certs https --trust
```

now if we kill the comand service and try to use the same endpoint, you will see that it's taking time.

![alt waiting](images/083-waiting.png)

it stil works, but here you can see that even though we specified asyn await, we still had to wait, and in our platformservice, we got this message:

![alt failure](images/084-failure.png)

## branch 19

let's get our command service running in kubernetes and setup our clusterip's for both:

before we can get our command service running in kubernetes, we need to dockerize it and push the image up to docker hub.

lets make sure that we are in our CommandService project and create a file in the root called Dockerfile:

```js
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

WORKDIR /app

COPY *.csproj ./

RUN dotnet restore

COPY . ./

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app

COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "CommandsService.dll"]
```

then let's open a terminal and type this command:

```js
docker build -t c5m7b4/commandservice .
```

then we'll push it up to docker hub

```js
docker push c5m7b4/commandservice
```

![alt docker-hub](images/085-docker-hub.png)

we probably should have tested this before we pushed it, but we'll just do that now

```js
docker run -p 8080:80 c5m7b4/commandservice
```

we should see this:

![alt command-service-running](images/086-command-service-running.png)

let's go ahead and stop that service

## branch 20

now it's time to go back to our K8s project

let's add this to the end of our platforms-depl.yaml file

```js
---
apiVersion: v1
kind: Service
metadata:
  name: platforms-clusterip-srv
spec:
  type: ClusterIP
  selector:
    app: platformservice
  ports:
  - name: platformservice
    protocol: TCP
    port: 80
    targetPort: 80
```

we'll leave that be for the moment and we will create a new deployment for our commandsService, so create a file called commands-depl.yaml

```js
apiVersion: apps/v1
kind: Deployment
metadata:
  name: commands-depl
spec:
  replicas: 1
  selector:
    matchLabels:
      app: commandservice
  template:
    metadata:
      labels:
        app: commandservice
    spec:
      containers:
        - name: commandservice
          image: c5m7b4/commandservice:latest
---
apiVersion: v1
kind: Service
metadata:
  name: commands-clusterip-srv
spec:
  type: ClusterIP
  selector:
    app: commandservice
  ports:
  - name: commandservice
    protocol: TCP
    port: 80
    targetPort: 80
```

it's basically just copying the platforms-depl.yaml file and change the names over. pretty basic setup.

## branch 21

we need to update our platformservice so open that project up

we need to let our production build know how to talk to the CommandService

so in appsettings.json, add this entry:

```js
"CommandService": "http://commands-clusterip-srv:80/api/c/platforms"
```

now, since we have changes our image we need to rebuild and publish it again

```js
docker build -t c5m7b4/platformservice .
```

```js
docker push c5m7b4/platformservice
```

now we should be good to go

## branch 22

let check to see where we are at, so open up the K8s project

```js
kubectl get deployments
```

we only have one deployment so far

![alt deployments](images/087-deployments.png)

```js
kubectl get pods
```

![alt pods](images/088-pods.png)

```js
kubectl get services
```

![alt services](images/089-services.png)

```js
kubectl apply -f platforms-depl.yaml
```

![alt deploy-platforms](images/090-deploy-platforms.png)

now lets check our services

```js
kubectl get services
```

everything should match here

we still need to force kubernetes to pull down the latest image, because although we created the service, it did not detect a change in our actual application

```js
kubectl rollout restart deployment platforms-depl
```

![alt restart](images/091-restart.png)

now we should be set with that, so let's deploy our command service

```js
kubectl apply -f commands-depl.yaml
```

![alt commands-service](images/092-commands-service.png)

now if we check our services

```js
kubectl get services
```

![alt services](images/093-services.png)


now let check out deployments

```js
kubectl get deployments
```

![alt deployments](images/094-deployments.png)

and now let's check our pods

```js
kubectl get pods
```

![alt pods](images/095-pods.png)

all is looking pretty good so far.

now if we go and look at our docker desktop

![alt docker-desktop](images/096-docker-desktop.png)


we can probably kill the two dead ones while we are at it

but not it's time to actually test these puppies out.

## branch 23

let's test all this out with insomnia

all this is going to happen in our K8s folder that we created in insomnia, so let's just make sure that the Get all Platforms is still working

![alt get-all-platforms](images/097-get-all-platforms.png)

that stil looks good

![alt create-platform](images/098-create-platform.png)

looks good from the outside, but let's check the logs in docker desktop

![alt command-service](images/099-command-service.png)

notice that last line --> Ibound Post # command service

that's what we want to see. now in our platform service:

![alt platform-service](images/100-platform-service.png)

once again, notice the last line. That is what we want to see. If for some reason, we are getting some crazy error about certificates, we can use this line on both projects:

```js
dotnet dev-certs https --trust
```

now, just to double check, run the Get all Platforms again and we should see our new platform:

![alt get-all-platforms](images/101-get-all-platforms.png)

!!!!!!!!!!!!!!!!! this is definately a start

as a side note, you may notice the redirection errors. that's becuase we are not using https. this error is coming from this line of code in both our platform and commands services:

if you look in the Program.cs file, you will find this line

```js
app.UseHttpsRedirection();
```

we could comment this out, but then we would have to rebuild and re-push to docker hub, so we'll leave in the warnings for now.

if you've made it this far, congrats, but there is a ton of more cool shit that we need to do

## branch 24

this is where we have come so far

![alt progress](images/102-progress.png)

and this is the next step:

![alt next-step](images/103-next-step.png)

we are going to setup the ingress nginx container as our gateway. the node port is good for testing, but not good for production.

let's do a google search for ingress nginx

and we want to find the one from kubernetes

![alt ingress-nginx](images/104-ingress-nginx.png)

here is the [link](https://github.com/kubernetes/ingress-nginx)

then lets checkout the getting started section, and then the one about using docker desktop. we will probably have to explore a way for aws as well.

```js
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.4.0/deploy/static/provider/aws/deploy.yaml
```

just for fun, type the url of this puppy into chrome and take a look at their yaml file. it's quite impressive.

but i digress...

let's but that whole thing in our clipboard, open up the K8S app because that is where we have been working with kubernetes. you don't actually have to do that, but I am trying to stay consistent

im going to paste that into my terminal and let it do its thing. Its actually pretty fast. now if we look at our docker plugin:

![alt docker-plugin](images/105-docker-plugin.png)

and if we look at docker desktop, we are going to see a ton of new stuff

![alt docker-desktop](images/106-docker-desktop.png)

now if we run

```js
kubectl get deployments
```

![alt deployments](images/107-deployments.png)

notice that we dont see nginx.same for pods. nginx is running in its own namespace, so let's take a look at what we have there

```js
kubectl get namespace
```

![alt namespaces](images/108-namespaces.png)

so, what we can do it this:

```js
kubectl get pods --namespace=ingress-nginx
```

![alt nginx](images/109-nginx.png)

notice how 2 of them completed. nginx goes through an initialize phase where it will create pods to create other pods so now if we look at docker desktop again

![alt docker-desktop](images/110-docker-desktop.png)

so, we can probably just get rid of the ones that have exited

now if we look at the services:

```js
kubectl get services --namespace=ingress-nginx
```

![alt services](images/111-serices.png)

notice we have a load balancer by default.

## branch 25

make sure that you are in the K8S project and create a new file called ingress-srv.

this is going to be our routing file fron nginx to our services

```js
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: ingress-srv
  annotations:
    kubernetes.io/ingress.class: nginx
    nginx.ingress.kubernetes.io/user-regex: 'true'
spec:
  rules:
    - host: acme.com
      http:
        paths:
          - path: /api/platforms
            pathType: Prefix
            backend:
              service:
                name: platforms-clusterip-srv
                port:
                  number: 80
          - path: /api/c/platforms
            pathType: Prefix
            backend: 
              service:
                name: commands-clusterip-srv
                port:
                  number: 80

```

spacing is very critical here!!!!!

since we used acme.com, we need to change our host file to that acme.com is associated with something

my location is 

```js
C:\Windows\System32\drivers\etc
```

im going to add this line

```js
127.0.0.1 acme.com
```

also, if you are running IIS, then make sure to stop if for this


now back in our K8S project, run this command

```js
kubectl apply -f ingress-srv.yaml
```

and we should see this:

![alt created](images/112-created.png)

now it's actually time to test the beast

back to insomnia

Im going to rename our PlatformService under the K8s to PlatformServer (Node Port)

then create another folder called PlatformService (Nginx)

let now create a new test:

this time we are going to use acme.com

![alt acme](images/113-acme-1.png)

sweet, things are starting to come together. in the next section, we'll setup our sql server and then start to move on to rabbitmq for our message bus

## branch 26

now we are going to setup our first sql server, and this is going to need some storage, so let's look to see what storeageclasses we already have

```js
kubectl get storageclass
```

![alt storeageclass](images/114-storeageclass.png)

there are three types of classes

- persistent volume claim
- persistent volume
- storeage class

back in our K8S project, let's create a new file called local-pvc.yaml for local persistent volume

```js
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: mssql-claim
spec:
  accessModes:
    - ReadWriteMany
  resources:
    requests:
      storage: 200Mi

```

this is our first basic setup and it will allow for persistent storage requesting 200 MB of storage. 

now we type this command:

```js
kubectl apply -f local-pvc.yaml
```

as a note, if you have already done this, you can remove it and start from scratch by running this command:

```js
kubectl delete pvc mssql-claim
```

now we can see what we have by running this command

```js
kubeclt get pvc
```

![alt persistent-storage](images/115-persistent-storage.png)

now if we run

```js
kubectl get pvc
```

we shoudl see this:

![alt pvc](images/116-pvc.png)

ok, for the next part, we are going to need to supply sql with an administrator password, so let's do some footwork first. let's check that we have not already stored a secret

```js
kubeclt get secrtes
```

hopefully you are seeing this

![alt secrets](images/117-secrets.png)

if for some reason, you are doing this series again for practice, you might see this:

![alt existing-secrets](images/118-existing-secrets.png)

you can remove this by using this command:

```js
kubectl delete secrets mssql
```

just trying to cover all the bases here. so now, we should be ready to get started. sorry for the long winded explanation.

now let's store our sa password into a secret. Now, obviously, this is a terrible thing altogether, but we'll probably come back aroung to this:

```js
kubectl create secret generic mssql --from-literal=MSSQSL_SA_PASSWORD="pa55w0rd!" 
```

![alt create-secret](images/119-create-secret.png)

now, i think we are ready to create our sql server so let's stop here for now.

## branch 27

now, we are back in our K8S project, we are going to create a new file called mssql-plat-depl.yaml

```js
apiVersion: apps/v1
kind: Deployment
metadata:
  name: mssql-depl
spec:
  replicas: 1
  selector:
    matchLabels:
      app: mssql
  template:
    metadata:
      labels:
        app: mssql
    spec:
      containers:
        - name: mssql
          image: mcr.microsoft.com/mssql/server:2017-latest
          ports:
            - containerPort: 1433
          env:
          - name: MSSQL_PID
            value: "Express"
          - name: ACCEPT_EULA
            value: "Y"
          - name: MSSQL_SA_PASSWORD
            valueFrom:
              secretKeyRef:
                name: mssql
                key: MSSQL_SA_PASSWORD
          volumeMounts:
          - mountPath: /var/opt/mssql/data
            name: mssqldb
      volumes:
      - name: mssqldb
        persistentVolumeClaim:
          claimName: mssql-claim

```

this is our basic setup, but we also have to add the networking, so we can copy the settings from our commands-depl.yaml file. we will copy the cluster section and make some neccessary changes:

```js
---          
apiVersion: v1
kind: Service
metadata:
  name: mssql-clusterip-srv
spec:
  type: ClusterIP
  selector:
    app: mssql
  ports:
  - name: mssql
    protocol: TCP
    port: 1433
    targetPort: 1433
```

i really want to play with these ports because I have sql server running on my laptop, and i would like to find a way to have these not interfere with each other, so we may need to play with this a little bit, but let's just get all this up and running and then we can look into making this our bitch.

now we want to be able to connect to this from our local laptop, so we'll add a load balancer

```js
---
apiVersion: v1
kind: Service
metadata:
  name: mssql-loadbalancer
spec:
  type: LoadBalancer
  selector:
    app: mssql
  ports:
    - protocol: TCP
      port: 1433
      targetPort: 1433   
```

so our whole file should look like this:

```js
apiVersion: apps/v1
type: Deployment
metadata:
  name: mssql-depl
spec:
  replicas: 1
  selector:
    matchLabels:
      app: mssql
  template:
    metadata:
      labels:
        app: mssql
    spec:
      containers:
        - name: mssql
          image: mcr.microsoft.com/mssql/server:2017-latest
          ports:
            - containerPort: 1433
          env:
          - name: MSSQL_PID
            value: "Express"
          - name: ACCEPT_EULA
            value: "Y"
          - name: MSSQL_SA_PASSWORD
            valueFrom:
              secretKeyRef:
                name: mssql
                key: MSSQL_SA_PASSWORD
          volumeMounts:
          - mountPath: /var/opt/mssql/data
            name: mssqldb
      volumes:
      - name: mssqldb
        persistentVolumeClaim:
          claimName: mssql-claim
---          
apiVersion: v1
kind: Service
metadata:
  name: mssql-clusterip-srv
spec:
  type: ClusterIP
  selector:
    app: mssql
  ports:
  - name: mssql
    protocol: TCP
    port: 1433
    targetPort: 1433
---
apiVersion: v1
kind: Service
metadata:
  name: mssql-loadbalancer
spec:
  type: LoadBalancer
  selector:
    app: mssql
  ports:
  - protocol: TCP
    port: 1433
    targetPort: 1433    
```

alright, lot's to process here. Let's stop here and when we come back, we'll spint this baby up.

## branch 28

now let's deploy this thing:

```js
kubectl apply -f mssql-plat-depl.yaml
```

cross your fingers

![alt sql-deploy](images/120-sql-deploy.png)

now lets run this command

```js
kubectl get services
```

![alt services](images/121-services.png)

and if we do a 

```js
kubectl get pods
```

![alt pods](images/122-pods.png)

here we have an error, so we are going to try to figure out why that is. this is not a bad thing, these yaml files are a bitch.

i verified the yaml file is good, so pretty sure that this is a secrets problem. let's tear down the deployment with 

```js
kubectl delete deployment mssql-depl
```

now let's clear out our secret

```js
kubectl delete secret mssql
```

and make sure they are cleared

```js
kubectl get secrets
```

redo our secret

```js
kubectl create secret generic mssql --from-literal=MSSQL_SA_PASSWORD="pa55w0rd!"
```

redploy our yaml file

```js
kubectl apply -f mssql-plat-depl.yaml
```

now let's check everything out

```js
kubectl get deployments
```

![alt deployments](images/123-deployments.png)

looks good, now let's check our pods

```js
kubectl get pods
```

![alt pods](images/124-pods.png)

sweet jesus!!!!!

OK, let's try to login to our sql server. You will need to bring up SQL Server Managment Studio. If you do not have it, you can get it [here](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)

try to connect up using this:

![alt sql-login](images/125-sql-login.png)

the login seems weird, it actually localhost,1433. I think we need to try and play with using different external ports, so we can actually run multiples of these, but for not, this should be find. also, if you are running sql on your local machine, shut it down because they will conflict because of the port. work in progress I guess, but still learning, so this will have to do for now.

we were able to login, but there are no databases yet:

![alt server](images/126-server.png)

but we can test the persistent storage by creating a database just as a test

![alt test-db](images/127-test-db.png)

now, we can close out of SSMS, and go to Docker Desktop and find our mssql instance and kill it:

![alt mssql](images/128-mssql.png)

shortly after we kill it, it will respawn, and after it comes back, we will log in again and make sure out test database is still there.

![alt test-db](images/129-test-db.png)

yep, still there. let's delete it because we don't need it. in the next branch, we'll update the platform service to actually use this sql server.

## branch 29

let's open back up our PlatformService app and get it ready to connect to our sql server when in production mode. in dev mode, we'll still use the inMemory database though.

open up the appsettings.json file and add this line:

```js
  "ConnectionStrings": {
    "PlatformsConn": "Server=mssql-clusterip-srv,1433;Initial Catalog=platformsdb;User Id=sa;Password=pa55w0rd!"
  }
```

now we need a conditional statement in our Program.cs to decide which database to use when in development vs production. If you are watching the youtube video, it's using .net5 and we are using .net6 which is a little different, so i had to figure a different way to do this, so, in our Program.cs file, we'll add this code:

```js
if (builder.Environment.IsDevelopment())
{
  Console.WriteLine("--> running in developement mode");
  builder.Services.AddDbContext<AppDbContext>(opt =>
      opt.UseInMemoryDatabase("InMem"));
}
else
{
  Console.WriteLine("--> running in production mode");
  builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
```

we are going to put that before this line:

```js
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
```

let's test everything out and make sure everybody is happy, so do a dotnet run command and just make sure that you are not getting any errors.
after this, we are ready to start setting up some migrations

## branch 30

before we start with migrations, we need to make a small change to our propdb.cs file, so in the DataFolder, open up PropDb.cs, and make this small change:

```js
      if (isProd)
      {
        Console.WriteLine("--> Attempting to apply migrations");
        try
        {
          context.Database.Migrate();
        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> could not run migrations: {ex.Message}");
        }
      }
```

we are going to have to comment out the line for migration because we are not totally there yet

now, it's time to setup our migrations: open up a command prompt and type

```js
dotnet ef migrations add initialmigration
```

and we get this lovely message:

![alt failed-migrations](images/130-failed-migrations.png)

there are a couple of work-arounds for this. Basically, this is just telling us that it has no idea what ef it, and this is something that Microsoft changed in .net 6, so let's run these commands:

```js
dotnet new tool-manifest
dotnet tool install --local dotnet-ef --version 6.0.10
```

![alt add-ef](images/131-add-ef.png)

![alt add tool](images/132-add-tool.png)

let's try the migrations one more time, but this time the command will change a little bit

```js
dotnet dotnet-ef migrations add initialmigration
```

now you are going to get a bunch of nasty error, so there is still a little work that needs to be done. the problem here is the the inMemory database does not support migrations, so we need to fake things out in order to get this up and running

```js
// if (builder.Environment.IsDevelopment())
// {
//   Console.WriteLine("--> running in developement mode");
//   builder.Services.AddDbContext<AppDbContext>(opt =>
//       opt.UseInMemoryDatabase("InMem"));
// }
// else
// {
  Console.WriteLine("--> running in production mode");
  builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
// }
```

basically, we are just telling our app that we are only using a real sql database. and this is just to get our migrations started.

and comment out this line as well

```js
// PrepDb.PrepPopulation(app, app.Environment.IsProduction());
```

we are also going to need to copy our connection string from the appsettings.json and past it in our appsettings.Development.json, and make a small change

```js
  "ConnectionStrings": {
    "PlatformsConn": "Server=localhost,1433;Initial Catalog=platformsdb;User Id=sa;Password=pa55w0rd!"
  }
```

that should be enough to get our migrations working

```js
dotnet dotnet-ef migrations add initialmigration
```

![alt migrations](images/133-migrations.png)

now there should be a new folder called migrations with 3 files in it:

![alt migrations-folder](images/134-migrations-folder.png)

now let's roll back the things that we commented out to get the migrations to work. also dont forget to go back to PrepDb in our Data folder and remove the comment on this line:

```js
context.Database.Migrate();
```

you may also have to import 

```js
using Microsoft.EntityFrameworkCore;
```

that's quite a chunk, so in the next branch, we'll rebuild our image, and push it back up to docker hub and check to see if our migrations are working or not.

## branch 31

back in our PlatformService, let's do another build

```js
docker build -t c5m7b4/platformservice .
```

dont forget the period at the end. and then push it up to docker hub

```js
docker push c5m7b4/platformservice
```

now just double check docker hub and make sure everything is cool.

now we need to update our kubernetes instance so it pulls down our latest version

it doesnt matter what command prompt you do this from, so you don't have to go back to the K8s project to do this.

```js
kubectl get deployments
```

![alt deployments](images/135-deployments.png )

we want to restart the platforms-depl

```js
kubectl rollout restart deployment platforms-depl
```

and now if you do

```js
kubectl get pods
```

you should see everything running. if for some reason, things are out of whack, like maybe you mispelled the password in the config, you can kill the deployment like this

```js
kubectl delete deployment platforms-depl
```

then fix your errors and redeploy

now, if you look at the logs for the platformservice in docke desktop, you should be able to see the migrations in action:

![alt migrations](images/136-migrations.png)

now just to really check, open up ssms and take a look at our database

![alt db](images/137-db.png)

![alt seeded-data](images/138-seeded-data.png)

now back to insomnia using the K8s version we can go a get all platforms

![alt get-all-platforms](images/139-get-all-platforms.png)

now let's create a platform

![alt create-platform](images/140-create-platform.png)

and now recheck our platforms again

![alt get-all-platforms](images/141-get-all-platforms.png)

now we'll check sql

![alt data](images/142-data.png)

there you go. we have come a long way so far, but much more to come

## branch 32

now we are going to fledge out our commandsservice to make it more real

so, let's open up the commandsService project and create a new folder called Models

we will create our first model inside that folder called Platform.cs

```js
namespace CommandsService.Models
{
  public class Platforms
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int ExternalId { get; set; }
  }
}
```

the next model we will create is our Command.cs file

```js
using System.ComponentModel.DataAnnotations;

namespace CommandsService.Models
{
  public class Command
  {
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public string HowTo { get; set; }
    [Required]
    public string CommandLine { get; set; }
    [Required]
    public int PlatformId { get; set; }
    public Platform Platform { get; set; }
  }
}
```

one thing I'm going to change about this project is in the CommandsService.csproj file

```js
<!-- <Nullable>enable</Nullable> -->
```

now back over to our Platform class, we will add this:

```js
using System.ComponentModel.DataAnnotations;

namespace CommandsService.Models
{
  public class Platform
  {
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public int ExternalId { get; set; }

    public ICollection<Command> Commands { get; set; } = new List<Command>();
  }
```

now let's just do a dotnet build to make sure everything is kosher

## branch 33

now we are going to create an in memory database for this for now, so create a new folder called Data and create a new file inside of that called AppDbContext.cs

```js
using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {

    }

    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Command> Commands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder
        .Entity<Platform>()
        .HasMany(p => p.Commands)
        .WithOne(p => p.Platform!)
        .HasForeignKey(p => p.PlatformId);

      modelBuilder
        .Entity<Command>()
        .HasOne(p => p.Platform)
        .WithMany(p => p.Commands)
        .HasForeignKey(p => p.PlatformId);
    }

  }
}
```

now let's just run a dotnet build just to check

now let's create a file in the Data folder called ICommandRepo.cs

```js
using CommandsService.Models;

namespace CommandsService.Data
{
  public interface ICommandRepo
  {
    bool SaveChanges();

    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform plat);
    bool PlatformExists(int platformId);

    IEnumerable<Command> GetCommandsForPlatform(int platformId);
    Command GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
  }
}
```

and now create the concrete class to go with the interface, so create a file called CommandRepo.cs in the Data folder:

```js
using CommandsService.Models;

namespace CommandsService.Data
{
  public class CommandRepo : ICommandRepo
  {
    private readonly AppDbContext _context;

    public CommandRepo(AppDbContext context)
    {
      _context = context;
    }
    public void CreateCommand(int platformId, Command command)
    {
      if (command == null)
      {
        throw new ArgumentNullException(nameof(command));
      }

      command.PlatformId = platformId;
      _context.Commands.Add(command);
    }

    public void CreatePlatform(Platform plat)
    {
      if (plat == null)
      {
        throw new ArgumentNullException(nameof(plat));
      }

      _context.Platforms.Add(plat);

    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
      return _context.Platforms.ToList();
    }

    public Command GetCommand(int platformId, int commandId)
    {
      return _context.Commands
        .Where(c => c.PlatformId == platformId && c.Id == commandId).FirstOrDefault();
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
      return _context.Commands
        .Where(c => c.PlatformId == platformId)
        .OrderBy(c => c.Platform.Name);
    }

    public bool PlatformExists(int platformId)
    {
      return _context.Platforms.Any(p => p.Id == platformId);
    }

    public bool SaveChanges()
    {
      return (_context.SaveChanges() >= 0);
    }
  }
}
```

now we are just going to run a dotnet build just to check

## branch 34

now we are going to create some Dtos, so create a folder  in the root of the project called Dtos. let's create our first file, PlatformReadDto.cs

```js
namespace CommandsService.Dtos
{
  public class PlatformReadDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
  }
}
```

now lets create the CommandReadDto.cs

```js
namespace CommandsService.Dtos
{
  public class CommandReadDto
  {
    public int Id { get; set; }
    public string HowTo { get; set; }
    public string CommandLine { get; set; }
    public int PlatformId { get; set; }
  }
}
```

now we are going to create one called CommandCreateDto.cs

```js
using System.ComponentModel.DataAnnotations;

namespace CommandsService.Dtos
{
  public class CommandCreateDto
  {
    [Required]
    public string HowTo { get; set; }
    [Required]
    public string CommandLine { get; set; }
  }
}
```

## branch 35

now it's time for automapper

let's open up the Program.cs file and add this under AddControllers

```js
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
```

now we create the folder called Profiles and create a file in there called CommandsProfile.cs

```js
using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.Profiles
{
  public class CommandsProfile : Profile
  {
    public CommandsProfile()
    {
      // source => target
      CreateMap<Platform, PlatformReadDto>();
      CreateMap<CommandCreateDto, Command>();
      CreateMap<Command, CommandReadDto>();
    }
  }
}
```

that's it for our automapper

## branch 36

still inside of the CommandsService project, let's open up the PlatformsController.cs. let's make these changes to it

```js
    private readonly ICommandRepo _repo;
    private readonly IMapper _mapper;

    public PlatformsController(ICommandRepo repo, IMapper mapper)
    {
      _repo = repo;
      _mapper = mapper;
    }
```

we are missing a major piece though. we need to setup our dependency injection for our repo. so back in program.cs, we need to add this:

```js
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
```

let make sure that it builds by running dotnet build

now let's go back to the PlatformsController.cs and add another endpoint

```js
    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
      Console.WriteLine("--> Getting Platforms from CommandsService");

      var platformItems = _repo.GetAllPlatforms();

      return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }
```

let's give this a go, so we'll spin it up by running dotnet run

let's go over to insomnia and create a test for that endpoint

![alt get-all-platforms](images/143-get-all-platforms.png)

looks good. we wouldn't expect to have any data in the inmemory database that we just spun up.

let's create our second controller called CommandsController.cs

```js
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
  [Route("api/c/platforms/{platformId}/[controller]")]
  [ApiController]
  public class CommandsController : ControllerBase
  {
    private readonly ICommandRepo _repo;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepo repo, IMapper mapper)
    {
      _repo = repo;
      _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
      Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");

      if (!_repo.PlatformExists(platformId))
      {
        return NotFound();
      }

      var commands = _repo.GetCommandsForPlatform(platformId);

      return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }
  }
}
```

we can start with this, but if we try to test it, we aren't going to get anything back, but that's ok for now.

lets do a dotnet run and head back to insomnia
![alt get-commands-for-platform](images/144-get-commands-for-platform.png)

this looks good for now, we are not expecting anything back from this just yet.

let's add another endpoint

```js

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
      Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

      if (!_repo.PlatformExists(platformId))
      {
        return NotFound();
      }

      var command = _repo.GetCommand(platformId, commandId);

      if (command == null)
      {
        return NotFound();
      }

      return Ok(_mapper.Map<CommandReadDto>(command));
    }
```

we can spin it up and make sure that the endpoint is working
so, do a dotnet run

we still get a not found

![alt not-found](images/145-not-found.png)

but if we check out log, we can see the results there

![alt arguments](images/146-arguments.png)

and now for our final route

```js
    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
    {
      Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

      if (!_repo.PlatformExists(platformId))
      {
        return NotFound();
      }

      var command = _mapper.Map<Command>(commandDto);

      _repo.CreateCommand(platformId, command);
      _repo.SaveChanges();

      var commandReadDto = _mapper.Map<CommandReadDto>(command);

      return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
    }
```

spin up a dotnet run and create this in insomnia

![alt post](images/147-post.png)

and also make sure that we can see our log because we are just going to get a 404 not found

![alt hit-create-command](images/148-hit-create-command.png)

## branch 37

now we are going to look into setting up rabbitmq as our messaging but. I need to do more research on this because there are features that I would like to learn. one thing to lookup also is amqp (advanced-messaging-queuing-protocal)

there are 4 types of exchange

- Direct
- Fanout
- Topic
- Header

so, let's go to our K8S project and create a new file called rabbitmq-depl.yaml

```js
apiVersion: apps/v1
kind: Deployment
metadata:
  name: rabbitmq-depl
spec:
  replicas: 1
  selector:
    matchLabels:
      app: rabbitmq
  template:
    metadata:
      labels:
        app: rabbitmq
    spec:
      containers:
        - name: rabbitmq
          image: rabbitmq:3-management
          ports:
            - containerPort: 15672
              name: rbmq-mgmt-port
            - containerPort: 5672
              name: rbmq-msg-port
---
apiVersion: v1
kind: Service
metadata:
  name: rabbitmq-clusterip-srv
spec:
  type: ClusterIP
  selector:
    app: rabbitmq
  ports:
  - name: rbmq-mgmt-port
    protocol: TCP
    port: 15672
    targetPort: 15672
  - name: rbmq-msg-port
    protocol: TCP
    port: 5672
    targetPort: 5672
---
apiVersion: v1
kind: Service
metadata:
  name: rabbitmq-loadbalancer
spec:
  type: LoadBalancer
  selector:
    app: rabbitmq
  ports:
  - name: rbmq-mgmt-port
    protocol: TCP
    port: 15672
    targetPort: 15672
  - name: rbmq-msg-port
    protocol: TCP
    port: 5672
    targetPort: 5672
```

now lets run the deployment

```js
kubectl apply -f rabbitmq-depl.yaml
```

![alt rabbit](images/149-rabbit-mq.png)

now let's get our deployments

```js
kubectl get deployments
```

![alt deployments](images/150-deployments.png)

it might take some time to start. notice in my screenshot, it is not started yet, so let's check out pods

```js
kubectl get pods
```

![alt deployments](images/151-deployments.png)

looking good now.

![alt services](images/152-services.png)

now let's open up a browser and go go localhost:15672

![alt rabbit](images/153-rabbit.png)

you can login with guest/guest

![alt dashboard](images/154-dashboard.png)

## branch 38

now let's go into our platform service

we need to add a dependency for RabbitMQ

```js
dotnet add package RabbitMQ.Client
```

now in our appsettings.Development.json we need to add our config

```js
  "RabbitMQHost": "localhost",
  "RabbitMQPort": "5672"
```

and in our appsettings.json, we need to add that config

```js
  "RabbitMQHost": "rabbitmq-clusterip-srv",
  "RabbitMQPort": "5672"
```

now let's create a dto, so in the Dtos folder create a file named PlatformPublishedDto.cs

```js
namespace PlatformService.Dtos
{
  public class PlatformPublishedDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Event { get; set; }
  }
}
```

now we need to create another mapping, so let's go into the Profiles folder and update that file:

```js
CreateMap<PlatformReadDto, PlatformPublishedDto>();
```

now let's create a folder in the root of our project and name it AsyncDataServices and create a file in there called IMessageBusClient

```js
using PlatformService.Dtos;

namespace PlatformService.AsyncDataService
{
  public interface IMessageBusClient
  {
    void PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
  }
}
```

now we are going to implement this interface so create a file calledMessageBusClient.cs. this is going to be kind of a lengthy file

```js
using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataService
{
  public class MessageBusClient : IMessageBusClient
  {
    private readonly IConfiguration _config;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration config)
    {
      _config = config;
      var factory = new ConnectionFactory() { HostName = _config["RabbitMQHost"], Port = int.Parse(_config["RabbitMQPort"]) };
      try
      {
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

        Console.WriteLine("--> Connected to Message Bus");
      }
      catch (System.Exception ex)
      {
        Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
      }
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
      Console.WriteLine("--> Message Bus has disconnected");
    }

    private void SendMessage(string message)
    {
      var body = Encoding.UTF8.GetBytes(message);

      _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);

      Console.WriteLine($"--> We have sent {message}");
    }


    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
      var message = JsonSerializer.Serialize(platformPublishedDto);

      if (_connection.IsOpen)
      {
        Console.WriteLine("--> Rabbit MQ Connection open, sending message...");
        SendMessage(message);
      }
      else
      {
        Console.WriteLine("--. Rabbit MQ connection is closed, not sendind");
      }
    }

    public void Dispose()
    {
      Console.WriteLine("Message Bus Disposed");
      if (_channel.IsOpen)
      {
        _channel.Close();
        _connection.Close();
      }
    }
  }
}

```

lets do a dotnet build just to make sure we haven't broken anything

now we just need to register our dependency injection

```js
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
```

after you commit, you may have some nasty errors, so use ctrl-shift-p and type reload to fix things up.

now let's go into the PlatformsController and setup our Dependency Injection

```js
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;
    private readonly IPlatformRepo _repo;

    public PlatformsController(IPlatformRepo repo, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
    {
      _repo = repo;
      _mapper = mapper;
      _commandDataClient = commandDataClient;
      _messageBusClient = messageBusClient;
    }
```

and then in our post, we'll add this code

```js
      try
      {
        var platforPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
        platforPublishedDto.Event = "Platform_Published";
        _messageBusClient.PublishNewPlatform(platforPublishedDto);
      }
      catch (System.Exception ex)
      {
        Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
      }
```

now let's fire up our PlatformService with a dotnet run

now let's open up the CommandService and do a dotnet run on it

then we'll go to  insomnia and on our local platform service, make sure we can still get all platforms and then let's create a platform

then in our command service, we should still see this

![alt command-service](images/155-command-service.png)

and i our PlatformService we should see our new logs

![alt platform-service](images/156-platform-service.png)

now if we blast the Create command, we can see in rabbitmq, our activity

![alt rabbit](images/157-rabbit.png)

## branch 40

now let's open up teh ComandsService and work on that for a little while.

first we need to add the RabbitMQ.Client dependency

```js
dotnet add package RabbitMQ.Client
```

make sure the check the CommandsService.csproj to make sure it got loaded.

now in our appsettings.Development.json, let's add our RabbitMQ config

```js
  "RabbitMQHost": "localhost",
  "RabbitMQPort": "5672"
```

then i nour appsettings.json, we need to add the config there as well

```js
  "RabbitMQHost": "rabbitmq-clusterip-srv",
  "RabbitMQPort": "5672"
```

now we are going to create some Dtos, so create a file in the Dto folder called PlatformPublishedDto.cs

```js
namespace CommandsService.Dtos
{
  public class PlatformPublishedDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Event { get; set; }
  }
}
```

now create another dto called GenericEventDto.cs

```js
namespace CommandsService.Dtos
{
  public class GenericEventDto
  {
    public string Event { get; set; }
  }
}
```

now we need to change our profile code to AutoMapper

```js
      CreateMap<PlatformPublishedDto, Platform>()
        .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
```

now i our ICommandRepo.cs file, add this little snippet

```js
bool ExternalPlatformExists(int externalPlatformId);
```

and then we need to implement this in the CommandRepo.cs file

```js
    public bool ExternalPlatformExists(int externalPlatformId)
    {
      return _context.Platforms.Any(p => p.ExternalId == externalPlatformId);
    } 
```

## branch 41

let's create a root folder in our CommandsService project called EventProcessing.

inside of there, create a file called IEventProcessor.cs

```js
namespace CommandsService.EventProcessing
{
  public interface IEventProcessor
  {
    void ProcessEvent(string message);
  }
}
```

then create another file called EventProcessor.cs

```js
using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
  public class EventProcessor : IEventProcessor
  {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
      _scopeFactory = scopeFactory;
      _mapper = mapper;
    }
    public void ProcessEvent(string message)
    {
      var eventType = DetermineEvent(message);


      switch (eventType)
      {
        case EventType.PlatformPublished:
          // To Do
          break;
        default:
          break;
      }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
      Console.WriteLine("--> Determining Event Type");

      var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

      switch (eventType.Event)
      {
        case "Platform_Published":
          Console.WriteLine("--> Platform Published Event detected");
          return EventType.PlatformPublished;
        default:
          Console.WriteLine("--> Could not determine event type");
          return EventType.Undetermined;
      }
    }

    private void AddPlatform(string platformPublishedMessage)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

        try
        {
          var plat = _mapper.Map<Platform>(platformPublishedDto);
          if (!repo.ExternalPlatformExists(plat.ExternalId))
          {
            repo.CreatePlatform(plat);
            repo.SaveChanges();
          }
          else
          {
            Console.WriteLine("--> Platform already exitss");
          }

        }
        catch (System.Exception ex)
        {
          Console.WriteLine($"--> Could not add platform to teh db {ex.Message}");
        }

      }
    }
  }

  enum EventType
  {
    PlatformPublished,
    Undetermined
  }
}
```

i know, this one is big

now in our Program.cs file, we need to register our dependency injection

```js
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
```

let's quickly do a build with dotnet build

and let's test a dotnet run, just to check

## branch 42

now let's create another folder in the root of the CommandsService project called AsyncDataServices

now in that folder, create a file called MessageBusSubscriber.cs

```js
using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
  public class MessageBusSubscriber : BackgroundService
  {
    private readonly IConfiguration _config;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IModel _channel;
    private string _queueName;

    public MessageBusSubscriber(IConfiguration config, IEventProcessor eventProcessor)
    {
      _config = config;
      _eventProcessor = eventProcessor;
      InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
      var factory = new ConnectionFactory() { HostName = _config["RabbitMQHost"], Port = int.Parse(_config["RabbitMQPort"]) };

      _connection = factory.CreateConnection();
      _channel = _connection.CreateModel();
      _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
      _queueName = _channel.QueueDeclare().QueueName;
      _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");

      Console.WriteLine("--> Listening on the Message Bus");

      _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      // do this one last
      stoppingToken.ThrowIfCancellationRequested();

      var consumer = new EventingBasicConsumer(_channel);

      consumer.Received += (ModuleHandle, ea) =>
      {
        Console.WriteLine("--> Event received");

        var body = ea.Body;
        var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

        _eventProcessor.ProcessEvent(notificationMessage);
      };

      _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

      return Task.CompletedTask;
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
      Console.WriteLine("--> Connection shutdown");
    }

    public override void Dispose()
    {
      if (_channel.IsOpen)
      {
        _channel.Close();
        _connection.Close();
      }


      base.Dispose();
    }
  }
}
```

now we just need to register this, so open up the Program.cs file and add this after the controllers

```js
builder.Services.AddHostedService<MessageBusSubscriber>();
```

now, we are ready to test

## branch 43

let's open up the platform service and do a dotnet build and dotnet run

then go back to the command service and do a dotnet build and a dotnet run

this is the important part

![alt listening](images/158-listening.png)

now over to insomnia and we are using the local versions for testing. make sure first that the Get all Platforms is still working, and then we are going to Create a platform and see what happens.

in our command service, we should see this:

![alt command-received](images/159-event-received.png)

and in our platform service, we should see this

![alt sent](images/160-sent.png)

we are missing something though, in the command service, open uup the EventProcessor class and let's first add a console message on lin 66 after we save the 

then in our ToDo section that we left around line 25,
we need to add this:

```js
      switch (eventType)
      {
        case EventType.PlatformPublished:
          AddPlatform(message);
          break;
        default:
          break;
      }
```

now we can restart the command service and test Creating a platform again with insomnia

now in our command service, we'll get some better logging:

![alt platform-added](images/161-platform-added.png)

now if we go to the CommandService in insomnia, we should be able to run the Get all platforms there and we should see the platform we added

![alt new-platform](images/162-new-platform.png)

now let's try the Create Command for Platform. note the id that was returned

![alt new-command](images/163-new-command.png)

actually, i messed that up. I need to the use id that that is in the previous screenshot

![alt new-command](images/164-new-command.png)

and now we should be able to run the Get all Commands for Platform

![alt all-commands](images/165-all-commands.png)

congrats, this was all a big step. next up, let's rebuild everthing and re-push our images to docker hub and refresh kubernetes and do some more testing

## branch 44

let's go back over to our platform service, shut everthing down and do a fresh docker build

```js
docker build -t c5m7b4/platformservice .
```

and now we will push it back up to docker hub

```js
docker push c5m7b4/platformservice
```

now we need to go over to our CommandService and do a build there as well

```js
docker build -t c5m7b4/commandservice .
```

and then push that up to docker hub

```js
docker push c5m7b4/commandservice
```

check our work on docker hub of course

next up, let's double check our deployments

```js
kubectl get deployments
```

![alt deployments](images/166-deployments.png)

now lets restart the platform-depl

```js
kubectl rollout restart deployment platforms-depl
```

make sure the pods are running 

```js
kubectl get pods
```

now we'll go over to insomnia and using our K8s Platform service let's create a platform and see what we get

![alt create-platform](images/167-create-platform.png)

now if we do a get all platforms

![alt platforms](images/168-platforms.png)

all is looking good. let check the logs

![alt logs](images/169-logs.png)

now let's restart the CommandService

```js
kubectl rollout restart deployment commands-depl
```

make sure the pods are running 

```js
kubectl get pods
```

now we'll create another platform

![alt create-platform](images/170-create-platform.png)

and take a look at the logs

![alt platform-added](images/171-platform-added.png)

now lets create a CommandService folder in our K8s insomnia folder and create the Get all Platforms request

![alt get-all-platforms](images/172-get-all-platforms.png)

![alt create-command](images/173-create-command.png)

![alt get-all-commands](images/174-get-all-commands.png)

in the next section we will look at grpc and really syncing our commands

## branch 45

in order to get grpc working, we are going to have to modify our platforms-depl.yaml file, so let's open up the K8s project and open that platforms-depl.yaml file

we just need to add one more port

```js
  - name: platformgrpc
    protocol: TCP
    port: 666
    targetPort: 666
```

redeploy our service

```js
kubectl apply -f platforms-depl.yaml
```

```js
kubectl get services
```

![alt services](images/175-services.png)

now you will notice the 666 port is present

now let's go back to the platform service because it is going to be the grpc server

now in our appsettings.json, we need to add this little bit of config
***** on a side note, i'm pretty sure that this has to go into appsetting.Production.json or we will get a clash of port 80 in developement mode.

```js
  "Kestrel": {
    "Endpoints": {
      "Grpc":{
        "Protocols": "Http2",
        "Url":"http://platforms-clusterip-srv:666"
      },
      "webApi":{
        "Protocols": "Http1",
        "Url":"http://platforms-clusterip-srv:80"
      }
    }
  }
```

we do need to add one package to our Platform service

```js
dotnet add package Grpc.AspNetCore
```

now we are going to jump back over to our command service and add the needed packages there

```js
dotnet add package Grpc.Tools
dotnet add package Grpc.Net.Client
dotnet add package Google.Protobuf
```

now let's go back into our PlatformService and create the proto file
let's create a folder in the root called Protos and we are going to create a file in there called platforms.proto

```js
syntax = "proto3";

option csharp_namespace = "PlatformService";

service GrpcPlatform {
  rpc GetAllPlatforms (GetAllRequests) returns (PlatformResponse);
}

message GetAllRequests {}

message GrpcPlatformModel{
  int32 platformId = 1;
  string name = 2;
  string publisher = 3;
}

message PlatformResponse{
  repeated GrpcPlatformModel platform = 1;
}
```

this one is a bit strange

now we need to go to the csproj file and add this

```js
  <ItemGroup>
    <Protobuf Include="Protos\platforms.proto" GrpcServices="Server" />
  </ItemGroup>
```

now let's do a dotnet build

now if we need in obj\Debug\Protos we will see the autogenerated code

now let's create a folder inside our SyncDataServices folder called Grpc and create a file in there called GrpcPlatformService.cs

```js
namespace PlatformService.SyncDataServices.Grpc
{
  public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
  {
    
  }
}
```

but before we can finish this file, we need to go back into the Profiles folder and add another map

```js
      CreateMap<Platform, GrpcPlatformModel>()
        .ForMember(dest => dest.PlatformId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publi
```

now let's go back into the GrpcPlatformService.cs file

```js
using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
  public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
  {
    private readonly IPlatformRepo _repo;
    private readonly IMapper _mapper;

    public GrpcPlatformService(IPlatformRepo repo, IMapper mapper)
    {
      _repo = repo;
      _mapper = mapper;
    }

    public override Task<PlatformResponse> GetAllPlatforms(GetAllRequests request, ServerCallContext context)
    {
      var response = new PlatformResponse();
      var platforms = _repo.GetAllPlatforms();

      foreach (var plat in platforms)
      {
        response.Platform.Add(_mapper.Map<GrpcPlatformModel>(plat));
      }

      return Task.FromResult(response);
    }
  }
}
```

now in our Progam.cs file, we need to register Grpc right below our singleton

```js
builder.Services.AddGrpc();
```

then at the bottom of our Program.cs just before the app.Run, add these lines

```js
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.proto", async context =>
{
  await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.prot"));
});
```

now let's do a dotnet build

now let's try a dotnet run

we skipped branch46

## branch 47

now we move on to the Commandservice

in the appsettings.Development.json, add this config

```js
"GrpcPlatform": "https://localhost:5001"
```

and then in the production settings

```js
"GrpcPlatform": "http://platforms-clust:666"
```

let's make a folder in the root now called Protos and copy the file from the PlatformService project into this one

we are also going to copy the ItemGroup additions from the PlatformService found in teh csproj file with one change - Client mode

```js
  <ItemGroup>
    <Protobuf Include="Protos\platforms.proto" GrpcServices="Client" />
  </ItemGroup>
```

lets do a dotnet build

in the root, create a folder called SyncDataService and then another folder called Grpc and then inside of that a file called IPlatformDataClient.cs

```js
using CommandsService.Models;

namespace CommandsService.SyncDataServices.Grpc
{
  public interface IPlatformDataClient
  {
    IEnumerable<Platform> ReturnAllPlatforms();
  }
}
```

then create another file in the Grpc folder called PlatformDataClient.cs

```js
using AutoMapper;
using CommandsService.Models;

namespace CommandsService.SyncDataServices.Grpc
{
  public class PlatformDataClient : IPlatformDataClient
  {
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public PlatformDataClient(IConfiguration config, IMapper mapper)
    {
      _config = config;
      _mapper = mapper;
    }
    public IEnumerable<Platform> ReturnAllPlatforms()
    {
      throw new NotImplementedException();
    }
  }
}
```

at this point, let's set up the automapper so go into Profiles

```js
      CreateMap<GrpcPlatformModel, Platform>()
        .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.PlatformId))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Commands, opt => opt.Ignore());
```

now ack to our PlatformDataClient.cs file

```js
using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
  public class PlatformDataClient : IPlatformDataClient
  {
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public PlatformDataClient(IConfiguration config, IMapper mapper)
    {
      _config = config;
      _mapper = mapper;
    }
    public IEnumerable<Platform> ReturnAllPlatforms()
    {
      Console.WriteLine($"--> Calling Grpc Service: {_config["GrpcPlatform"]}");
      var channel = GrpcChannel.ForAddress(_config["GrpcPlatform"]);
      var client = new GrpcPlatform.GrpcPlatformClient(channel);
      var request = new GetAllRequests();

      try
      {
        var reply = client.GetAllPlatforms(request);
        return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
      }
      catch (System.Exception ex)
      {
        Console.WriteLine($"--> Could not call Grpc Server: {ex.Message}");
        return null;
      }
    }
  }
}
```

now in our Program.cs file, we need to add this

```js
builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();
```

dotnet build
dotnet run

now in the Data folder, create a file called PrepDb.cs

```js
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data
{
  public static class PrepDb
  {
    public static void PrepPopulation(IApplicationBuilder builder)
    {
      using (var serviceScope = builder.ApplicationServices.CreateScope())
      {
        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

        var platforms = grpcClient.ReturnAllPlatforms();

        SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
      }
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
    {
      Console.WriteLine("Seeding new platforms");

      foreach (var plat in platforms)
      {
        if (!repo.ExternalPlatformExists(plat.ExternalId))
        {
          repo.CreatePlatform(plat);
        }
        repo.SaveChanges();
      }
    }
  }
}
```

now in startup, we just need to add this one last line

```js
PrepDb.PrepPopulation(app);
```

## branch 48

we are back in the platform service again and do a dotnet run

now lets just check insomnia to make sure there is some data in there.

now we can go over to the comands servic and run dotnet run and when it starts up, it should use grpc to pull data from the PlatformService

![alt seeding](images/176-seeding.png)

![alt get-all-platforms](images/177-get-all-platforms.png)

head back over to the platformservice

```js
docker build -t c5m7b4/platformservice .

```

```js
docker push c5m7b4/platformservice
```

now back over to the CommandService

```js
docker build -t c5m7b4/commandservice .
```

```js
docker push c5m7b4/commandservice
```

let go back over to the PlatformService 

```js
kubectl get deployments
```

```js
kubectl rollout restart deployment platforms-depl
```

```js
kubectl get pods
```

make sure all pods are running and check the logs after it starts up
probably wouldnt hurt to check insomnia also to make sure we can get all platforms

```js
kubectl rollout restart deployment commands-depl
```

check the pods
check the logs

this one actually fails, so i need to do some more research

![alt crash](images/178-crash.png)

actually if we go back into the appsettings.json for the CommandServer,
we can fix the address:

```js
"GrpcPlatform": "http://platforms-clusterip-srv:666"
```

still inside of the CommandService

```js
docker build -t c5m7b4/commandservice .
```

```js
docker push c5m7b4/commandservice
```

check out deployments and restart the commandservice

```js
kubectl rollout restart deployment commands-depl
```

![alt success](images/179-success.png)

congrats for making it this far.

more food for thought:

- https/tls
- Event Process
- Service Discovery
- Bearer Authentication
- refactor endpoint return types
