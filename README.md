# HandTrackerTest
## Basic Hand Tracking app. Made with Unity+OpenCV

The main goal of this project is to show some of my skills and capabilities. Certain logic might seem excessive and some naming might be weird, but that is only because I'm using my old code. I developed a system, that allows for easy setup and management of a complex architecture of a project developed with Unity. Since this project is pretty small I had to strip my system down a bit. 

- Program entry point is Awake method in the GameController class
- I really like the idea of a "self-registering" Monobehaviour components. My realization of this approach might seem a little convoluted because I originally designed it to work with big numbers of components and on various scenes. There's only one Gameobject on the scene that contains all references to all Unity components I interact with. It's named "ComponentHolders"
- The general idea behind the system is rather simple. First, there are services that are responsible for some meta, abstract and high-level logic. They also communicate with related controllers, which in turn control the components they access via component holders. It might be easier to think of components (in holders) as somewhat like views in MVC pattern, and controllers as, well, controllers.
- OpenCV side of the app is based off this project http://simena86.github.io/blog/2013/08/12/hand-tracking-and-recognition-with-opencv/
I did some refactoring, but overall logic was left the same.
- I wanted to test out the trick of passing an array of pixels by pointer,  from unmanaged code into managed space, without copying. I'm really glad I've had managed to make it work, although not perfectly.

Known issues:
- Unity editor keeps libraries in memory between play sessions, as it seems, and because of that, the actual hand detection won't work in playmode on the second time. After editor restart, everything works just fine. I'm still working on resolving this issue.
- This project was developed on the mac and unfortunately won't work on any other platform as it is. I started developing the native plugin with VS in virtual box, but it was taking too long this way, so I've decided to stick with macOS bundle to save time
