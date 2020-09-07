# HandTrackerTest
## Basic Hand Tracking app. Made with Unity+OpenCV

- Program entry point is `Awake` method in the `GameController` class
- I really like the idea of a "self-registering" Monobehaviour components pattern. My implementation of this approach might seem a little "convoluted" at first, but only because it was originally designed it to work with big numbers of components across various scenes. I have developed a system that allows for easy setup and management of a Unity project with complex architecture. Since this project is pretty small I had to strip my system down a bit. There's only one `GameObject` on the scene that contains all references to all Unity components we interact with. You can find it under the name "ComponentHolders".
- The general idea behind the system is rather simple. First, there are services that are responsible for some meta, abstract and high-level logic. They also communicate with related controllers, which in turn control the components they access via component holders. It might be easier to think of components (inside holders) as something similar to MVC views, and controllers as, well, MVC controllers.
- OpenCV side of the app is based off this project http://simena86.github.io/blog/2013/08/12/hand-tracking-and-recognition-with-opencv/
I did some refactoring, and rewrote main script to make it work as a Unity plugin.

**Known issues:**
- This project was developed on Mac and unfortunately won't work on any other platform as it is.

**Little demo video:**

[![DEMO](https://img.youtube.com/vi/jNVBCaRX38A/0.jpg)](https://youtu.be/jNVBCaRX38A "DEMO")
